using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject credits;
    bool cred;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            credits.SetActive(!credits.activeSelf);
            cred = credits.activeSelf;
        }

        else if (Input.anyKeyDown)
        {
            if (!cred)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
