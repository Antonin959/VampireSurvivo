using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Obsolete]
public class buttonsUi : MonoBehaviour
{
    public GameObject pausePannel;

    public void OnRestart()
    {
        SceneManager.LoadScene(0);
    }
    public void OnPause()
    {
        pausePannel.SetActive(!pausePannel.active);
    }
}
