using System;
using UnityEngine;

/**
 * GravityState stores the current state of the gravity vector and stores
 * the vectors that are currently to the players front, back, left and right
 **/

public class GravityState
{
    /* The different directions the player can rotate. */

    public enum Direction { Left, Right, Forward, Back };

    /* Current gravity vector */
    private Vector3 currentVec;

    /* Vector that points to the player's front */
    private Vector3 forwardVec;

    /* Vector that points to the player's backside */
    private Vector3 backVec;

    /* Vector that points to the left of the player */
    private Vector3 leftVec;

    /* Vector that points to the left of the player */
    private Vector3 rightVec;

    /*
     * GravityState constructor
     * Sets the current gravity vector equal to the vector points in the -y direction
     */

    public GravityState()
    {
        currentVec = Vector3.down;
        forwardVec = Vector3.forward;
        backVec = Vector3.back;
        leftVec = Vector3.left;
        rightVec = Vector3.right;
    }

    /*
     * When the player wants to rotate, change the gravity vector depending on which way they want to rotate
     */

    // TODO Constrain the blocks from moving in other axes with gravity flips
    //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

    public void rotate( Direction direction )
    {
        if ( direction == Direction.Forward )
        {
            forwardVec = currentVec;
            Vector3 temp = currentVec * -1;
            currentVec = backVec;
            backVec = temp;
        }
        else if ( direction == Direction.Left )
        {
            leftVec = currentVec;
            Vector3 temp = currentVec * -1;
            currentVec = rightVec;
            rightVec = temp;
        }
        else if ( direction == Direction.Back )
        {
            backVec = currentVec;
            Vector3 temp = currentVec * -1;
            currentVec = forwardVec;
            forwardVec = temp;
        }
        else if ( direction == Direction.Right )
        {
            rightVec = currentVec;
            Vector3 temp = currentVec * -1;
            currentVec = leftVec;
            leftVec = temp;
        }
        else
        {
            // Do nothing
        }
    }

	/* Returns current gravity vector */
	public Vector3 getCurrentVec()
	{
		return currentVec;
	}

	/* Assigns current gravity vector */
	public void setCurrentVec(Vector3 newGrav)
	{
		currentVec = newGrav;
	}
}