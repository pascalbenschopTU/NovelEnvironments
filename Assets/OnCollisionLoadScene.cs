using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class OnCollisionLoadScene : MonoBehaviour
{
    public UnityEvent eventz;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onCollisionEnter(Collision collision)
    {
        Debug.Log("On Collision");
    }

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			eventz.Invoke();
			Debug.Log("Entered Area Scene");
            SceneManager.LoadSceneAsync ("SampleScene");

		}
	}

    // void OnTriggerEnter(Collider collider)
    // {
    //     Debug.Log("Colided");
    //     if (collider.gameObject.tag == "Player")
    //     {
    //         SceneManager.LoadScene ("SampleScene");
    //         //Or:
    //         //SceneManager.LoadScene (SceneIndex); //(without these: ", because it's a number - an int, not a string)
    //     }
    // }
}
