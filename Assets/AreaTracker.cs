using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaTracker : MonoBehaviour
{
	
	public UnityEvent eventz;
	bool done = false;
	public string target = "Player";
	public bool once = true;
	public bool area = false;
	public UnityEvent leaveEventz;
	public bool destroy = false;
    public LookDirection lookDirection;
	
    // Start is called before the first frame update
    void Start()
    {
        lookDirection = GameObject.Find("Player").GetComponent<LookDirection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void SetDone(bool done) {
		this.done = done;
	}
	
	void OnTriggerEnter(Collider other) {
		if(!done && other.gameObject.CompareTag(target)) {
			eventz.Invoke();
			if(once) {
				done = true;
			}
			Debug.Log("Entered Area");
            lookDirection.setTarget(gameObject);
			if(destroy) Destroy(other.gameObject);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(area && other.gameObject.CompareTag(target)) {
			leaveEventz.Invoke();
            Debug.Log("Exit Area");
            lookDirection.setTarget(null);
		}
	}
}