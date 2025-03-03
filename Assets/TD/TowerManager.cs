using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TowerManager : MonoBehaviour
{
    public bool state = false, ondebug = false;
    public float range = 5.0f, shottimer = 1.0f;

    float timer;

    bool UpState = false;

    Animator anim;
    Transform ammospawnpos;
    ParticleSystem shooteffect;

    public GameObject Ammo;

    public int towerType = 0;

    void Start()
    {
        timer = shottimer;

        if (towerType == 0)
        {
            anim = transform.GetChild(0).GetComponent<Animator>();
            ammospawnpos = transform.GetChild(1);
            shooteffect = ammospawnpos.GetChild(0).GetComponent<ParticleSystem>();
        }
        else if (towerType == 1)
        {
            ammospawnpos = transform.GetChild(1);
        }
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
            if (towerType == 0)
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
            else
            {
                timer -= Time.deltaTime;
                float scale = (1 - (timer / shottimer)) * range;
                ammospawnpos.transform.localScale = new Vector3(scale, ammospawnpos.localScale.y, scale);
                if (timer <= 0)
                {
                    timer = shottimer;
                }
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
