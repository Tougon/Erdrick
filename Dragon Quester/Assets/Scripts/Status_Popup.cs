using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status_Popup : MonoBehaviour
{

    public Image me, child;
    Sprite defaultSprite;
    public List<Sprite> mySprites;
    int iterator;

    private void Awake()
    {
        defaultSprite = child.sprite;
        mySprites = new List<Sprite>();
        iterator = 0;
    }

    public void AddEffectToList(Effect e)
    {
        if (e.eff1 != null)
        {
            mySprites.Add(e.eff1);
            mySprites.Add(e.eff2);
            ShowEffectPopup();
        }
    }

    public void ShowEffectPopup()
    {
        me.enabled = true;
        child.enabled = true;
        if (mySprites.Count <= 2)
        {
            StartCoroutine(CycleEffects());
        }
    }

    void HideEffectPopup()
    {
        StopCoroutine(CycleEffects());
        me.enabled = false;
        child.enabled = false;
        mySprites.Clear();
        me.sprite = defaultSprite;
        child.sprite = defaultSprite;
    }

    public void RemoveEffectFromList(Effect e)
    {
        Debug.Log("remove");
        if (e.eff1 != null && mySprites.Contains(e.eff1)) 
        {
            mySprites.Remove(e.eff1);
            mySprites.Remove(e.eff2);
            if(mySprites.Count <= 1)
            {
                HideEffectPopup();
            }

        }
    }

    IEnumerator CycleEffects()
    {
        if (mySprites.Count >= 2)
        {
            me.sprite = mySprites[iterator];
            yield return new WaitForSeconds(0.5f);
            iterator++;
            if (iterator >= mySprites.Count)
            {
                iterator = 0;
            }
            if (mySprites.Count >= 2)
            {
                StartCoroutine(CycleEffects());
            }
            else
            {
                HideEffectPopup();
            }
        }
        else
        {
            HideEffectPopup();
        }
    }
}
