using UnityEngine;
using System.Collections;

public class AntLion : MonoBehaviour {

	float start_time;
	float last_time;
	float agr_delta_start;
	float agr_delta_len = 3.0f;
	float leg_cycle;

	Hashtable initialRotations = new Hashtable();

	float agression;
	public float creep_min = 0.0f;
	public float creep_range = 2.0f;
	public float head_range = 40.0f;

	// Use this for initialization
	void Start () {
		start_time = Time.time;
		//make creep happen right away
		agr_delta_start = start_time - agr_delta_len - 0.01f;
	}

	void setLocalEulerZ(string childName, float angle) {
		//to imitate flash's rotation relative to original angle setting,
		//store the original angle for each child
		Transform t = transform.Find(childName);
		if (!initialRotations.Contains(childName)) {
			initialRotations[childName] = t.localEulerAngles.z;
		}

		//then rotate along local z axis
		Vector3 angles = t.localEulerAngles;
		angles.z = (float)initialRotations[childName] + angle;
		t.localEulerAngles = angles;
	}

	// Update is called once per frame
	void Update () {
		float current_time = Time.time;
		float elapsed = current_time - start_time + 0.00001f;
		float local_elapsed = current_time - last_time;
		
		//Note: a movieclip's _rotation property uses degrees, but all of Flash's trig functions use radians.
		
		//Move the head
		float head_period = 5.0f + Mathf.Sin( elapsed / 2.0f * Mathf.PI + (Mathf.PI / 2) );
		setLocalEulerZ("Head",head_range*Mathf.Deg2Rad * Mathf.Sin( elapsed / head_period ) * Mathf.Rad2Deg);


		//Move the claws
		float claw_period = 2.0f * Mathf.Sin( elapsed / 2.0f * Mathf.PI );
		setLocalEulerZ("Head/LeftClaw", 10 + 8 * Mathf.Sin( agression * elapsed / claw_period * Mathf.PI) );
		setLocalEulerZ("Head/RightClaw", -transform.Find ("Head/LeftClaw").localEulerAngles.z);

		//Move the antlion out of its hole according to its agression.  The higher the agression, the further it moves out of its hole
		float creep_velocity = ((creep_min + (agression * creep_range)) - transform.localPosition.y) / 4 * (local_elapsed * 10.0f);
		transform.Translate (0.0f, creep_velocity, 0.0f, Space.World);

		//Move the legs according to how fast the antlion is moving; the legs all run on the same cycle, but they're offset by one quarter cycle (Mathf.PI / 2) from each other.  I've put the unsimiplified math in the comments to make this more clear
		leg_cycle += creep_velocity;
		setLocalEulerZ("LeftRearLeg", 10 * Mathf.Sin(leg_cycle * Mathf.PI));							//leg_cycle * Mathf.PI + (0 * (Mathf.PI / 2))
		setLocalEulerZ("LeftFrontLeg", 10 * Mathf.Sin(leg_cycle * Mathf.PI + Mathf.PI));					//leg_cycle * Mathf.PI + (2 * (Mathf.PI / 2))
		setLocalEulerZ("RightRearLeg", 10 * Mathf.Sin(leg_cycle * Mathf.PI + (Mathf.PI / 2)));			//leg_cycle * Mathf.PI + (1 * (Mathf.PI / 2))
		setLocalEulerZ("RightFrontLeg", 10 * Mathf.Sin(leg_cycle * Mathf.PI + (3 * Mathf.PI / 2)));		//leg_cycle * Mathf.PI + (3 * (Mathf.PI / 2))

		//Update the agression randomly after a random interval between 1 and 4 seconds
		if (current_time - agr_delta_start > agr_delta_len)
		{
			agr_delta_start = current_time;
			agr_delta_len = 1.0f + Random.value * 3.0f;
			agression = Random.value;
		}
		
		last_time = current_time;
	}
}
