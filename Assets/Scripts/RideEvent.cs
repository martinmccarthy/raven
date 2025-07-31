using System.Collections.Generic;
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
    public List<Transform> targets = new();
    public Vector3 customRotation; // could be quaternions but for ease of getting this done euler rotation is simple

    //public bool useRotationCurve = false;
    //public AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 360f);
    //public Vector3 rotationAxis = Vector3.up;
}
