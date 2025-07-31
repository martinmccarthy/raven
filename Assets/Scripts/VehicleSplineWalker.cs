using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleSplineWalker : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] private List<RideEvent> rideEvents = new();
    [SerializeField, Range(0f, 1f)] private float m_splineTime;
    [SerializeField] private float rotationSmoothness = 10f;

    private int currentEventIndex = 0;
    private bool isPaused = false;


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
            if(currentEvent.target != null)
            {
                Vector3 dir = (currentEvent.target.position - transform.position).normalized;
                targetRotation = Quaternion.LookRotation(dir, up);
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

            if(rideEvent.overrideRotation && rideEvent.target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine((Vector3) start, rideEvent.target.position);
            }
        }
    }
}
