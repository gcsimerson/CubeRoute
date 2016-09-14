using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WiimoteData
{
	public enum Buttons
	{
		A,
		B,
		Left,
		Right,
		Home
	}

	private Dictionary<Buttons, bool> m_buttonsLastFrame = new Dictionary<Buttons, bool>();
	private Dictionary<Buttons, bool> m_buttonsThisFrame = new Dictionary<Buttons, bool>();

	private float horizontalAxis;
	private float verticalAxis;

	// Accelerometer
	private Vector3 heading;

	// Motion Plus
	private float yaw;
	private float pitch;
	private float roll;

	public WiimoteData()
	{
		m_buttonsLastFrame.Add(Buttons.A, false);
		m_buttonsLastFrame.Add(Buttons.B, false);
		m_buttonsLastFrame.Add(Buttons.Left, false);
		m_buttonsLastFrame.Add(Buttons.Right, false);
		m_buttonsLastFrame.Add(Buttons.Home, false);

		m_buttonsThisFrame.Add(Buttons.A, false);
		m_buttonsThisFrame.Add(Buttons.B, false);
		m_buttonsThisFrame.Add(Buttons.Left, false);
		m_buttonsThisFrame.Add(Buttons.Right, false);
		m_buttonsThisFrame.Add(Buttons.Home, false);
	}

	#region Input Updates

	public void UpdateButton(Buttons button, bool value)
	{
		m_buttonsLastFrame[button] = m_buttonsThisFrame[button];
		m_buttonsThisFrame[button] = value;
	}

	public void UpdateHorizontalAxis(float value)
	{
		// TODO: Roughly 100 - 130... May depend on nunchuck, calibration, etc?
		if (value < 100)
		{
			horizontalAxis = -1;
		}
		else if (value > 130)
		{
			horizontalAxis = 1;
		}
		else
		{
			horizontalAxis = 0;
		}
	}

	public void UpdateVerticalAxis(float value)
	{
		// TODO: Roughly 115 - 145... May depend on nunchuck, calibration, etc?
		if (value < 115)
		{
			verticalAxis = -1;
		}
		else if (value > 145)
		{
			verticalAxis = 1;
		}
		else
		{
			verticalAxis = 0;
		}
	}

	public void UpdateHeading(float[] values)
	{
		// {x, z, y} ... :(
		heading = (new Vector3(values[0], values[2], values[1])).normalized;
	}

	public void UpdateYaw(float value)
	{
		yaw = value;
	}

	public void UpdatePitch(float value)
	{
		pitch = value;
	}

	public void UpdateRoll(float value)
	{
		roll = value;
	}

	#endregion

	#region Getters

	public float GetAxisHorizontal()
	{
		return horizontalAxis;
	}

	public float GetAxisVertical()
	{
		return verticalAxis;
	}

	public bool GetButton(Buttons button)
	{
		return m_buttonsThisFrame[button];
	}

	public bool GetButtonDown(Buttons button)
	{
		return (m_buttonsLastFrame[button] == false && m_buttonsThisFrame[button] == true);
	}

	public bool GetButtonUp(Buttons button)
	{
		return (m_buttonsLastFrame[button] == true && m_buttonsThisFrame[button] == false);
	}

	public Vector3 GetHeading()
	{
		return heading;
	}

	public float GetYaw()
	{
		return yaw;
	}

	public float GetPitch()
	{
		return pitch;
	}

	public float GetRoll()
	{
		return roll;
	}

	#endregion
}
