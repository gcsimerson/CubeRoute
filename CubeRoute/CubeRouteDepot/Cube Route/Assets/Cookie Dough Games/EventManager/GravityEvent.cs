using System.Collections;
using UnityEngine;

public class GravityEvent
{
    private Vector3 gravityVec;
	private int Player;

	public GravityEvent( Vector3 gravityVec, int player )
    {
        this.gravityVec = gravityVec;
		this.Player = player;
    }

    public Vector3 GetGravityVec()
    {
        //        Debug.Log(gravityVec);
        return gravityVec;
    }

	public int GetPlayer() {
		return Player;
	}
}