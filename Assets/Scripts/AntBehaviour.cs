using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AntBehaviour : MonoBehaviour {

	private GameObject grid;
	private GridMaker gridMaker;
	private GlobalState globalState;
	private GameObject cameraObject;
	private GameObject sand;
	private GameObject landslideIndicator;
    private GameObject chanceDisplay;
	private GameObject timerDisplay;
    private GameObject stageDisplay;
    private GameObject finalChanceDisplay;
	private Bounds antBounds;

	Vector2 antPos;
	Vector2 targetPos;
	enum MoveType {NONE, HORIZONTAL, VERTICAL, LANDSLIDE};

    float indicatorPeriod = 1.0f; //The time required to make one trip from edge to edge for indicator bar
    float indicatorMultiplier = 0f; //The multiplier to the current chance
	float indicatorCurrentValue = 0f;
    float INDICATOR_MAX_MULTIPLIER = 2.0f;
    float INDICATOR_MIN_MULTIPLIER = 0f;
    float additionalLandSlideChance_vertical = 1.2f; //The probability multiplier each time a vertical move succeeds
	float currentStageTime = 0f; //The time elapsed since the start of stage
	float TIME_LIMIT = 90f; // Time limit per stage
    float currentStage = 1f; // The current stage
    
	struct SlideProbs {public float horizontal; public float vertical;}
	SlideProbs probabilities = new SlideProbs { horizontal = 0.1f, vertical = 0.5f };
	MoveType movementType = MoveType.NONE;
	bool inSuspense = false;
	float movementStartTime;
	float suspenseStartTime;
	public float movementDurationSeconds = 1.0f;

	// Use this for initialization
	void Start () {
		//grab the grid maker
		grid = GameObject.Find ("Grid");
		DebugUtils.Assert(grid);
		gridMaker = grid.GetComponent<GridMaker> ();
		DebugUtils.Assert(gridMaker);
		sand = GameObject.Find ("SandBackground");
		DebugUtils.Assert (sand);
		cameraObject = GameObject.Find ("Main Camera");
		DebugUtils.Assert(cameraObject);
		landslideIndicator = GameObject.Find("MySlider");
		DebugUtils.Assert (landslideIndicator);
		chanceDisplay = GameObject.Find("MyChanceDisplay");
		DebugUtils.Assert(chanceDisplay);
		timerDisplay = GameObject.Find("MyStageTimer");
		DebugUtils.Assert(timerDisplay);
        stageDisplay = GameObject.Find("MyStageIndicator");
        DebugUtils.Assert(stageDisplay);
        Text txt = stageDisplay.GetComponent<Text>();
        txt.text = "STAGE " + currentStage.ToString();
        finalChanceDisplay = GameObject.Find("MyLandslideChance");
        DebugUtils.Assert(finalChanceDisplay);

        //make sure we have a global state - create one if it's missing (probably started scene from within Unity editor)
        GameObject glob = GameObject.Find ("GlobalState");
		if (glob) {
			globalState = glob.GetComponent<GlobalState>();
		} else {
			globalState = grid.AddComponent<GlobalState>();
			Debug.LogWarning("Creating global state object on the fly - assuming game is running in editor");
		}

		//size the ant to fill one grid cell, and put it in a bottom centre cell
		Sprite sprite = GetComponent<SpriteRenderer> ().sprite;
		antBounds = sprite.bounds;
		transform.localScale = new Vector3 (1.0f/antBounds.size.x * gridMaker.gridCellWidth, 1.0f/antBounds.size.y*gridMaker.gridCellHeight, 1.0f);
		antPos.x = (int)(gridMaker.gridSizeHorizontal / 2.0f);
		targetPos = antPos;

      

		ResetProbabilities();
		UpdateMovement ();
	}

	void InitializeMovement() {
		movementStartTime = Time.time;
	}

	//transform a grid-cell x,y coordinate into a world x,y,z coordinate
	private Vector3 GridPosToVector3(Vector2 pos) {
		return grid.transform.position + new Vector3 (gridMaker.gridCellWidth * pos.x, gridMaker.gridCellHeight * pos.y, 0.0f);
	}

	//initialize the landslide probabilities based on difficulty level
	void ResetProbabilities() {
		switch (globalState.difficulty) {
		case GlobalState.Difficulty.EASY: {
			probabilities.horizontal = 0.2f;
			probabilities.vertical = 0.3f;
			break;
		}
		case GlobalState.Difficulty.MEDIUM: {
			probabilities.horizontal = 0.25f;
			probabilities.vertical = 0.4f;
			break;
		}
		case GlobalState.Difficulty.HARD: {
			probabilities.horizontal = 0.3f;
			probabilities.vertical = 0.5f;
			break;
		}
		}
	}

	//calculate the current state of the game
	public GlobalState.GameState CheckWinLose() {
		if (antPos.y >= gridMaker.gridSizeVertical)
		{
            currentStage = currentStage + 1f;
            indicatorPeriod = indicatorPeriod - 0.5f;
			return GlobalState.GameState.WON;
		} else if (antPos.y < 0 || currentStageTime > 90) {
			return GlobalState.GameState.LOST;
		}
		return GlobalState.GameState.PLAYING;
	}

	//determine the outcome of the current move action
	void CheckProbabilities() {
		switch (movementType) {
		case MoveType.HORIZONTAL: {
			if (Random.value < probabilities.horizontal * indicatorMultiplier) {
				MoveLandSlide(-1);
			} else {
				movementType = MoveType.NONE;
				//probabilities.horizontal += 0.1f;
				//probabilities.vertical /= 1.5f;
			}
			break;
		}
		case MoveType.VERTICAL: {
			if (Random.value < probabilities.vertical * indicatorMultiplier) {
				MoveLandSlide(-2);
			} else {
				movementType = MoveType.NONE;
                probabilities.horizontal *= additionalLandSlideChance_vertical;
                probabilities.vertical *= additionalLandSlideChance_vertical;
			}
			break;
		}
		case MoveType.LANDSLIDE: {
			movementType = MoveType.NONE;
			ResetProbabilities();
			break;
		    }
		}
	}

	//update the ant location, and handle the current movement
	void UpdateMovement() {
		Text textBox = chanceDisplay.GetComponent<Text>();
        textBox.text =  "The Landslide Chance Multiplier:" + indicatorMultiplier.ToString();
        Text textBox2 = finalChanceDisplay.GetComponent<Text>();
        textBox2.text = "Current Landslide Chance:" + (probabilities.vertical * indicatorMultiplier * 100) + "%";
        Vector3 movement = Vector3.zero;
		if (inSuspense) {
			if (suspenseStartTime + 1.0f < Time.time) {
				CheckProbabilities();
				GetComponent<SpriteRenderer> ().color = Color.white;
				inSuspense = false;
			}
		}
		if (!inSuspense && movementType != MoveType.NONE) {
			float elapsed = (Time.time-movementStartTime)/movementDurationSeconds;
			if (elapsed > 1) {
				antPos = targetPos;
				movement = Vector3.zero;
				if (CheckWinLose() != GlobalState.GameState.PLAYING)
				{
				} else {
					suspenseStartTime = Time.time;
					inSuspense = true;
					if (movementType != MoveType.LANDSLIDE) {
						GetComponent<SpriteRenderer> ().color = Color.blue;
					} else {
						GetComponent<SpriteRenderer> ().color = Color.red;
					}
				}
			} else {
				Vector3 start = GridPosToVector3(antPos);
				Vector3 target = GridPosToVector3(targetPos);
				float progress = 1 - Mathf.Pow( Mathf.Cos( elapsed * Mathf.PI / 2 ), 2 );
				movement = (target-start) * progress;
			}
		}
		transform.position = GridPosToVector3(antPos) + movement;

		Camera cam = cameraObject.GetComponent<Camera>();
		Bounds sandBounds = sand.GetComponent<SpriteRenderer>().bounds;
		float camx = Mathf.Max (sandBounds.min.x + cam.orthographicSize*cam.aspect,transform.position.x + antBounds.extents.x);
		camx = Mathf.Min (sandBounds.max.x - cam.orthographicSize*cam.aspect, camx);
		float camy = Mathf.Max (sandBounds.min.y + cam.orthographicSize, transform.position.y - 1.0f + antBounds.extents.y);
		camy = Mathf.Min (sandBounds.max.y - cam.orthographicSize, camy);
		cameraObject.transform.position = new Vector3(camx, camy, cameraObject.transform.position.z);
		landslideIndicator.transform.position = new Vector3 (landslideIndicator.transform.position.x, camy, landslideIndicator.transform.position.z);
		chanceDisplay.transform.position = new Vector3(chanceDisplay.transform.position.x, camy, chanceDisplay.transform.position.z);
		timerDisplay.transform.position = new Vector3(timerDisplay.transform.position.x, camy+2.0f, timerDisplay.transform.position.z);
        stageDisplay.transform.position = new Vector3(stageDisplay.transform.position.x, camy + 2.0f, stageDisplay.transform.position.z);
        finalChanceDisplay.transform.position = new Vector3(finalChanceDisplay.transform.position.x, camy - 1.5f, finalChanceDisplay.transform.position.z);
    }
	
	void MoveLandSlide(int steps) {
		MoveVertical(steps, true);
	}

	void MoveHorizontal(int steps) {
		targetPos = antPos;
		targetPos.x += steps;
		targetPos.x = Mathf.Max (targetPos.x, 0);
		targetPos.x = Mathf.Min (targetPos.x, gridMaker.gridSizeHorizontal - 1);
		if (targetPos != antPos) {
			movementType = MoveType.HORIZONTAL;
			InitializeMovement();
		}
	}

	void MoveVertical(int steps, bool isLandSlide = false) {
		targetPos = antPos;
		targetPos.y += steps;
		movementType = isLandSlide ? MoveType.LANDSLIDE : MoveType.VERTICAL;
		InitializeMovement();
	}

	private bool isIncreasing = true;
	private bool isDecreasing = false;

	// Update is called once per frame
	void Update () {
		//Check if time is up
		if (currentStageTime > TIME_LIMIT) 
		{
		}
		//Update stage time
		currentStageTime = currentStageTime + 0.033f;
		Text textBox2 = timerDisplay.GetComponent<Text>();
		textBox2.text = currentStageTime.ToString();
		Slider mySlider = landslideIndicator.GetComponent<Slider> ();
		if (mySlider.value <= 0) {
			isIncreasing = true;
			isDecreasing = false;
		}
		if (mySlider.value >= 2) {
			isDecreasing = true;
			isIncreasing = false;
		}
		if (isIncreasing) {
			mySlider.value = mySlider.value + 0.033f;
			indicatorCurrentValue = mySlider.value;
		}
		if (isDecreasing) {
			mySlider.value = mySlider.value - 0.033f;
			indicatorCurrentValue = mySlider.value;
		}


		//if not currently moving, allow new movement based on user input
		if (movementType == MoveType.NONE) {
            if (indicatorCurrentValue <= 1)
            {
                indicatorMultiplier = -2f * indicatorCurrentValue + (INDICATOR_MAX_MULTIPLIER * 2f / INDICATOR_MAX_MULTIPLIER);
            }
            else
            {
                indicatorMultiplier = 2f * indicatorCurrentValue - (INDICATOR_MAX_MULTIPLIER * 2f / INDICATOR_MAX_MULTIPLIER);
            }
            if (Input.GetKeyDown (KeyCode.UpArrow)) {
                StopIndicator();
                MoveVertical(1);
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
                StopIndicator();
                MoveVertical(-1);
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
                StopIndicator();
                MoveHorizontal (-1);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
                StopIndicator();
                MoveHorizontal (1);
			}
		}
		//always update the ant movement
		UpdateMovement();
	}

    void StopIndicator()
    {
        //Stop the indicator and re-evaludate the chance
        //indicatorMultiplier = (INDICATOR_MAX_MULTIPLIER - INDICATOR_MIN_MULTIPLIER) * respective position of bar
        
        //Re-initiate indicator within 3 seconds
        
    }
}
