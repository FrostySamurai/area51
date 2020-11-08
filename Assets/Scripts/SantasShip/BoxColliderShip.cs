using UnityEngine;

public class BoxColliderShip : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ship") 
        {
            other.GetComponentInParent<Ship>().CurrentCount++;
            Destroy(gameObject);          
        }
    }

}
