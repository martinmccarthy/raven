using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] private string m_locationName = "";
    [SerializeField] private GameObject m_manipulateObject;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_clipToPlay;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Vehicle"))
        {
            switch (m_locationName)
            {
                case "Library":
                    m_manipulateObject.SetActive(false);
                    break;
                case "Lenore1":
                    PlayAudio();
                    break;
                case "Lenore2":
                    PlayAudio();
                    break;
                case "Raven":
                    PlayAudio();
                    break;
                default:
                    break;
            }
        }
    }

    private void PlayAudio()
    {
        m_audioSource.clip = m_clipToPlay;
        m_audioSource.Play();
    }
}
