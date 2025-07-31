using UnityEngine;

[System.Serializable]
public class RideEvent
{
    [Range(0f, 1f)] public float startTime;
    [Range(0f, 1f)] public float endTime;
    public float speed = 1f;
    public bool pauseAtStart = false;
    public float pauseDuration = 0f;
    public bool overrideRotation = false;
    public Transform target;
    public Vector3 customRotation; // could be quaternions but for ease of getting this done euler rotation is simple
}
