using System.Collections;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public sounds[] sounds;

    public static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (sounds s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void play(string name)
    {

        sounds s =  Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound : "+name+" Not found");
            return;
        }
        s.source.PlayOneShot(s.clip);
    }
    public void playOnce(string name)
    {

        sounds s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound : " + name + " Not found");
            return;
        }
        s.source.Play();
    }
}
