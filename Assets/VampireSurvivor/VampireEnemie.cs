using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[System.Obsolete]
public class VampireEnemie : MonoBehaviour
{
    NavMeshSurface surface;
    public GameObject Orbe, Cube;
    Transform player, LifePivot, LifeBar;
    VampirePlayer vplayer;
    VampireManager vmanager;
    NavMeshAgent agent;
    float speed = 2.0f, attacktimer = 2.0f, timervalue = 1.5f, damage;
    public float life, maxLife = 100;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        vmanager = GameObject.Find("Main Camera").GetComponent<VampireManager>();
        vplayer = player.GetComponent<VampirePlayer>();
        agent = GetComponent<NavMeshAgent>();

        LifeBar = transform.GetChild(1);
        LifePivot = LifeBar.GetChild(0);

        if (CompareTag("armored"))
        {
            transform.GetComponent<Renderer>().material.SetFloat("_Type", 1);
            maxLife = 200;
        }
        else if (CompareTag("explod"))
        {
            transform.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);
            surface = GameObject.Find("terrain").GetComponent<NavMeshSurface>();
            timervalue = 1.0f;
        }
        else if (CompareTag("bossnormal"))
        {
            maxLife = 800;
            damage *= 1.5f;
        }

        maxLife *= vmanager.LifeMultiplier;
        life = maxLife;
        attacktimer = timervalue;
    }
    void Update()
    {
        LifePivot.localScale = new Vector3(Mathf.Clamp01(life / maxLife), LifePivot.localScale.y, LifePivot.localScale.z);
        LifeBar.eulerAngles = new Vector3(-90, 0, 0);
        if (tag == "bossnormal")
            LifeBar.position = transform.position + new Vector3(0, 1.5f, 0.4f);
        else
            LifeBar.position = transform.position + new Vector3(0, 0.6f, 0.4f);

        if (CompareTag("normal") || CompareTag("bossnormal"))
        {
            Mouvement();
            AttackLowDistance();
            speed = 3.0f * vmanager.SpeedMultiplier;
        }
        else if (CompareTag("armored"))
        {
            Mouvement();
            AttackLowDistance();
            speed = 2.0f * vmanager.SpeedMultiplier;
        }
        else if (CompareTag("magic"))
        {
            Mouvement();
            AttackHighDistance();
            speed = 0.5f * vmanager.SpeedMultiplier;

        }
        else if (CompareTag("explod"))
        {
            Mouvement();
            OnExplode();
            speed = 4.0f * vmanager.SpeedMultiplier;
        }
    }

    void OnExplode()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 4.0f) && hit.transform.gameObject.CompareTag("Player"))
        {
            attacktimer -= Time.deltaTime;
            Debug.Log(attacktimer);
            if (attacktimer <= 0)
            {
                player.GetComponent<Rigidbody>().velocity += (player.transform.position - transform.position);
                GameObject c = Instantiate(Cube, transform.position, Quaternion.identity, surface.transform);
                float size = Random.Range(2.5f, 3.5f);
                c.transform.localScale = new Vector3(size, size, size);

                surface.BuildNavMesh();
                Destroy(gameObject);
            }

        }
        else
            attacktimer = timervalue;
    }

    void Mouvement()
    {
        agent.destination = player.position;
        agent.speed = speed;
    }
    void AttackLowDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0f))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                attacktimer -= Time.deltaTime;
                if (attacktimer < 0)
                {
                    damage = Random.Range(10.0f, 15.0f) * vmanager.DamageMultiplier;
                    vplayer.OnTakeDamage(damage);
                    vplayer.TextForDamage(hit.point, vplayer.RoundBy(damage, 10) + "");
                    attacktimer = 1.0f;
                }
            }
        }

        Debug.DrawRay(transform.position, transform.forward * 3.0f, Color.yellow);
    }
    void AttackHighDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, 20.0f))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                attacktimer -= Time.deltaTime;
                if (attacktimer < 0)
                {
                    Instantiate(Orbe, transform.position, Quaternion.identity).GetComponent<OrbeScript>().dir = (player.position - transform.position).normalized;
                    attacktimer = 5.0f;
                }
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "piege")
        {
            life -= vplayer.PiegeDamage;
            vplayer.TextForDamage(transform.position, vplayer.RoundBy(vplayer.PiegeDamage, 10) + "");
            vplayer.StartCoroutine(vplayer.DestroyAndExplode(other.gameObject));

            vplayer.VampireDeath(GetComponent<VampireEnemie>());
        }
    }
}
