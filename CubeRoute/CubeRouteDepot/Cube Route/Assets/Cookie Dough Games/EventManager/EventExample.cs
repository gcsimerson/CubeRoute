using UnityEngine;
using System.Collections;

public class EventExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventManager.Dance += DoDance;
	}

    void Update()
    {
        if (Input.GetButtonDown("Activate"))
        {
            EventManager.Raise(new EventObject("Do a little dance"));
        }
    }

    void OnDisable()
    {
        EventManager.Dance -= DoDance;
    }

    public void DoDance(EventObject e)
    {
        if (e.GetEventType().Equals("Do a little dance"))
        {
            print("Do a little dance, make a little love");
        }
    }
}
