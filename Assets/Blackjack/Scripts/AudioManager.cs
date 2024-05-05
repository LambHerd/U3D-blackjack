using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private bool _isAudioDisabled;

    [SerializeField]
    private AudioClip _buttonClip;
    [SerializeField]
    private AudioClip _cardClip;

    [SerializeField]
    private AudioClip[] HumanWon;

    [SerializeField]
    private AudioClip[] GreedyCard;

    [SerializeField]
    private AudioClip[] DemonWon;

    [SerializeField]
    private AudioClip[] HumanDie;

    [SerializeField]
    private AudioClip[] DemonDie;

    [SerializeField]
    private AudioClip[] nokillstep1;

    [SerializeField]
    private AudioClip[] nokillstep2;

    [SerializeField]
    private AudioClip[] killstep1;

    [SerializeField]
    private AudioClip[] killstep2;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        var uiManager = FindObjectOfType<UIManager>();
        uiManager.OnAudioButtonEvent += SwitchAudioEvent;
    }

    public bool SwitchAudioEvent()
    {
        _isAudioDisabled = !_isAudioDisabled;

        return _isAudioDisabled;
    }

    public void PlayButtonClip()
    {
        PlayClip(_buttonClip);
    }
    
    public void PlayCardClip()
    {
        PlayClip(_cardClip);
    }

    public void PlayDialogue(string state)
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
        if (state == "HumanWon")
        {
            StartCoroutine(PlayClips(HumanWon));
        }
        else if (state == "ComputerWon")
        {
            StartCoroutine(PlayClips(DemonWon));
        }

        else if (state == "GreedyCardActive")
        {
            StartCoroutine(PlayClips(GreedyCard));
        }
        else if (state == "humandie")
        {
            StartCoroutine(PlayClips(HumanDie));
        }
        else if (state == "demondie")
        {
            StartCoroutine(PlayClips(DemonDie));
        }
        else if (state == "nokillstep1")
        {
            StartCoroutine(PlayClips(nokillstep1));
        }
        else if (state == "nokillstep2")
        {
            StopCoroutine(PlayClips(nokillstep1));
            Debug.Log("停止上一轮的携程");
            StartCoroutine(PlayClips(nokillstep2));
        }
        else if (state == "killstep1")
        {
            StartCoroutine(PlayClips(killstep1));
        }
        else if (state == "killstep2")
        {
            StopCoroutine(PlayClips(killstep1));
            Debug.Log("停止上一轮的携程");
            StartCoroutine(PlayClips(killstep2));
        }

    }
    IEnumerator PlayClips(AudioClip[] clip)
    {
        if(_audioSource.clip != clip[0] && _audioSource.clip!= clip[1])
        {
            Debug.Log("清空上一次的clip");
            _audioSource.Stop();
        }
        Debug.Log("in co");
        _audioSource.clip = clip[0];
        _audioSource.Play();
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        _audioSource.clip = clip[1];
        _audioSource.Play();




    } 
    private void PlayClip(AudioClip clip)
    {
        if (_isAudioDisabled) {
            return;
        }

        _audioSource.PlayOneShot(clip);
    }
}