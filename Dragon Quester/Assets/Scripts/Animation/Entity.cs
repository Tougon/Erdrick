using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }


    public void SetAnimation(string val)
    {
        anim.SetTrigger(val);
    }


    public void SetColorTween(Color c, float duration)
    {
        sprite.DOColor(c, duration);
    }


    public SpriteRenderer GetSpriteRenderer()
    {
        return sprite;
    }
}
