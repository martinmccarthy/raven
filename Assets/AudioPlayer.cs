using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AudioPlayer : MonoBehaviour
{
    public bool OnVehicle = false;
    public AudioClip clip;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.CompareTag("Vehicle"))
        {
            if(OnVehicle)
            {
                AudioSource a = other.GetComponent<AudioSource>();
                a.clip = clip;
                a.Play();
            }
            else
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
    }
}
