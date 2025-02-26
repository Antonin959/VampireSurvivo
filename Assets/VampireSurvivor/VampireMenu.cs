using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireMenu : MonoBehaviour
{
    bool OnMoving = false, CanMov = true;

    Transform player, enem1, enem2;
    float playerspeed, enemspeed, pz;

    float timer, side;

    void Start()
    {
        player = transform.GetChild(0);
        enem1 = transform.GetChild(1);
        enem2 = transform.GetChild(2);

        side = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    void Update()
    {
        if (!CanMov && !OnMoving)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                CanMov = true;
                side = Random.Range(0.0f, 100.0f) <= 50 ? 1 : -1;
                pz = Random.Range(-3f, 3f);
                Debug.Log(side);
            }
        }

        if (CanMov && !OnMoving)
        {
            player.position = new Vector3(10 * side, 1, 0);
            enem1.position = new Vector3(Random.Range(10.5f, 13f) * side, 1, pz + Random.Range(-0.5f, 0.5f));

            float rand = Random.Range(0f, 100f);
            if (rand > 50.0f)
            {
                enem2.position = new Vector3(Random.Range(10.5f, 13f) * side, 1, pz + Random.Range(1f, 2f));
                enem1.position = new Vector3(enem1.position.x, enem1.position.y, pz - Random.Range(1f, 2f));
            }
            else
                enem2.position = new Vector3(0, -1, 0);
            OnMoving = true;

            playerspeed = Random.Range(5.0f, 8.0f) * side;
            enemspeed = playerspeed - Random.Range(1.0f, 2.0f) * side;
        }

        player.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * playerspeed);
        enem1.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * enemspeed);
        enem2.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * enemspeed);
        if (OnMoving)
        {
            if (player.transform.position.x < -20 && side == 1 ||
                player.transform.position.x > 20 && side == -1)
            {
                CanMov = false;
                OnMoving = false;
                timer = 2.0f;
            }
        }
    }
}
