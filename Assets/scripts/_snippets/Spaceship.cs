using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : MonoBehaviour {
	
	
	
	
	
	
	
	
	
	// FIELDS
	
	/*
	 * OPTIONS
	 */
	
	public bool usesFuel = true;
	public bool usesHP = true;
	
	
	/*
	 * CONTROLS
	 */
	
	KeyCode throttleKey = KeyCode.W;
	KeyCode backwardsKey = KeyCode.S;
	KeyCode rotateClockwise = KeyCode.A;
	KeyCode rotateCounterClockwise = KeyCode.D;
	KeyCode brake = KeyCode.Space;
	//KeyCode radarPing = KeyCode.E;
	//KeyCode startDock = KeyCode.R;
	
	
	/*
	 * STATS (visible to player)
	 */
	
	public int Fuel { get; private set; }
	public float Velocity { get; private set; }
	public bool ThrottleOn { get; private set; }
	public int HP { get; private set; }
	public bool Docking { get; private set; }
	
	
	/*
	 * STATS (for editor use)
	 */
	
	public float ThrottleMultiplier { get; private set; }
	public float RotationMultiplier { get; private set; }
	public bool Destroyed { get; private set; }
	public float BrakeStrength { get; private set; }
	public float MaxVelocity { get; private set; }
	
	
	/*
	 * DEFAULT VALUES
	 */
	
	// These are magic numbers - replace as necessary
	public int startingFuel = 1138;
	public float startingThrottleMultiplier = 47;
	public float startingRotationMultiplier = 42;
	public int startingHP = 314;
	public float startingDockingRange = 1.5f;
	// NOTE: Braking's effectiveness is 1/BrakeStrength, so for brake strength, lower is better. 
	public float startingBrakeStrength = 10f;
	// when to cut off the ship's velocity while braking
	public float brakingFullStop = 0.8f;
	
	
	// relationship to Out Ship

	//public GameObject outShip;
	//public Material visibleLineMaterial;
	//public Material invisibleLineMaterial;
	LineRenderer l;
	public float lineWidth = 0.05f;
	public float ShipDistance { get; private set; }
	public bool ShowDistance { get; private set; }
	public float DockingRange { get; private set; }

	
	// GUI
	/*public float DistanceBoxSize { get; private set; }
	float startingDistanceBoxSize = 20f;*/
	
	
	
	
	// METHODS
	
	// appropriate fields are initialized with default values. This is called at Start()
	void SetDefaultValues () {
		Fuel = startingFuel;
		ThrottleMultiplier = startingThrottleMultiplier;
		RotationMultiplier = startingRotationMultiplier;
		HP = startingHP;
		BrakeStrength = startingBrakeStrength;
		ShowDistance = false;
		//DistanceBoxSize = startingDistanceBoxSize;
		Docking = false;
		DockingRange = startingDockingRange;
		//l.material = invisibleLineMaterial;
	}
	
	/*
	 * CONTROLS
	 */

	void Controls () {
		
		// THROTTLE
		if (Input.GetKey (throttleKey) || Input.GetKey (backwardsKey)) {
			// checks fuel if ship uses fuel, bypasses this step if ship doesn't use fuel
			if ((usesFuel && Fuel > 0) || !usesFuel) {
				ThrottleOn = true;
				
				// whether ship moves forward or backward
				if (Input.GetKey (throttleKey)) {
					rigidbody.AddRelativeForce (Vector3.up * ThrottleMultiplier);
				}
				if (Input.GetKey (backwardsKey)) {
					rigidbody.AddRelativeForce (Vector3.down * ThrottleMultiplier);
				}
			
				// decrement fuel if appropriate
				if (usesFuel) {
					Fuel--;
				}
			}
		}
		else {
			ThrottleOn = false;
		}
		
		// ROTATION
		if (Input.GetKey (rotateClockwise)) {
			transform.Rotate (Vector3.forward * Time.deltaTime * RotationMultiplier);
		}
		if (Input.GetKey (rotateCounterClockwise)) {
			transform.Rotate (Vector3.back * Time.deltaTime * RotationMultiplier);
		}
		
		// BRAKE
		if (Input.GetKey (brake)) {
			// reduces force in direction of movement (BRAKES!)
			// braking force is 1/4 throttle force...
			if ((usesFuel && Fuel > 0) || !usesFuel) {
				rigidbody.AddForce (-rigidbody.velocity * (ThrottleMultiplier / BrakeStrength));
			}
			// ...BUT braking takes 2 times as much fuel as moving (if ship uses fuel) (assumes X,Y 2-dimensional space)
			if ((rigidbody.velocity.x != 0 ||
				rigidbody.velocity.y != 0) &&
				usesFuel) {
				Fuel = Fuel - 2;
			}
			// brings rigidbody to full stop to prevent asymptotic braking
			// refers to absolute value so -x, -y coordinates don't return a false positive
			if (Math.Abs (rigidbody.velocity.x) < brakingFullStop &&
				Math.Abs (rigidbody.velocity.y) < brakingFullStop) {
				rigidbody.velocity = new Vector3 (0, 0, 0);
			}
		}
		
		
		
		// DOCKING
		/*
		if (ShipDistance < DockingRange) {
			if (Input.GetKeyDown (startDock) && !Docking) {
				Docking = true;
			}
		}
		// breaks the dock if not in range
		else {
			Docking = false;
		}
		*/
		
	}
	
	void UpdateControls () {
		// DISTANCE-MEASURING LINE
		/*
		if (Input.GetKeyDown (radarPing)) {
			RadarPing();
		}
		*/
	}
	
	/*
	 * RADAR DISPLAY DISTANCE TOGGLE
	 */
	
	void RadarPing () {
		/*
		ShowDistance = !ShowDistance;
		if (ShowDistance) {
			l.material = visibleLineMaterial;
		}
		else {
			l.material = invisibleLineMaterial;
		}
		*/
	}
	
	
	
	/*
	 * CALCULATE VELOCITY
	 */
	
	void CalculateVelocity () {
		Velocity = rigidbody.velocity.magnitude;
	}
	
	/*
	 * DESTROY SHIP
	 */
	
	void ShipDestroyed () {
		// only trigger if "Uses HP" is checked
		if (usesHP && HP <= 0) {
			Destroyed = true;
		}
	}
	
	/*
	 * CREATE, UPDATE, AND MEASURE LINE
	 */
	/*
	void CreateLine () {
		l = gameObject.AddComponent<LineRenderer>();
		l.SetWidth (lineWidth, lineWidth);
		// vertices located at player and at Out ship
		l.SetVertexCount(2);
		l.material = invisibleLineMaterial;
	}
		
	void UpdateLine () {
		l.SetPosition (0, transform.position);
		l.SetPosition (1, outShip.transform.position);
	}
	
	void LineLength () {
		ShipDistance = Vector3.Distance (transform.position, outShip.transform.position);
	}
	*/
	
	
	
	
	
	
	
	
	
	// MAKIN' IT HAPPEN
	void Start () {
		SetDefaultValues();
		//CreateLine();
		
	}
	
	void FixedUpdate () {
		Controls();
	}
	
	void Update () {
		CalculateVelocity();
		ShipDestroyed();
		//UpdateLine();
		//LineLength();
		UpdateControls();
		
	}
	
	void OnGUI () {
		//GUIDistance();
	}
	
	
}
