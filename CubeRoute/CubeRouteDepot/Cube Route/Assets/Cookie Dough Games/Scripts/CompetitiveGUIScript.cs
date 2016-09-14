using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompetitiveGUIScript : MonoBehaviour {

	public Canvas CompetitiveGui;

	public Text BlueScore;
	public Text YellowScore;
	public GameObject BlueWin;
	public GameObject YellowWin;
	public GameObject BlueTurn;
	public GameObject YellowTurn;
	public GameObject NonWinGUI;
	public GameObject WinGUI;

	private GameObject CompetitiveManager;
	private CompetitveManager cm;
	private GameObject SecondaryCamera;

	void OnLevelWasLoaded() {
		while (CompetitiveGui.Equals(null)) {
			print ("making sure nothing is breaking");
		}

		if (NetworkingSceneChanger.IsCompetitiveScene ()) {
			CompetitiveManager = GameObject.Find ("Networking Singletons/Competitive Manager");
			SecondaryCamera = GameObject.FindGameObjectWithTag ("SecondaryCamera");
			CompetitiveGui.transform.SetParent (SecondaryCamera.transform, true);
			CompetitiveGui.gameObject.SetActive (true);

			cm = CompetitiveManager.GetComponent<CompetitveManager> ();
			//print (CompetitiveManager.GetComponent<CompetitveManager>().p1Score);

			NonWinGUI.SetActive (true);
			BlueWin.SetActive (false);
			YellowWin.SetActive (false);
			YellowTurn.SetActive (false);
		} else {
			CompetitiveGui.gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (cm == null)
			return;

		if (!cm.gameOver) {
			BlueScore.text = "" + cm.p1Score;
			YellowScore.text = "" + cm.p2Score;

			if (cm.currentPlayer == 1) {
				BlueTurn.SetActive (true);
				YellowTurn.SetActive (false);
			} else if (cm.currentPlayer == 2) {
				YellowTurn.SetActive (true);
				BlueTurn.SetActive (false);
			}
		} else {
			NonWinGUI.SetActive (false);
			WinGUI.SetActive (true);

			if (cm.winner == 1) {
				BlueWin.SetActive (true);
			} else if (cm.winner == 2) {
				YellowWin.SetActive (true);
			}
		}

	}
}
