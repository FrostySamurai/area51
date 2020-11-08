using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RandomColorLight : MonoBehaviour
{
    private Light2D _light = null;
    private Color[] _colors = new Color[] { Color.blue, Color.red, Color.green };

    private void Awake()
    {
        if (!TryGetComponent(out _light))
            Debug.LogWarning($"{name} is missing light.");
    }

    public void AssignRandomColorToLight()
    {
        _light.color = _colors[Random.Range(0, _colors.Length)];
    }
}

