using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class amoscript : MonoBehaviour
{
    public Vector3 dir;

    float lifetime = 0;

    void Update()
    {
        transform.Translate(dir * 10.0f * Time.deltaTime);
        lifetime += Time.deltaTime;
        if (lifetime > 3.0f)
            Destroy(gameObject);
    }
}
