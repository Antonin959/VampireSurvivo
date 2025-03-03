using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextM : MonoBehaviour
{
    TMP_Text text;
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }
    void Update()
    {
        transform.localScale *= 0.99f;
        if (transform.localScale.x < 0.5f)
            Destroy(gameObject);
    }
}
