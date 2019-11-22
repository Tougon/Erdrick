using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    bool P1Ready, P2Ready, P2Alt;
    public bool takeInputs;
    [SerializeField] Image P2RegImage, P2RegAtk, P2RegBlk, P2AltImage, P2AltAtk, P2AltBlk, P1ReadyImg, P2ReadyImg;
    [SerializeField] Sprite readySprite;
    [SerializeField] FightController FC;
    Color dullColor, fullColor;

    private void Awake()
    {
        dullColor = new Color(0.6f, 0.6f, 0.6f);
        fullColor = new Color(1.0f, 1.0f, 1.0f);
        P1Ready = false;
        P2Ready = false;
        P2Alt = false;
        //takeInputs = false;
    }

    public void TakeInputs(bool should)
    {
        takeInputs = should;
    }

    void Update()
    {
        if (takeInputs)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !P1Ready)
            {
                P1Ready = true;
                P1ReadyImg.sprite = readySprite;
                SoundManager.Instance.PlaySound("Sounds/move_select");
                CheckReady();
            }

            if (Input.GetKeyDown(KeyCode.U) && !P2Ready)
            {
                if (P2Alt)
                {
                    P2Alt = false;
                    P2AltImage.color = dullColor;
                    P2AltAtk.color = dullColor;
                    P2AltBlk.color = dullColor;
                    P2RegImage.color = fullColor;
                    P2RegAtk.color = fullColor;
                    P2RegBlk.color = fullColor;
                }
                else
                {
                    P2Ready = true;
                    P2ReadyImg.sprite = readySprite;
                    SoundManager.Instance.PlaySound("Sounds/move_select");
                    CheckReady();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                if (P2Alt)
                {
                    P2Ready = true;
                    P2ReadyImg.sprite = readySprite;
                    SoundManager.Instance.PlaySound("Sounds/move_select");
                    CheckReady();
                }
                else
                {
                    Debug.Log("change color");
                    P2RegImage.color = dullColor;
                    P2RegAtk.color = dullColor;
                    P2RegBlk.color = dullColor;
                    P2AltImage.color = fullColor;
                    P2AltAtk.color = fullColor;
                    P2AltBlk.color = fullColor;
                    P2Alt = true;
                    
                }
            }
        }
    }

    void CheckReady()
    {
        if(P1Ready && P2Ready)
        {
            StartCoroutine(SendReady());
        }
    }

    IEnumerator SendReady()
    {
        takeInputs = false;
        yield return new WaitForSeconds(1.0f);
        FC.BeginBattle(P2Alt);
    }
}
