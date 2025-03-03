using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class playerrscript : MonoBehaviour
{
    Vector3 basepos, baserot;

    public Transform drawings;
    public GameObject rangeSphere, enemie, viewEnemie, GameOverPannel;
    public MeshFilter placepreview = null;
    public GameObject[] Towers;

    float speed = 5.0f;
    int previewtower = 0;

    Transform LinkTowerObject = null;

    static float vie = 10;
    static int gold = 20;
    static TMP_Text LifeText, GoldText;
    TMP_Text viewEnemieLife;
    static Animator LifeTextAnim;

    int[] GoldCost = { 10, 15 };

    void Start()
    {
        LifeText = GameObject.Find("LifeText").GetComponent<TMP_Text>();
        LifeTextAnim = LifeText.GetComponent<Animator>();
        GoldText = GameObject.Find("GoldText").GetComponent<TMP_Text>();
        viewEnemieLife = viewEnemie.transform.GetChild(0).GetComponent<TMP_Text>();

        basepos = transform.position;
        baserot = transform.eulerAngles;
    }

    void Update()
    {
        GoldText.text = gold + "";

        Mov();
        TowerEdit();

        if (placepreview.gameObject.activeSelf && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.tag == "normal")
        {
            placepreview.transform.position = hit.point + new Vector3(0, 0.5f, 0);
            if (Input.GetKeyDown(KeyCode.F) && gold >= GoldCost[previewtower])
            {
                gold -= GoldCost[previewtower];
                Instantiate(Towers[previewtower], hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity).GetComponent<TowerManager>().towerType = previewtower;

                placepreview.gameObject.SetActive(false);
            }
            
        }
        else
            placepreview.transform.position = new Vector3(0, -100, 0);

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit2) && hit2.transform.CompareTag("red"))
        {
            viewEnemie.SetActive(true);
            viewEnemieLife.text = hit2.transform.GetComponent<EnemieScript>().vie + " pv";
        }
        else
            viewEnemie.SetActive(false);

        if (vie <= 0)
        {
            GameOverPannel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public static void PlayerDamage(float damage)
    {
        vie -= damage;
        LifeTextAnim.Play("damageTextEffect");
        LifeText.text = vie + "";
    }
    public static void TakeGold(int amount)
    {
        gold += amount;
        GoldText.text = gold + "";
    }

    void Mov()
    {
        basepos += (removeY(transform.forward).normalized * Input.GetAxis("Vertical") + removeY(transform.right).normalized * Input.GetAxis("Horizontal")) * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.CompareTag("tower"))
            {
                LinkTowerObject = hit.transform;
                Debug.Log(LinkTowerObject.gameObject, LinkTowerObject);
                LinkTowerObject = hit.transform.GetChild(0).GetChild(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            LinkTowerObject = null;
        }

        if (LinkTowerObject != null)
        {
            transform.position = LinkTowerObject.position;
            transform.rotation = LinkTowerObject.rotation;
        }
        else
        {
            transform.position = basepos;
            transform.eulerAngles = baserot;
        }

        if (Input.GetKey(KeyCode.Q))
            baserot += new Vector3(0, -200 * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.E))
            baserot += new Vector3(0, 200 * Time.deltaTime, 0);
    }

    void TowerEdit()
    {
        for (int i = 0; i < drawings.childCount; i++)
        {
            Destroy(drawings.GetChild(i).gameObject);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.gameObject.tag == "tower")
            {
                Instantiate(rangeSphere, hit.transform.position, Quaternion.identity, drawings).transform.localScale = hit.transform.GetComponent<TowerManager>().range * new Vector3(1, 1, 1);
            }
        }
    }

    public static Vector3 removeY(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public void SpawTower(int tower)
    {
        if (gold < GoldCost[tower])
            return;

        placepreview.gameObject.SetActive(true);
        placepreview.mesh = Towers[tower].GetComponent<MeshFilter>().sharedMesh;
        previewtower = tower;
    }
    
    public void PauseButton()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }
}