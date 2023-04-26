using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class OnCollisionLoadScene : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			GameObject.Find("StartGame").GetComponent<StartGame>().TeleportPlayerToEnvironment();
		}
	}
}
