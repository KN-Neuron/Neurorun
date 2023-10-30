using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip menuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip level4Music;





    private Dictionary<string, AudioClip> musicDictionary;
    private AudioSource audioSource;

    private static MusicPlayer instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            musicDictionary = new Dictionary<string, AudioClip>
        {
            { "Menu", menuMusic },
            {"Levels", menuMusic },
            { "Level1", level1Music },
            { "Level2", level2Music },
            { "Level3", level3Music },
            { "Level4", level4Music }
        };

            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicDictionary.TryGetValue(scene.name, out AudioClip clip))
        {
            if (audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
