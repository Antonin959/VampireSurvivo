using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[System.Obsolete]
public class VampirePlayer : MonoBehaviour
{
    public TMP_Text goldtext, wavetext;
    VampireManager vmanager;

    Transform Xppivot, Wavepivot;
    public GameObject gameover, ChoicePannel, BadChoicePannel, AuraSphere, Piege, explosion, StatsPannel;
    Rigidbody rb;
    public float life = 100.0f, maxlife = 100;
    float speed = 5.0f, damage = 20f,
          targetXP = 5, AddTargetXp = 5, AddAddTargetXp = 4,
          targetWave = 5, AddTargetWave = 5,
          AuraTimer = 2.0f, auraDamage = 0.5f,
          PiegeTimer = 5.1f;
    public float PiegeDamage = 80.0f;
    int xp = 0;
    bool onaura = false, onpiege = false, onfall = true;
    

    Rigidbody cam;
    MeshRenderer render;

    Vector3 mousePos = new Vector3(0, 0, 0);

    public GameObject XpCoin, DamageText;

    int choicestate = 0;

    int[] Choices = new int[3], BadChoices = new int[3];

    List<int> AllChoices = new List<int>(), AllBadChoices = new List<int>();

    void Start()
    {
        Time.timeScale = 1.0f;
        rb = GetComponent<Rigidbody>();
        cam = GameObject.Find("Main Camera").GetComponent<Rigidbody>();
        render = GetComponent<MeshRenderer>();
        Xppivot = GameObject.Find("Xppivot").transform;
        Wavepivot = GameObject.Find("WavePivot").transform;
        vmanager = cam.GetComponent<VampireManager>();
    }

