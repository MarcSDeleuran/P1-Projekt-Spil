using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource soundSource;
    public GameObject heartbeat;
    public void PlayAudio(AudioClip music, AudioClip sound)
    {
        if (sound != null)
        {
            soundSource.clip = sound;
            soundSource.Play();
        }
        if (music != null && musicSource.clip != music)
        {
            StartCoroutine(SwitchMusic(music));
        }
        if (music == null)
        {
            musicSource.volume = 0.5f;
        }
    }

    private IEnumerator SwitchMusic(AudioClip music)
    {
        if (musicSource.clip != null)
        {
            while (musicSource.volume > 0)
            {
                musicSource.volume -= 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            musicSource.volume = 0;
        }

        musicSource.clip = music;
        musicSource.Play();
        while (musicSource.volume < 0.5f)
        {
            musicSource.volume += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Update(){
        if (musicSource.pitch != 1f - GameManager.Instance.StressAmount/400f){
            musicSource.pitch = 1f - GameManager.Instance.StressAmount/400f;
        }
        if (GameManager.Instance.StressAmount >= 160 && !heartbeat.activeSelf){
            heartbeat.SetActive(true);
        } else if (GameManager.Instance.StressAmount < 160 && heartbeat.activeSelf){
            heartbeat.SetActive(false);
        }
    }
}
