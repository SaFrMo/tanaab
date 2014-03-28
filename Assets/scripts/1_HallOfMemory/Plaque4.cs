using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plaque4 : Conversation {

	// IMPLEMENTATION:
	/* GetContent is the main function here. First, it resets the following values:
	 * 		playerLines: the dictionary<string, int> that contains what a player says and what index that leads to
	 * 		showContinueButton: whether or not the NPC has more to say before the player can chime in
	 * 		showPlayerLine: what the player can say to the NPC
	 * 		whereTo: -1 is a flag value that means clicking on the "next" button will simply advance the conversation by 1.
	 * 			If AllowContinue(int x) is called, -1 will be replaced by x, which will take the conversation to a different
	 * 			place than the next index.
	 * 
	 */

	// this is the string the NPC will say
	string toContent;



	// allow progression to next conversation index
	void AllowContinue () {
		showContinueButton = true;
	}

	// jump to a special index
	void AllowContinue (int where) {
		showContinueButton = true;
		whereTo = where;
	}

	void AllowPlayerLines () {
		showPlayerLine = true;
	}

	// HOW TO USE
	// 1. toContent = what the NPC will say.
	// 2a. If the player is allowed to progress to the next line without any choice, call AllowContinue() or AllowContinue(int where).
	// 2b. If the player has something to say, call AllowPlayerLines() and then create:
	// 		Dictionary<string, int> playerLines = new Dictionary<string, int>() { ... };
	// 		where each string is the player dialogue choice and each int is the corresponding index of the NPC's response
	// 3. If the NPC is to be interrupted, call Interrupt (gameObject interrupter, int lineInterrupterSays); on the relevant case.
	//		Make sure to set conversationIndex on the other character to the one you want them to start out with when you speak with them next.
	protected override void GetContent (int key, out string content, out Dictionary<string, int> playerLines) {

		interruptionOverride = false;
		playerLines = null;
		showContinueButton = false;
		showPlayerLine = false;
		whereTo = -1;

		switch (key) {


		case 0:
			toContent = "4. THE NEW INDUSTRY.";
			AllowPlayerLines();
			playerLines = new Dictionary<string, int>() {
				{ "[Continue]", 1 }
			};
			break;

			// 

		case 1:
			GAME_MANAGER.PLAYER.GetComponent<WASDMovement>().Freeze (true);
			toContent = "It was around the year and a half mark that entrepeneurs realized what a gold mine had developed, and travel agencies sprung up, mold-like, constantly working to one-up each other and offer the \"real Tanaab experience.\" ";
			AllowPlayerLines();
			playerLines = new Dictionary<string, int>() {
				{ "[Continue]", 2 }
			};
			break;

		case 2:
			toContent = "The battle itself was still raging, galvanized continuously by outside resources and pilots.";
			AllowPlayerLines();
			playerLines = new Dictionary<string, int>() {
				{ "[Close]", 3 }
			};
			break;

		case 3:
			GAME_MANAGER.PLAYER.GetComponent<WASDMovement>().Freeze (false);
			DoneTalking();
			Advance (0);
			break;

		};

		if (!showConversation)
			showPlayerLine = false;

		content = toContent;
	}
	


	
}
