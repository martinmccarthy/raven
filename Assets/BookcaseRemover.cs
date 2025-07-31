using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookcaseRemover : MonoBehaviour
{
    public GameObject bookcase;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Vehicle"))
        {
            bookcase.SetActive(false);
        }
    }
}
