using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource = null;

    private void Awake()
    {
        AppData.MusicManager = this;

        if (!TryGetComponent(out _audioSource))
            Debug.LogError($"{name} is missing AudioSource.");

        _audioSource.loop = true;
        if (!_audioSource.playOnAwake)
            _audioSource.Play();
    }
}

