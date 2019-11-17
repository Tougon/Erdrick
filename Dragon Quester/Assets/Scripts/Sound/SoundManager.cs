using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private AudioSource[] sources;

    // Awake is called before the first frame update
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        sources = GetComponentsInChildren<AudioSource>();
    }


    public void PlaySound(string path)
    {
        int index = -1;

        for(int i=0; i<sources.Length; i++)
        {
            if (!sources[i].isPlaying)
            {
                index = i;
                break;
            }
        }

        AudioSource target;

        if (index == -1)
            target = sources[0];
        else
            target = sources[index];

        AudioClip clip = Resources.Load(path, typeof(AudioClip)) as AudioClip;

        if(clip != null)
        {
            Debug.Log("DD");
            target.clip = clip;
            target.Play();
        }
        else
        {
            Debug.Log("FF");
        }
    }
}
