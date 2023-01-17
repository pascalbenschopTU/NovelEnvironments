using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject LoadingScreenPanel;
    [SerializeField] private TextMeshProUGUI LoadingText;

    // [SerializeField][Range(0f,5f)] float TickRate = 1f;
    // [SerializeField][Range(2,10)] int MaxDots = 4;
    // private float lastTimestamp;
    // private int dots;
    private bool loading;
    
    public void LoadScene(string sceneName)
    {
        LoadingScreenPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(SceneManager.LoadSceneAsync(sceneName)));
    }
    
    public void LoadScene(int sceneId)
    {
        LoadingScreenPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(SceneManager.LoadSceneAsync(sceneId)));
    }

    public void LoadSceneWait(string sceneName, float waitFor=2.0f)
    {
        LoadingScreenPanel.SetActive(true);
        StartCoroutine(LoadSceneAsyncWait(sceneName, waitFor));
    }
    private IEnumerator LoadSceneAsync(AsyncOperation loadingOperation, float waitFor=0.0f)
    {
        loading = true;

        while (!loadingOperation.isDone)
        {
            yield return null;
        }        
    }
    
    private IEnumerator LoadSceneAsyncWait(string sceneName, float waitFor=0.0f)
    {
        loading = true;

        if (waitFor > 0.0f)
        {
            yield return new WaitForSeconds(waitFor);
        }

        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadingOperation.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }        
    }

    private void OnEnable()
    {
        loading = true;
    }

    private void OnDisable()
    {
        loading = false;
        LoadingScreenPanel.SetActive(false);
    }
}
