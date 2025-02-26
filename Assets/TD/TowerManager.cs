using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public bool state = false, ondebug = false;
    public float range = 5.0f, shottimer = 3.0f;

    float timer;

    bool UpState = false;

    Animator anim;
    Transform ammospawnpos;
    ParticleSystem shooteffect;

    public GameObject Ammo;

    void Start()
    {
        timer = shottimer;
        anim = transform.GetChild(0).GetComponent<Animator>();
        ammospawnpos = transform.GetChild(1);
        shooteffect = ammospawnpos.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        RaycastHit[] targets = Physics.SphereCastAll(transform.position, range, Vector3.up);
        Transform target = null;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].transform.tag == "red")
            {
                target = targets[i].transform;

                break;
            }
        }
        if (target == null)
        {
            timer = shottimer;

            if (UpState)
            {
                anim.Play("StandDown");
                UpState = false;
            }
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(((target.transform.position + target.transform.GetComponent<EnemieScript>().speed * target.transform.forward) - transform.position).normalized, Vector3.forward), 3);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = shottimer;
                Instantiate(Ammo, ammospawnpos.position, Quaternion.identity).GetComponent<amoscript>().dir = transform.forward.normalized;
                shooteffect.Play();
            }

            if (!UpState)
            {
                anim.Play("StandUp");
                UpState = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && ondebug)
        {
            Gizmos.color = new Color32(255, 255, 255, 20);
            Gizmos.DrawSphere(transform.position, range);
        }
    }
}
