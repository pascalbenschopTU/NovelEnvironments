using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookDirection : MonoBehaviour
{

    public GameObject playerCamera;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetMouseButtonDown(0)) {
        CheckDirection(target, playerCamera);
        // }
    }

    bool inArea = false;

    public void CheckDirection(GameObject _target, GameObject camera) {
        if (_target== false) {
            Debug.Log("Not close to any Landmark");
            return;
        }

        Vector3 directionToMe = _target.transform.position - camera.transform.position;
        if (Vector3.Angle(camera.transform.forward, directionToMe) < 45.0f)
        {
            Debug.Log("In view!");
        // Whatever you wanna do
        }
        else {
            Debug.Log("Not looking at the Lighthouse");
        }

    }

    public void setTarget(GameObject tar) {
        target = tar;
    }

    public void SetInArea(bool _inArea) {
        inArea = _inArea;
    }
}
