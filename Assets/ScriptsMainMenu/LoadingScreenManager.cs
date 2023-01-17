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
    
    // private IEnumerator UpdateLoadingTextAsync()
    // {
    //     while (loading)
    //     {
    //         Debug.Log("Update ");
    //         dots++;
    //         dots %= MaxDots;
    //         StartCoroutine(UpdateLoadingText());
    //     }
    //
    //     yield return null;
    // }
    // private IEnumerator UpdateLoadingText()
    // {
    //     var text = new StringBuilder();
    //     text.Append("Loading ");
    //     for (var i = 0; i < dots; i++)
    //     {
    //         text.Append(".");
    //     }
    //
    //     LoadingText.text = text.ToString();
    //     yield return new WaitForSeconds(TickRate);
    // }

    private IEnumerator LoadSceneAsync(AsyncOperation loadingOperation)
    {
        loading = true;
        while (!loadingOperation.isDone)
        {
            yield return null;
        }        
    }

    private void OnEnable()
    {
        Debug.Log("???????????????????????");
        loading = true;
        // lastTimestamp = 0f;
        // dots = 0;
        // StartCoroutine(UpdateLoadingTextAsync());
    }

    private void OnDisable()
    {
        loading = false;
        LoadingScreenPanel.SetActive(false);
    }
}
