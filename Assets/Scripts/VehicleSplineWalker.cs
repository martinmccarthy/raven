using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class VehicleSplineWalker : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] private List<RideEvent> rideEvents = new();
    [SerializeField, Range(0f, 1f)] private float m_splineTime;
    [SerializeField] private float rotationSmoothness = 10f;

    [SerializeField] private InputActionReference m_pauseButton;
    [SerializeField] private InputActionReference m_restartButton;

    private int currentEventIndex = 0;
    private bool isPaused = false;

    private void OnEnable()
    {
        m_pauseButton.action.performed += OnPausePerformed;
        m_restartButton.action.performed += OnRestartPerformed;
    }

    private void Start()
    {
        if(rideEvents.Count > 0)
        {
            m_splineTime = rideEvents[0].startTime;
        }
    }

    private void Update()
    {
        if(isPaused || rideEvents.Count == 0 || currentEventIndex >= rideEvents.Count) return;

        RideEvent currentEvent = rideEvents[currentEventIndex];
        m_splineTime += currentEvent.speed * Time.deltaTime;
        m_splineTime = Mathf.Clamp01(m_splineTime);

        if (m_splineTime >= currentEvent.endTime)
            AdvanceEvent();

        m_splineContainer.Evaluate(m_splineTime, out float3 pos, out float3 tan, out float3 up);
        transform.position = pos;

        RotateVehicle(currentEvent, tan, up);
    }

    private void RotateVehicle(RideEvent currentEvent, float3 tan, float3 up)
    {
        Quaternion targetRotation = Quaternion.LookRotation(tan, up);
        if(currentEvent.overrideRotation)
        {
            int count = currentEvent.targets.Count;

            if(count > 0)
            {
                float normalizedT = Mathf.InverseLerp(currentEvent.startTime, currentEvent.endTime, m_splineTime);

                if(count == 1)
                {
                    Vector3 dir = (currentEvent.targets[0].position - transform.position).normalized;
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.0001f) dir.Normalize();
                    targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                }
                else
                {
                    float scaledT = normalizedT * (count - 1);
                    int index = Mathf.FloorToInt(scaledT);
                    int nextIndex = Mathf.Min(index + 1, count - 1);
                    float lerpT = scaledT - index;

                    Vector3 startDir = (currentEvent.targets[index].position - transform.position).normalized;
                    Vector3 endDir = (currentEvent.targets[nextIndex].position - transform.position).normalized;

                    startDir.y = 0f;
                  
                    endDir.y = 0f;
                    
                    startDir.Normalize();
                    endDir.Normalize();

                    Quaternion startRot = Quaternion.LookRotation(startDir, Vector3.up);
                    Quaternion endRot = Quaternion.LookRotation(endDir, Vector3.up);

                    targetRotation = Quaternion.Slerp(startRot, endRot, lerpT);
                }
            }
            else
            {
                targetRotation = Quaternion.Euler(currentEvent.customRotation);
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
    }

    private void AdvanceEvent()
    {
        currentEventIndex++;
        if(currentEventIndex < rideEvents.Count)
        {
            RideEvent next = rideEvents[currentEventIndex];

            if(next.pauseAtStart)
            {
                StartCoroutine(PauseRoutine(next.pauseDuration));
            }
        }
    }

    private IEnumerator PauseRoutine(float duration)
    {
        isPaused = true;
        yield return new WaitForSeconds(duration);
        isPaused = false;
    }

    private void OnDrawGizmos()
    {
        if (m_splineContainer == null || rideEvents == null) return;

        foreach(RideEvent rideEvent in rideEvents)
        {
            m_splineContainer.Evaluate(rideEvent.startTime, out float3 start, out _, out _);
            m_splineContainer.Evaluate(rideEvent.endTime, out float3 end, out _, out _ );

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(start, .2f);
            Gizmos.DrawSphere(end, .2f);
            Gizmos.DrawLine(start, end);

            if(rideEvent.overrideRotation && rideEvent.targets.Count > 0)
            {
                for (int i = 0; i < rideEvent.targets.Count; i++)
                {
                    Transform target = rideEvent.targets[i];
                    if (target == null) continue;

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine((Vector3)start, target.position);
                    Gizmos.DrawSphere(target.position, 0.2f);

                    if (i < rideEvent.targets.Count - 1)
                    {
                        // Draw a small connector between look targets to show order
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(rideEvent.targets[i].position, rideEvent.targets[i + 1].position);
                    }
                }
            }
        }
    }


    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;
    }

    private void OnRestartPerformed(InputAction.CallbackContext context)
    {
        currentEventIndex = 0;
        isPaused = false;
    }
}
