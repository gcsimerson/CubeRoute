using UnityEngine;
using System.Collections;
using WiimoteApi;

public class WiiControllerInfo : MonoBehaviour
{
	// Wiimote interface.
	private Wiimote wiimote;

	// Wiimote data
	private WiimoteData wiimoteData = new WiimoteData();

	private bool setLight = true;
	private bool pingNunchuck = true;
	private bool pingWMP = true;

	public WiimoteData GetWiimoteData()
	{
		if (wiimote != null)
		{
			return wiimoteData;
		}

		return null;
	}

	void Start()
	{
		WiimoteManager.FindWiimotes();
	}

	void Update()
	{
		if (WiimoteManager.Wiimotes.Count > 0)
		{
			wiimote = WiimoteManager.Wiimotes[0];

			// Set P1 light.
			if (setLight)
			{
				wiimote.SendPlayerLED(true, false, false, false);
				setLight = false;
			}

			// Wake up the nunchuck if we need to.
			if (pingNunchuck)
			{
				// EXT16 = original nunchuck.
				wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
				pingNunchuck = false;
			}

			// Wake up motion plus if we need to.
			if (pingWMP)
			{
				wiimote.RequestIdentifyWiiMotionPlus();

				if (wiimote.wmp_attached)
				{
					wiimote.ActivateWiiMotionPlus();
					pingWMP = false;
				}

			}

			int ret;
			do
			{
				ret = wiimote.ReadWiimoteData();
			}
			while (ret > 0);

			UpdateInputs();
		}
		else
		{
			wiimote = null;
		}
	}

	private void UpdateInputs()
	{
		if (wiimote != null)
		{
			wiimoteData.UpdateButton(WiimoteData.Buttons.A, wiimote.Button.a);
			wiimoteData.UpdateButton(WiimoteData.Buttons.B, wiimote.Button.b);
			wiimoteData.UpdateButton(WiimoteData.Buttons.Left, wiimote.Button.d_left);
			wiimoteData.UpdateButton(WiimoteData.Buttons.Right, wiimote.Button.d_right);
			wiimoteData.UpdateButton(WiimoteData.Buttons.Home, wiimote.Button.home);

			if (wiimote.Nunchuck != null)
			{
				wiimoteData.UpdateHorizontalAxis(wiimote.Nunchuck.stick[0]);
				wiimoteData.UpdateVerticalAxis(wiimote.Nunchuck.stick[1]);
			}

			if (wiimote.MotionPlus != null)
			{
				wiimoteData.UpdateYaw(wiimote.MotionPlus.YawSpeed);
				wiimoteData.UpdatePitch(wiimote.MotionPlus.PitchSpeed);
				wiimoteData.UpdateRoll(wiimote.MotionPlus.RollSpeed);
			}

			wiimoteData.UpdateHeading(wiimote.Accel.GetCalibratedAccelData());
		}
	}

	void OnDestroy()
	{
		WiimoteManager.Cleanup(wiimote);
	}
}
