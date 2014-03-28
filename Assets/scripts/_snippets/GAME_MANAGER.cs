using UnityEngine;
using System.Collections;

public class GAME_MANAGER : MonoBehaviour {

	public static GameObject PLAYER = null;

	// Use this for initialization
	void Start () {
		if (PLAYER == null) {
			if ((PLAYER = GameObject.Find ("Player")) == null) {
				print ("Couldn't find player!");
			}
		}
	}
}
