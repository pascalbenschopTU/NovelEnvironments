using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class OnCollisionLoadScene : MonoBehaviour
{
	[SerializeField] private string SceneToLoad = "EnvironmentScene";
	[SerializeField] private LoadingScreenManager LoadingScreenManager;
	
    void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			Debug.Log("Entered Area Scene");
			LoadingScreenManager.LoadScene(SceneToLoad);
		}
	}
}
