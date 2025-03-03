using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EnemieScript : MonoBehaviour
{
    public float speed = 2.0f;
    public int vie = 5;
    int index = 0;

    float stayDamageTimer = 0;

    public GameObject GoldTaker, TriggerIn = null;
    public TMP_Text damageText;
    Transform Canvas;

    public List<Vector3> path;
    List<Vector3> points = new List<Vector3>();

    void Start()
    {
        Canvas = GameObject.Find("Canvas").transform;

        for (int i = path.Count - 1; i >= 0; i--) points.Add(4 * path[i]);
        transform.position = points[0] + new Vector3(0, 3, 0);

        Debug.Log((points[0] + new Vector3(0, 1, 0)));
    }
    void Update()
    {
        Vector3 forward = playerrscript.removeY(transform.forward).normalized;

        transform.position += (forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(playerrscript.removeY((points[index] - transform.position).normalized), Vector3.up);
        if (Vector3.Distance(playerrscript.removeY(transform.position), playerrscript.removeY(points[index])) < 0.5f)
        {
            index++;
            if (index >= points.Count)
            {
                playerrscript.PlayerDamage(1.0f);
                Destroy(gameObject);
            }
        }

        if (TriggerIn != null)
        {
            stayDamageTimer -= Time.deltaTime;
            if (stayDamageTimer <= 0)
            {
                stayDamageTimer = 1.0f;
                TakeDamage();
            }
        }
        else
            stayDamageTimer = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ammo")
        {
            TakeDamage(other.gameObject);
        }
        if (other.tag == "café")
        {
            TriggerIn = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        TriggerIn = null;
    }

    void TakeDamage(GameObject other = null)
    {
        vie--;

        Instantiate(damageText, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, Canvas).text = "" + 1;
        
        if (other != null)
            Destroy(other.gameObject);

        Debug.LogWarning("DAMAGEDDD");

        if (vie <= 0)
        {
            Instantiate(GoldTaker, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, Canvas);
            Destroy(gameObject);
        }
    }
}
