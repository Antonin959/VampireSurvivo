using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbeScript : MonoBehaviour
{
    float timer = 5.0f;
    public Vector3 dir;

    void Update()
    {
        transform.Translate(dir * 5.0f * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer < 0)
            Destroy(gameObject);
    }
}
