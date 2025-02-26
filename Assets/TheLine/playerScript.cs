using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerScript : MonoBehaviour
{
    public GameObject GameOver, DeathSphere;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "red") //Trigger gameover
        {
            GameOver.SetActive(true);
            Instantiate(DeathSphere, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(1);
    }
}
