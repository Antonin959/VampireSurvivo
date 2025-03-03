using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomMoney : MonoBehaviour
{
    readonly Vector3 ScreenTarget = new Vector3(Screen.width * 0.9f, Screen.height * 0.9f);

    void Start()
    {
        Color color = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, Random.Range(0.7f, 0.85f));
    }

    void Update()
    {
        Debug.Log(transform.position);
        transform.position += (ScreenTarget - transform.position).normalized * 5;

        if (Vector2.Distance(ScreenTarget, transform.position) < 10)
        {
            playerrscript.TakeGold(2);
            Destroy(gameObject);
        }
    }
}
