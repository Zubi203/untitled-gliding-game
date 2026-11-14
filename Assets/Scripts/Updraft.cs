using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Updraft : MonoBehaviour
{
    [SerializeField] private float updraftForce = 100f;
    [SerializeField] private List<string> affectedTags = new List<string>{"Player"};


    void Awake()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void OnTriggerStay(Collider other)
    {
        bool validCollider = false;
        foreach (string tag in affectedTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                validCollider = true;
            }
        }

        if (!validCollider) return;

        if (other.isTrigger) return;

        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(Vector3.up * updraftForce, ForceMode.Acceleration);
        }
    }
}
