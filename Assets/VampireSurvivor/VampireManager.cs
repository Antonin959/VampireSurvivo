using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.VFX;

[System.Obsolete]
public class VampireManager : MonoBehaviour
{
    public GameObject VampirePrefab;
    public Transform player;

    public int wave = 0, bosswave = 10;

    public float DamageMultiplier = 1.0f, spawnSpeed = 5.0f, SpawnMutiplier = 1.0f, SpeedMultiplier = 1.0f, LifeMultiplier = 1.0f;

    float magicrand = 60, armoredrand = 10, explod = 0;

    float timer = 5.0f;
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            wave++;
            DamageMultiplier *= 1.02f;
            LifeMultiplier *= 1.03f;
            spawnSpeed *= 0.99f;

            int amount = 1;
            float randamount = Random.Range(0f, 100f);
            if (randamount < 10)
            {
                amount = 1;
            }
            else if (randamount < 75)
            {
                amount = 2;
            }
            else if (randamount < 95)
            {
                amount = 3;
            }
            else
            {
                amount = 4;
            }

            for (int i = 0; i < amount; i++)
                OnSpawn();

            amount = (int)((float)amount * SpawnMutiplier);

            OnSpawn();
            timer = Random.Range(spawnSpeed, 2 * spawnSpeed);


            if (wave == 5)
            {
                magicrand = 50;
                armoredrand = 20;
                explod = 0;
            }
            else if (wave == 10)
            {
                magicrand = 70;
                armoredrand = 40;
                explod = 10;
            }
            else if (wave == 15)
            {
                magicrand = 60;
                armoredrand = 40;
                explod = 15;
            }

        }
    }

    void OnSpawn()
    {
        Vector3 AddPos = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
        RaycastHit hit;
        if (Physics.Raycast(transform.position + AddPos, transform.forward, out hit) && hit.collider.tag != "NotSurface")
        {
            if (wave == bosswave)
            {
                VampireEnemie v = Instantiate(VampirePrefab, hit.point, Quaternion.identity).GetComponent<VampireEnemie>();
                v.gameObject.tag = "bossnormal";
                v.gameObject.transform.localScale *= 2.5f;

                bosswave += 10;
            }
            else
            {
                string tag = "normal";
                float rand = Random.Range(0f, 100f);
                if (rand < explod)
                    tag = "explod";
                else if (rand < armoredrand)
                    tag = "armored";
                else if (rand < magicrand)
                    tag = "magic";


                Debug.Log(rand);
                VampireEnemie v = Instantiate(VampirePrefab, hit.point, Quaternion.identity).GetComponent<VampireEnemie>();
                v.gameObject.tag = tag;
            }
        }
    }
}
