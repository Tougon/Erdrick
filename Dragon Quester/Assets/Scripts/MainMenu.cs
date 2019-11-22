using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject credits;
    bool areCreditsOpen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl) && !areCreditsOpen)
        {
            credits.SetActive(true);
            areCreditsOpen = true;
        }

        else if (Input.anyKeyDown)
        {
            if (areCreditsOpen)
            {
                credits.SetActive(false);
                areCreditsOpen = false;
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
