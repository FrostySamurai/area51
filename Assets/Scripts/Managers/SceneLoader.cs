using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        AppData.SceneLoader = this;
    }

    public void LoadScene(SceneName scene)
    {
        StartCoroutine(LoadSceneAsync(scene));
    }

    private IEnumerator LoadSceneAsync(SceneName scene)
    {
        AsyncOperation loader = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Single);

        while (!loader.isDone)
            yield return null;

        // MH: if we need to do something after scene load or some such
    }
}

