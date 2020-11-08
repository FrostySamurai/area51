using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ship : MonoBehaviour
{
    public GameObject PresentInstantiator;
    public List<GameObject> Presents;
    public PlayerCollider PlayerCollider;
    public int Count = 5;
    public int CurrentCount = 0;
    private Animator _animator;
    protected readonly int h_startAnim = Animator.StringToHash("StartAnim");
    private bool invoke = false;

    private void Start()
    {
        
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!invoke && PlayerCollider.trigger == true) 
        {
            invoke = true;
            StartCoroutine(StartSpawn());
        }

        if (Count == CurrentCount) 
        {
            _animator.SetBool(h_startAnim, true);
        }
    }

    public IEnumerator StartSpawn() 
    {
        int counter = 0;
        while (counter <= Count - 1)
        {
            yield return new WaitForSeconds(1);
            Vector3 position = new Vector3(PresentInstantiator.transform.position.x, PresentInstantiator.transform.position.y,0f);
            Instantiate(Presents[Random.Range(0, Presents.Count - 1)], position, Quaternion.identity);
            counter++;
        }
    }
}

