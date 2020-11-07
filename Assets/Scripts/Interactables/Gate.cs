using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Gate : MonoBehaviour
{
    [SerializeField] private SceneName scene;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            Debug.Log(collision.gameObject.layer);
            if (AppData.GameManager.LevelIsDone && AppData.InputController.VerticalMovement > 0)
            {
                AppData.SceneLoader.LoadScene(scene);
            }
        }
    }
}
