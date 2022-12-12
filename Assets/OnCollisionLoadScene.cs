using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class OnCollisionLoadScene : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			Debug.Log("Entered Area Scene");
			SceneManager.LoadScene("SampleScene");

			Settings.environment = 3;
		}
	}
}