    [System.Obsolete]
    void Update()
    {
        wavetext.text = "" + vmanager.wave;

        if (onaura)
            OnAura();
        if (onpiege)
            OnPiege();

        rb.velocity = (Input.GetAxisRaw("Vertical") * new Vector3(0, 0, 1) + Input.GetAxisRaw("Horizontal") * new Vector3(1, 0, 0)) * speed + new Vector3(0, rb.velocity.y, 0);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            mousePos = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            if (hit.transform.TryGetComponent(out VampireEnemie enemie) && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Mathf.Abs(hit.transform.position.y - transform.position.y) < 1)
                {
                    TextForDamage(hit.point, RoundBy(damage, 10) + "");

                    hit.transform.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    enemie.life -= damage;

                    VampireDeath(enemie);
                }
            }
        }

        if (rb.velocity.y < -7)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 1.0f))
            {
                if (onfall)
                {
                    life += (rb.velocity.y + 7)*20;
                    TextForDamage(transform.position, "" + RoundBy((rb.velocity.y + 7)*20, 10));
                    onfall = false;
                }
            }
        }
        else
            onfall = true;

        render.material.color = new Color(0, 0, Mathf.Clamp01(life / maxlife));


        if (life <= 0 || transform.position.y <= -50)
        {
            Time.timeScale = 0;
            gameover.SetActive(true);
        }

        goldtext.text = "" + xp;

        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angleRad = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        float angleDeg = (100 / Mathf.PI) * angleRad - 90;

        transform.rotation = Quaternion.LookRotation(mousePos - transform.position, Vector3.up);

        Vector2 PlayerScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (PlayerScreenPos.x > Screen.width * 0.75f)
            cam.velocity = (new Vector3(speed, 0, cam.velocity.z));
        else if (PlayerScreenPos.x < Screen.width * 0.25f)
            cam.velocity = (new Vector3(-speed, 0, cam.velocity.z));

        if (PlayerScreenPos.y > Screen.height * 0.75f)
            cam.velocity = (new Vector3(rb.velocity.x, 0, speed));
        else if (PlayerScreenPos.y < Screen.height * 0.25f)
            cam.velocity = (new Vector3(rb.velocity.x, 0, -speed));

        cam.velocity *= 0.9f;
        if (transform.position.y >= -3)
            cam.transform.position = new Vector3(cam.transform.position.x, transform.position.y + 7, cam.transform.position.z);


        Xppivot.transform.localScale = new Vector3(Mathf.Clamp01(1 - (targetXP - xp) / AddTargetXp), 1, 1);
        Wavepivot.transform.localScale = new Vector3(Mathf.Clamp01(1 - (targetWave - vmanager.wave) / AddTargetWave), 1, 1);

        if (xp >= targetXP)
        {
            AllChoices.Clear();
            for (int i = 0; i < 4; i++)
            {
                if (onaura && i == 1)
                    continue;

                AllChoices.Add(i);
            }

            if (!onpiege)
            {
                AllChoices.Add(4);
            }

            if (onaura)
            {
                for (int i = 5; i < 7; i++)
                    AllChoices.Add(i);
            }

            ChoicePannel.SetActive(true);
            Time.timeScale = 0;
            for (int i = 0; i < 3; i++)
            {
                Choices[i] = AllChoices[UnityEngine.Random.Range(0, AllChoices.Count())];
                ChoicePannel.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = choiceConvertor(Choices[i]);
                AllChoices.Remove(Choices[i]);
            }
            choicestate = 1;
            targetXP += AddTargetXp;

            AddAddTargetXp++;
            if (AddAddTargetXp >= 4)
            {
                AddTargetXp++;
            }
        }

        if (vmanager.wave >= targetWave)
        {
            AllBadChoices.Clear();
            for (int i = 0; i < 5; i++)
                AllBadChoices.Add(i);

            BadChoicePannel.SetActive(true);
            Time.timeScale = 0;
            for (int i = 0; i < 3; i++)
            {
                BadChoices[i] = AllBadChoices[UnityEngine.Random.Range(0, AllBadChoices.Count())];
                BadChoicePannel.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = badchoiceConvertor(BadChoices[i]);
                AllBadChoices.Remove(BadChoices[i]);
            }
            targetWave += AddTargetWave;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !BadChoicePannel.active && !ChoicePannel.active)
        {
            StatsPannel.SetActive(true);

            TMP_Text LeftText = StatsPannel.transform.GetChild(0).GetComponent<TMP_Text>(),
                    RightText = StatsPannel.transform.GetChild(1).GetComponent<TMP_Text>();

            LeftText.text = "max life= " + RoundBy(maxlife, 10) + "\ndamage= " + RoundBy(damage, 10);
            if (onaura)
            {
                LeftText.text += "\n\nAuraSphere damage = " + RoundBy(20 * auraDamage, 10) + "\nAuraSphere scale= " + RoundBy(AuraSphere.transform.localScale.x, 10);
            }
            if (onpiege)
            {
                LeftText.text += "\n\nPiege damage= " + PiegeDamage;
            }

            RightText.text = "enemie damage *= " + RoundBy(vmanager.DamageMultiplier, 10) + "\nenemie life *=" + RoundBy(vmanager.DamageMultiplier, 10) + "\nenemie speed *= " + RoundBy(vmanager.SpeedMultiplier, 10) + "\nspawn speed= " + RoundBy(vmanager.spawnSpeed, 10) + "s";
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            StatsPannel.SetActive(false);
        }
    }

    public void VampireDeath(VampireEnemie vampire)
    {
        if (vampire.life <= 0)
        {
            GameObject xp = Instantiate(XpCoin, vampire.gameObject.transform.position, Quaternion.identity);
            if (vampire.CompareTag("bossnormal"))
            {
                xp.GetComponent<Renderer>().material.color = Color.blue;
                xp.tag = "gold2";
            }
                Destroy(vampire.gameObject);
        }
    }


    void OnAura()
    {
        AuraTimer -= Time.deltaTime;
        if (AuraTimer <= 0)
        {
            AuraSphere.SetActive(true);
            StartCoroutine(AutoDisable(AuraSphere, 0.25f));
            Collider[] targets = Physics.OverlapSphere(transform.position, AuraSphere.transform.localScale.x*0.5f);

            foreach (Collider target in targets)
            {
                if (target.TryGetComponent(out VampireEnemie t))
                {
                    t.life -= 20 * auraDamage;
                    TextForDamage(target.transform.position, "" + (auraDamage * 20));
                    VampireDeath(t);
                }
            }
            AuraTimer = 1.0f;
        }
    }

    void OnPiege()
    {
        PiegeTimer -= Time.deltaTime;
        if (PiegeTimer <= 0)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 0.6f))
                Instantiate(Piege, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);
            PiegeTimer = 4.1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(mousePos, 0.2f);
        Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 10, Color.red);

        BoxCollider SpawnZone = transform.GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("gold"))
        {
            xp++;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("gold2"))
        {
            xp += 4;
            Destroy(other.gameObject);
        }
        
        else if (other.CompareTag("orbe"))
        {
            life -= 30;
            TextForDamage(other.gameObject.transform.position, "30");
            Destroy(other.gameObject);
        }
    }


    public void TextForDamage(Vector3 pos, string text, float scalespeed = 0.98f)
    {        
        TMP_Text dtxt = Instantiate(DamageText, pos + new Vector3(0, 1.0f, 0), Quaternion.Euler(90, 0, 0)).GetComponent<TMP_Text>();
        dtxt.text = text;

        if (scalespeed != 0.98f)
            dtxt.GetComponent<DamageTexting>().scalespeed = scalespeed;
    }

    public void OnTakeDamage(float damage)
    {
        life -= damage;
    }


    public void ChoiceButton(int choiceIndex)
    {
        if (choicestate >= 1)
        {
            executeChoice(Choices[choiceIndex]);
            ChoicePannel.SetActive(false);

            if (!BadChoicePannel.active)
                Time.timeScale = 1;
        }
    }
    public void BadChoiceButton(int badchoiceIndex)
    {
        executeBadChoice(Choices[badchoiceIndex]);
        BadChoicePannel.SetActive(false);

        if (!ChoicePannel.active)
            Time.timeScale = 1;
    }

    void executeChoice(int index)
    {
        switch(index)
        {
            case 0:
                damage *= 1.15f; TextForDamage(transform.position, "<color=\"green\">dmg= " + RoundBy(damage, 10), 0.995f); break;
            case 1:
                onaura = true; break;
            case 2:
                maxlife += 30; TextForDamage(transform.position, "<color=\"green\">life= " + RoundBy(maxlife, 10), 0.995f); break;
            case 3:
                life = maxlife; TextForDamage(transform.position, "<color=\"green\">regen!", 0.995f); break;
            case 4:
                onpiege = true; break;
            case 5:
                AuraSphere.transform.localScale *= 1.15f; TextForDamage(transform.position, "<color=\"green\">scale= " + RoundBy(maxlife, AuraSphere.transform.localScale.x), 0.995f); break;
            case 6:
                auraDamage += 0.2f; TextForDamage(transform.position, "<color=\"green\">dmg= damage * " + RoundBy(auraDamage, 10), 0.995f); break;
        }
    }

    void executeBadChoice(int index)
    {
        switch(index)
        {
            case 0:
                vmanager.DamageMultiplier *= 1.2f; TextForDamage(transform.position, "<color=\"orange\">dmg= x" + RoundBy(vmanager.DamageMultiplier, 10), 0.995f); break;
            case 1:
                vmanager.LifeMultiplier *= 1.2f; TextForDamage(transform.position, "<color=\"orange\">life= x" + RoundBy(vmanager.LifeMultiplier, 10), 0.995f); break;
            case 2:
                vmanager.spawnSpeed *= 0.9f; TextForDamage(transform.position, "<color=\"orange\">speed= " + RoundBy(vmanager.spawnSpeed, 10), 0.995f); break;
            case 3:
                vmanager.SpawnMutiplier *= 1.3f; TextForDamage(transform.position, "<color=\"orange\">spawn count= x" + RoundBy(vmanager.SpawnMutiplier, 10), 0.995f); break;
            case 4:
                vmanager.SpeedMultiplier *= 1.2f; TextForDamage(transform.position, "<color=\"orange\">speed= x" + RoundBy(vmanager.SpeedMultiplier, 10), 0.995f); break;
        }
    }

    string choiceConvertor(int index)
    {
        switch (index)
        {
            case 0:
                return "Dégats\nx1.15";
            case 1:
                return "Aura Sphere!";
            case 2:
                return "Vie totale\n+ 30";
            case 3:
                return "Regen";
            case 4:
                return "Pieges";
            case 5:
                return "Aura scale\nx1.15";
            case 6:
                return "Aura damage miltiplcateur\n+0.2";
        }
        return "-1";
    }
    string badchoiceConvertor(int index)
    {
        switch (index)
        {
            case 0:
                return "Dégats\nx1.2";
            case 1:
                return "Vie\nx1.2";
            case 2:
                return "Vitesse d'apparition\nx0.9";
            case 3:
                return "Quantité\nx1.3";
            case 4:
                return "Vitesse\nx1.2";
        }
        return "-1";
    }


    public float RoundBy(float value, float m)
    {
        return Mathf.Round(value * m) / m;
    }
    
    IEnumerator AutoDisable(GameObject obj, float timer)
    {
        yield return new WaitForSeconds(timer);
        obj.SetActive(false);
    }

    public IEnumerator DestroyAndExplode(GameObject obj)
    {
        GameObject exp = Instantiate(explosion, obj.transform.position, explosion.transform.rotation);
        yield return new WaitForSeconds(0.5f);
        Destroy(obj);
        Destroy(exp);
    }
}
