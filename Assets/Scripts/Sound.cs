using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [HideInInspector]
    public AudioSource audioSource;
    public string name;
    [Range(0, 1)]
    public float volume;
    [Range(0, 2)]
    public float pitch;
    public bool loop;


}
