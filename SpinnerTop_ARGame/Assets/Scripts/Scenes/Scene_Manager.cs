using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : Singleton<Scene_Manager>
{
    private string _sceneNameToLoad;
    public void LoadScene(string sceneName)
    {
        _sceneNameToLoad = sceneName;
        StartCoroutine(InitialiseSceneLoading());

    }

    IEnumerator InitialiseSceneLoading()
    {
        //First, we load the laoding scene
        yield return SceneManager.LoadSceneAsync("Scene_Loading");
        //Load the actual scene
        StartCoroutine(LoadActualScene());
    }
    IEnumerator LoadActualScene()
    {
        var asyncSceneLoading = SceneManager.LoadSceneAsync(_sceneNameToLoad);
        //This value stops the scene ffrom displaying when it is still loading
        asyncSceneLoading.allowSceneActivation = false;

        while(!asyncSceneLoading.isDone)
        {
            Debug.Log(asyncSceneLoading.progress);
            if (asyncSceneLoading.progress >= 0.9f)
            {
                //Finally show the scene
                asyncSceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
