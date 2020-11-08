using System;
using UnityEngine;


public class PlayerCollider : MonoBehaviour
{
    public bool trigger = false;

    private void Awake()
    {
        AppData.MessageDisplayer.OnMessagesShown += HandleMessagesShown;
        gameObject.SetActive(false);
    }

    private void HandleMessagesShown()
    {
        gameObject.SetActive(true);
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
