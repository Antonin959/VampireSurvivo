using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
public class player : MonoBehaviour
{
    public GameObject GameOverPannel, PausePannel;

    Rigidbody rb;

    const float jump_force = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (PausePannel.active)
            Time.timeScale = 0;
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount == 1)
                rb.velocity += new Vector3(0, 1, 0) * 10.0f;

            transform.eulerAngles = new Vector3(-90 - rb.velocity.y * 10, 90, 0);

            Time.timeScale = 1;

        }
    }

    void OnCollisionEnter(Collision coll)
    {
        GameOverPannel.SetActive(true);

    }
}
