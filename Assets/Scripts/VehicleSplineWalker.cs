using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Transform))]
public class VehicleSplineWalker : MonoBehaviour
{
    [Tooltip("Reference to the SplineContainer in the scene")]
    public SplineContainer splineContainer;

    [Tooltip("Duration in seconds to travel the entire path")]
    public float travelTime = 10f;

    [Tooltip("Should the movement loop?")]
    public bool loop = true;

    // Optional offset to apply to spline positions
    public float3 offset = new float3(-15.85136f, 1f, -17.99869f);

    void Start()
    {
        if (splineContainer == null)
        {
            Debug.LogError("VehicleSplineWalker: Assign a SplineContainer.");
            return;
        }

        var spline = splineContainer.Spline;
        int count = spline.Count;
        if (count < 2)
        {
            Debug.LogError("VehicleSplineWalker: Spline needs at least 2 points.");
            return;
        }

        // Build point array with offset
        Vector3[] pts = new Vector3[count];
        for (int i = 0; i < count; i++)
            pts[i] = spline[i].Position + offset;

        // Place at start and face first segment
        transform.position = pts[0];
        transform.rotation = Quaternion.LookRotation((pts[1] - pts[0]).normalized, Vector3.up);

        // Split path into two segments for demonstration
        int mid = count / 2;
        Vector3[] pts1 = new Vector3[mid + 1];
        Vector3[] pts2 = new Vector3[count - mid];
        for (int i = 0; i <= mid; i++) pts1[i] = pts[i];
        for (int i = mid; i < count; i++) pts2[i - mid] = pts[i];

        float time1 = travelTime * 0.4f;
        float time2 = travelTime * 0.6f;

        // Sequence: move first segment, spin, then move second segment
        var seq = LeanTween.sequence();

        // 1) Move along first half with ease-in
        seq.append(
            LeanTween.moveSpline(gameObject, pts1, time1)
                     .setOrientToPath(true)
        );

        seq.append(
            LeanTween.rotate(gameObject, new Vector3(0f, 360f, 0f), time1)
        );

        // 3) Move along second half with ease-out
        seq.append(
            LeanTween.moveSpline(gameObject, pts2, time2)
                     .setOrientToPath(true)
        );
    }
}
