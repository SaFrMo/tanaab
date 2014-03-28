using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Conversation : MonoBehaviour {

	// THE ENORMOUS AND TERRIFYING CONVERSATION-HAVER
	// ================================================
	// But it works great!

	// USE: Create a class based on GenericConversationTemplate, which is a child of this class. Follow instructions from there.
	// NOTE: You shouldn't need to attach <CONVERSATION>, this script, anywhere.
	

	// <CONVERSATION AVAILABLE> MODULE
	// ================================

	
	// icon to display
	public Texture2D conversationIcon;
	
	// so we can hide the icon when the conversation's displayed
	public bool showIcon { get; private set; }
	
	// bouncing variables
	public float distanceAboveNPC = 10f;
	public float bouncingSmooth = 6.5f;
	public float bounceHeight = 10f;
	
	// dummy style to clear the default Unity style (ie, no visible background, in this case)
	GUIStyle style = new GUIStyle();
	
	// shows the "use key" icon above the NPC if a conversation is available
	void ShowConversationAvailable () {
		if (isNearPlayer && showIcon) {
			GUI.Box (new Rect (Camera.main.WorldToScreenPoint(transform.position).x - renderer.bounds.extents.x,
			                   Screen.height - Camera.main.WorldToScreenPoint (transform.position).y + renderer.bounds.extents.y + distanceAboveNPC,
			                   conversationIcon.width,
			                   conversationIcon.height), conversationIcon, style);
		}
	}
	
	// animates the bouncing for the icon
	bool rising = true;
	
	void CalculateDistanceAboveNPC () {
		if (rising) {
			distanceAboveNPC = Mathf.SmoothStep (distanceAboveNPC, bounceHeight, Time.deltaTime * bouncingSmooth);
			// the 0.2f here and below help the bouncing motion avoid the Unity long, slow end-of-lerp problem
			if (distanceAboveNPC > bounceHeight - 0.2f) {
				rising = false;
			}
		}
		else {
			distanceAboveNPC = Mathf.SmoothStep (distanceAboveNPC, -bounceHeight, Time.deltaTime * bouncingSmooth);
			// see the "if" clause above
			if (Mathf.Abs (distanceAboveNPC) > bounceHeight - 0.2f) {
				rising = true;
			}
		}
	}

	
	// <SCAN FOR CONVERSATION> MODULE
	// ================================
	
	
	/*
	 * FIELDS
	 */
	
	// is the NPC near a player?
	public bool isNearPlayer { get; private set; }
	// range when the above will trigger "true"
	public float range = 2f;
	// space to save the player
	protected GameObject player;
	
	
	/*
	 * METHODS
	 */
	
	// save the player gameobject
	void GetPlayer() {
		player = GameObject.Find ("Player");
	}
	
	// is the player near this npc?
	void GetNearPlayer () {
		if (Vector3.Distance (player.transform.position, gameObject.transform.position) <= range) {
			if (GetComponent<Conversation>().interruptionOverride) {
				isNearPlayer = false;
			}
			else {
				isNearPlayer = true;
			}
		}
		else {
			if (GetComponent<Conversation>().interruptionOverride) {
				isNearPlayer = true;
			}
			else {
				isNearPlayer = false;
			}
		}
	}
	


	// CONVERSATION BASE CLASS
	// =========================


	/*
	 * FIELDS
	 */

	public GUISkin skin;


	/*
	public GameObject GUIStyleHolder;
	public GUIStyle style;
	public GUIStyle text;
	public GUIStyle playerChoice;
	*/

	// will display conversation text
	public bool showConversation { get; private set; }
	public Texture2D continueIcon;

	// these need to be overridden in children classes
	protected int conversationIndex = 0;


	// Content information
	/*public Dictionary<int, string> conversation = new Dictionary<int, string>();
	
	public void AddCell (int key, string val) {
		conversation.Add (key, val);
	}*/
	
	protected virtual void GetContent (int key, out string content, out Dictionary<string, int> playerLines) {//out string[] playerLines) {
		content = "It's more than a name, it's an attitude.";
		playerLines = null;
	}

	// continue text
	protected bool showContinueButton = false;


	// box resizing information
	float currentWidth = 0;
	float currentHeight = 0;
	public float maxWidth = 200f;
	public float maxHeight = 200f;

	float spacer = 5f;
	float smooth = 2f;

	// key to activate/deactivate conversations
	KeyCode useKey = KeyCode.E;

	// player's dialogue
	public bool showPlayerLine = false;
	float currentPlayerLineWidth = 0;
	float currentPlayerLineHeight = 0;
	float maxPlayerLineWidth;
	float maxPlayerLineHeight;

	protected int whereTo = -1;

	// override when a far-away character interrupts the speaker
	public bool interruptionOverride = false;
	public bool beingInterrupted = false;

	/*
	 * METHODS
	 */



	// displays either the "hey! listen! conversation available!" icon or the conversation itself
	void IsShowWindow () {
		if (isNearPlayer) {
			if (Input.GetKeyDown(useKey)) {
				showConversation = !showConversation;
			}
			if (showConversation) 
				showIcon = false;
			else {
				if (interruptionOverride)
					showIcon = false;
				else
					showIcon = true;
			}
		}
		else {
			showConversation = false;
		}
	}

	public float conversationWindowX = 0;
	public float conversationWindowY = 0;

	// the conversation window itself, where all the text is displayed
	void ConversationWindow () {




		Rect textBox = new Rect ((Camera.main.WorldToScreenPoint (transform.position).x - currentWidth * .75f) + conversationWindowX,
		                         Camera.main.WorldToScreenPoint (transform.position).y + transform.renderer.bounds.extents.y + spacer - conversationWindowY,
		                         currentWidth,
		                         currentHeight);


		//string content = GetContent (conversationIndex);
		string content;
		Dictionary<string, int> playerLinesArray;
		GetContent (conversationIndex, out content, out playerLinesArray);

		//string playerLines = string.Empty;
		/*if (playerLinesArray != null) {
			foreach (string x in playerLinesArray) {
				playerLines += "\n" + x;
			}
		}*/

		

		// resize dialogue window
		currentWidth = Mathf.Lerp (currentWidth, (showConversation ? maxWidth : 0), smooth * Time.deltaTime);
		currentHeight = Mathf.Lerp (currentHeight, (showConversation ? maxHeight : 0), smooth * Time.deltaTime);
		// prevent text-shuffling
		if (currentWidth <= 0.8f * maxWidth) {
			content = "";
			//text.wordWrap = false;
		} //else { text.wordWrap = true; }


		// PLAYER LINES

		// side offshoot containing dialogue choices
		maxPlayerLineWidth = maxWidth;
		maxPlayerLineHeight = maxHeight / 2;

		currentPlayerLineWidth = Mathf.Lerp (currentPlayerLineWidth, (showPlayerLine ? maxPlayerLineWidth : 0), smooth * Time.deltaTime);
		currentPlayerLineHeight = maxPlayerLineHeight;//Mathf.Lerp (currentPlayerLineHeight, (showPlayerLine ? maxPlayerLineHeight : 0), smooth * Time.deltaTime);

		if (showPlayerLine) {
			float boxX = textBox.x + textBox.width;
			float boxY = textBox.y + .25F * textBox.height;
			// background box
			GUI.Box (new Rect (boxX,
			                   boxY,
			                   currentPlayerLineWidth,
			                   currentPlayerLineHeight), "");
			GUILayout.BeginArea ( new Rect (boxX,
			                     boxY,
			                     currentPlayerLineWidth,
			                     currentPlayerLineHeight));
			if (playerLinesArray != null) {
				foreach (KeyValuePair<string, int> x in playerLinesArray) {
					if (GUILayout.Button (x.Key)) {
						Advance (x.Value);
					}
				}
			}
			GUILayout.EndArea();
		}



		// disappear it if it gets too small
		if (currentWidth >= 10f && currentHeight >= 10f) {
			GUI.depth = 1;
			// THIS IS THE CONTENT OF THE CONVERSATION
			GUI.Box (textBox, content);
			// THERE IT IS ^
			GUI.depth = 0;
			if (showContinueButton) {
				if (whereTo == -1) {
					if (GUI.Button (textBox, ( (int) Time.realtimeSinceStartup % 2 == 0 ? continueIcon : null))) {
						Advance ();
					}
				}
				else {
					if (GUI.Button (textBox, ( (int) Time.realtimeSinceStartup % 2 == 0 ? continueIcon : null))) {
						Advance (whereTo);
					}
				}
			}
		}
	}

	// rough "advance one"
	void Advance () {
		conversationIndex++;
	}

	protected void Advance (int where) {
		conversationIndex = where;
	}

	protected void DoneTalking() {
		showConversation = false;
	}

	protected int conversationIndexCopy;

	// have another character say something
	protected void Interrupt (GameObject character, int theirLine) {
		//character.GetComponent<Conversation>().conversationIndexCopy = character.GetComponent<Conversation>().conversationIndex;
		showConversation = false;
		interruptionOverride = true;
		character.GetComponent<Conversation>().interruptionOverride = false;
		character.GetComponent<Conversation>().conversationIndex = theirLine;
		character.GetComponent<Conversation>().showConversation = true;
	}


	//protected void DoneBeingInterrupted () {


	/*
	 * MAKIN' IT HAPPEN
	 */

	protected void Start () {
		/*
		text.wordWrap = true;
		style = GUIStyleHolder.GetComponent<MasterGUIStyle>().style;
		text = GUIStyleHolder.GetComponent<MasterGUIStyle>().text;
		playerChoice = GUIStyleHolder.GetComponent<MasterGUIStyle>().playerChoice;
		*/
		showIcon = true;
		GetPlayer();
	}

	void Update () {
		IsShowWindow();
		GetNearPlayer();
	}

	void OnGUI () {
		GUI.skin = skin;
		ShowConversationAvailable();
		CalculateDistanceAboveNPC();
		ConversationWindow();
	}
}
