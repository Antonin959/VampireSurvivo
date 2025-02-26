using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemieScript : MonoBehaviour
{
    public float speed = 2.0f;
    public int vie = 5;
    int index = 0;

    public List<Vector3> path;
    List<Vector3> points = new List<Vector3>();

    void Start()
    {
        for (int i = path.Count - 1; i >= 0; i--) points.Add(4 * path[i]);
        transform.position = points[0] + new Vector3(0, 3, 0);

        Debug.Log((points[0] + new Vector3(0, 1, 0)));
    }
    void Update()
    {
        Vector3 forward = removey(transform.forward).normalized;

        transform.position += (forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(removey((points[index] - transform.position).normalized), Vector3.up);
        if (Vector3.Distance(removey(transform.position), removey(points[index])) < 0.5f)
        {
            index++;
            if (index >= points.Count)
                Destroy(gameObject);
        }
    }

    Vector3 removey(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ammo")
        {
            vie--;
            Destroy(other.gameObject);

            Debug.LogWarning("DAMAGEDDD");

            if (vie <= 0)
                Destroy(gameObject);
        }
    }
}
