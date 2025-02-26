using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathscript : MonoBehaviour
{
    public float scale = 1.0f;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.GetChild(i).position, scale);
        }
    }
}