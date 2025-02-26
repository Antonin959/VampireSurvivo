using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Obsolete]
public class Manager : MonoBehaviour
{
    List<GameObject> spawned = new List<GameObject>();
    Rigidbody player;
    public Light mainLight;
    public TMP_Text bestScore;
    Camera cam;

    TMP_Text scoreTxt;
    public GameObject Model_I, Model_D, Model_O, Model_IPetit, GameOver;

    float playerspeed = 3.0f, CamRandomRot = 0, CamTargetRot = 0, LensTimer, LensTarget = 0;

    float score = 0;

    Vector3 mouseWorldPos = new Vector3(0, 0, 0);

    public GameObject boxVolume;
    LensDistortion lens;

    bool loadBest = true;
    
    void Start()
    {
        spawned.Add(GameObject.Find("First_Obj"));
        player = GameObject.Find("player").GetComponent<Rigidbody>();
        scoreTxt = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        LensTimer = Random.Range(10f, 20f);
    }
    void Update()
    {
        if (!GameOver.active)
        {
            playerspeed += 0.075f * Time.deltaTime;
            score = player.transform.position.y * 0.5f; // 1 secondes = 2 point
            scoreTxt.text = (int)score + "";

            //Cycle jour nuit
            mainLight.intensity = 0.9f * Mathf.Cos((score * (Mathf.PI * 2)) * 0.001f);
            mainLight.transform.eulerAngles = new Vector3(mainLight.intensity * 360, mainLight.intensity * 360, 0);

            Effets();
            if (cam.transform.eulerAngles.z < CamTargetRot)
                cam.transform.eulerAngles += new Vector3(0, 0, 2.0f) * Time.deltaTime * (playerspeed / 3);
            if (cam.transform.eulerAngles.z > CamTargetRot)
                cam.transform.eulerAngles -= new Vector3(0, 0, 2.0f) * Time.deltaTime * (playerspeed / 3);

            boxVolume.GetComponent<Volume>().profile.TryGet(out lens);
            if (lens.intensity.value < LensTarget)
                lens.intensity.Override(Mathf.Clamp(lens.intensity.value + 0.5f * Time.deltaTime, -0.8f, 0.8f));
            if (lens.intensity.value > LensTarget)
                lens.intensity.Override(Mathf.Clamp(lens.intensity.value - 0.5f * Time.deltaTime, -0.8f, 0.8f));


            SpawnPlatform();

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                mouseWorldPos = hit.point;

            player.MovePosition(new Vector3(0, transform.position.y, mouseWorldPos.z));

            transform.position += new Vector3(0, playerspeed * Time.deltaTime, 0);

            for (int i = 0; i < spawned.Count; i++)
            {
                if (transform.position.y - spawned[i].transform.position.y > 80)
                {
                    Destroy(spawned[i]);
                    spawned.RemoveAt(i);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.O))
                PlayerPrefs.SetInt("bestscore", 0);

            if (loadBest)
            {
                if ((int)score > PlayerPrefs.GetInt("bestscore"))
                {
                    PlayerPrefs.SetInt("bestscore", (int)score);
                    bestScore.text = "Nouveau meilleur score ! " + (int)score;
                }
                else
                {
                    bestScore.text = "Meilleur score : " + PlayerPrefs.GetInt("bestscore");
                }
                loadBest = false;
            }
        }
    }
    void SpawnPlatform()
    {
        if (Vector3.Distance(player.transform.position, spawned[spawned.Count - 1].transform.position) < 15)
        {
            float random = Random.Range(0.0f, 100.0f);
            GameObject spawnObj = Model_I;
            if (random >= 30 && random < 55)
                spawnObj = Model_IPetit;
            else if (random >= 55 && random < 80)
                spawnObj = Model_D;
            else
                spawnObj = Model_O;


            spawned.Add(Instantiate(spawnObj, spawned[spawned.Count - 1].transform.GetChild(spawned[spawned.Count - 1].transform.childCount - 1).position, Quaternion.identity).gameObject);

            if (spawned[spawned.Count - 1].name == "First_Obj")
                return; //Premier objet à ne pas modifier

            spawned[spawned.Count - 1].transform.localScale = new Vector3(1, 1, Random.Range(0, 2) == 0 ? 1 : -1); //Rotation aléatoire sur lui même pour retourner la platforme

            int indexToKeep = Random.Range(0, spawned[spawned.Count - 1].transform.GetChild(0).childCount); //index du child contenant la config du bloc
            for (int i = 0; i < spawned[spawned.Count - 1].transform.GetChild(0).childCount; i++) //Désactiver tout les child sauf celui à garder
                spawned[spawned.Count - 1].transform.GetChild(0).GetChild(i).gameObject.SetActive(i == indexToKeep);
        }
    }

    void Effets()
    {
        //Timer
        CamRandomRot -= Time.deltaTime;
        if (CamRandomRot <= 0)
        {
            //Event
            CamRandomRot = Random.Range(9.0f, 12.0f);
            CamTargetRot = Random.Range(-60.0f, 60.0f);
        }

        //Timer
        LensTimer -= Time.deltaTime;
        if (LensTimer <= 0)
        {
            //Event
            boxVolume.GetComponent<Volume>().profile.TryGet(out lens);
            LensTarget = lens.intensity.value + Random.Range(0.1f, 0.25f) * (Random.Range(0, 2) == 0 ? 1 : -1);
            LensTimer = Random.Range(10f, 20f);
        }
    }
}
