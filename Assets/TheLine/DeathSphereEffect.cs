using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSphereEffect : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Rigidbody>().velocity = (transform.GetChild(i).position - transform.position).normalized * 10;
        }
    }
}
