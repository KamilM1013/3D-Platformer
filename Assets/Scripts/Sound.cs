using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public string _name;

    public AudioClip _clip;

    [Range(0f, 3f)]
    public float _volume;

    [Range(0f, 2f)]
    public float _pitch;

    public bool _loop;

    [HideInInspector]
    public AudioSource _source;
}
