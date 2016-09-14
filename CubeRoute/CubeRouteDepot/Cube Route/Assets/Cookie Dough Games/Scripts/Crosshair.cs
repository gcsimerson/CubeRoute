using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class Crosshair : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if(!VRDevice.isPresent) {
            gameObject.SetActive(false);
        }
            
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
