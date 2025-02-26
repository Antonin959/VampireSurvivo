using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageTexting : MonoBehaviour
{
    public float scalespeed = 0.99f;
    private void Update()
    {
        transform.localScale *= scalespeed;
        if (transform.localScale.x < 0.1f)
            Destroy(gameObject);
    }
}
