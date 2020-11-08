using UnityEngine;


public class PlayerCollider : MonoBehaviour
{
    public bool trigger = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            trigger = true;
        }
    }

}
