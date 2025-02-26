using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete]
public class generation : MonoBehaviour
{
    TMP_Text scoreText;

    List<GameObject> Tuyaux = new List<GameObject>();
    List<bool> OnScore = new List<bool>();
    public GameObject tuyau_prefab, GameOverPannel, bigfloppy;

    float spawntimer = 0.0f;
    const float mov_speed = 5.0f;

    public int score = 0;

    void Start()
    {
        scoreText = GameObject.Find("scoreText").GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!GameOverPannel.active)
            Generation();

        if (Input.GetKeyDown(KeyCode.P))
        {
            for (int i = 0; i < 10; i++)
            {
                PlayerPrefs.SetInt("bestscore" + i, 0);
                PlayerPrefs.SetString("scores" + i, "");
            }
        }
    }
    void Generation()
    {
        spawntimer -= Time.deltaTime;
        if (spawntimer <= 0.0f)
        {           
            GameObject newTuyau = Instantiate(tuyau_prefab, new Vector3(15.0f, 1.5f + Random.Range(-1.5f, 3.0f), 0), Quaternion.identity);
            newTuyau.transform.GetChild(0).transform.localPosition = new Vector3(0, 4.7f + Random.Range(-2.5f, 1.5f), 0);
            
            Tuyaux.Add(newTuyau);
            OnScore.Add(true);
            spawntimer = Random.Range(1.5f, 2.5f);
        }


        for (int i = 0; i < Tuyaux.Count; i++)
        {
            Tuyaux[i].transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * mov_speed);
            if (Tuyaux[i].transform.position.x < -15.0f)
            {
                Destroy(Tuyaux[i]);
                OnScore.RemoveAt(i);
                Tuyaux.RemoveAt(i);
            }

            if (OnScore[i] && Tuyaux[i].transform.position.x <= 0)
            {
                score++;
                scoreText.text = score + " pt";
                OnScore[i] = false;
            }
        }
    }
}
