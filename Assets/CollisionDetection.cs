using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] private string m_locationName = "";
    [SerializeField] private GameObject m_manipulateObject;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Vehicle"))
        {
            switch (m_locationName)
            {
                case "Library":
                    m_manipulateObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
