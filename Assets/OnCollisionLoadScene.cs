using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class OnCollisionLoadScene : MonoBehaviour
{
	public int startingEnvironment = 1;

    void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			Debug.Log("Entered Area Scene");
			Settings.environment = Settings.environment > 0 ? Settings.environment : startingEnvironment;
			SceneManager.LoadScene("EnvironmentScene");
		}
	}
}
