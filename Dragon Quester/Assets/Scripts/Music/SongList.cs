using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSongList", menuName = "Song List", order = 2)]
public class SongList : ScriptableObject
{
    public List<Song> songs = new List<Song>();


    public Song GetRandomSong()
    {
        return songs[Random.Range(0, songs.Count)];
    }
}

[System.Serializable]
public class Song
{
    public string name;
    public AudioClip song;
}
