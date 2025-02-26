using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuUI : MonoBehaviour
{
    public void ButtonStart(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void ButtonLeft()
    {
        Application.Quit();
    }
}
