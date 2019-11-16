using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Randomizes a song from the song list.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RandomizeMusic : MonoBehaviour
{
    public SongList songList;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        Song song = songList.GetRandomSong();
        source.clip = song.song;
        source.Play();

        Debug.Log("Now Playing: " + song.name);
    }
}
