using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete]
public class BestScore : MonoBehaviour
{
    TMP_Text txt;
    generation genScript;

    public TMP_InputField textBox;

    List<Tuple<int, string>> scores = new List<Tuple<int, string>>();

    bool newBest = false;
    int lastBest = 0;

    void Start()
    {
        genScript = GameObject.Find("Main Camera").GetComponent<generation>();
        txt = GetComponent<TMP_Text>();

        newBest = genScript.score > PlayerPrefs.GetInt("bestscore" + 0);

        for (int i = 0; i < 10; i++)
        {
            scores.Add(Tuple.Create(PlayerPrefs.GetInt("bestscore" + i), PlayerPrefs.GetString("name" + i)));
        }
        lastBest = scores[0].Item1;
        if (genScript.score > lastBest)
        {
            scores.Add(Tuple.Create(genScript.score, ""));
            scores.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            scores.RemoveAt(10);
        }
        WriteText();
    }
    void WriteText()
    {
        txt.text = "";
        for (int i = 0; i < 10; i++)
        {
            txt.text += eme(i + 1) + "  ";
            if (scores[i].Item1 > 0)
            {
                if (scores[i].Item2 == "")
                    txt.text += "- ";
                else
                    txt.text += PlayerPrefs.GetString("name" + i) + " ";
                
                txt.text += scores[i].Item1 + "pt\n";
            }
            else
                txt.text += "--" + "\n";


            PlayerPrefs.SetInt("bestscore" + i, scores[i].Item1);
            PlayerPrefs.SetString("name" + i, scores[i].Item2);
        }
        newBest = true;
    }

    void Update()
    {
        if (genScript.score > lastBest && newBest)
        {
            textBox.gameObject.SetActive(true);

            PlayerPrefs.SetString("name" + 0, textBox.text);
            scores[0] = Tuple.Create(scores[0].Item1, textBox.text);

            WriteText();
        }
    }

    string eme(int num)
    {
        if (num == 1)
            return "1er";
        else
            return num + "e";
    }
}
