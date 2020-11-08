using System;
using UnityEngine;


public class PlayerCollider : MonoBehaviour
{
    public bool trigger = false;

    private void Awake()
    {
        AppData.MessageDisplayer.ObjectToActivate = gameObject;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            AppData.GameManager.ToggleControllers(false);
            trigger = true;
        }
    }

}
