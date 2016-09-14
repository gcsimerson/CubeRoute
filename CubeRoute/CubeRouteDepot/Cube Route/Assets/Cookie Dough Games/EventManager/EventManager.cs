using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

	// example code used with EventExample.cs and the public raise method within
	// this script
	// I HIGHLY RECOMMEND CHANGING THIS IF YOU WANT TO HAVE A DELEGATE THAT
	// USES THE EVENTOBJECT CLASS
    public delegate void ExampleEvent(EventObject e);
    public static event ExampleEvent Dance;

	// Currently used delegates for death events and gravity
    public delegate void GravDelegate(GravityEvent e);
    public static event GravDelegate Grav;
	public delegate void ChangeTurnDelegate(int currentPlayer);
	public static event ChangeTurnDelegate Turn;
	public delegate void DeathEvent(int player);
	public static event DeathEvent Death;

	// the temp/basic method to raise an EventObject over the Example Event
	// delegate. 
	// I HIGHLY RECOMMEND CHANGING THIS IF YOU WANT TO HAVE A DELEGATE THAT
	// USES THE EVENTOBJECT CLASS
    public static void Raise(EventObject e)
    {
        if (e.GetEventType().Equals("Do a little dance"))
        {
            if (Dance != null)
            {
                Dance(e);
            }
        }
    }

	// Methods to handle the basic calling of the delegates when another method
	// wishes to raise an event.
	public static void RotateGravity(GravityEvent e)
    {
        if (Grav != null)
        {
            Grav(e);
        }
    }

	public static void RaiseTurnEvent(int currentPlayer) {
		if (Turn != null) {
			Turn (currentPlayer);
		}
	}

	public static void RaiseDeathEvent(int player) {
		if (Death != null) {
			Death (player);
		}
	}
}
