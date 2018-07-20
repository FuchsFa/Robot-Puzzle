using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour {

    public static GameStateManager Instance { get; protected set; }

    private RobotManager robotManager;
    [HideInInspector]
    public WorldObjectManager worldObjectManager;
    public VictoryPanelManager victoryPanelManager;

    public Text playButtonText;

    public Slider speedSlider;

    private float timePerTurn = 1;
    private float timer;
    private float timeMultiplier;

    public int currentTurn;

    public bool isPaused;

	// Use this for initialization
	void Start () {
        Instance = this;
        timer = 0;
        currentTurn = 0;
        timeMultiplier = 1;
        isPaused = true;
        robotManager = GetComponent<RobotManager>();
        worldObjectManager = GetComponent<WorldObjectManager>();
	}

    /// <summary>
    /// Erstellt die Für den Test nötigen Objekte.
    /// </summary>
    public void TestSolve() {
        //Robot1
        GameObject robot1Object = robotManager.CreateRobot(-6, 3);
        robotManager.TurnRobotStartingDirection(robot1Object);
        robotManager.TurnRobotStartingDirection(robot1Object);
        ShreddingTool tool1 = new ShreddingTool(robot1Object.GetComponent<Robot>());
        BasicLeg leg1 = new BasicLeg(robot1Object.GetComponent<Robot>());
        TextAsset text = Resources.Load<TextAsset>("Texts/Robot 1Actions");
        string scriptCode = text.text;
        robot1Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        //Robot2
        GameObject robot2Object = robotManager.CreateRobot(-7, 5);
        BasicArm tool2 = new BasicArm(robot2Object.GetComponent<Robot>());
        SpiderLeg leg2 = new SpiderLeg(robot2Object.GetComponent<Robot>());
        BasicSensor sensor2 = new BasicSensor(robot2Object.GetComponent<Robot>());
        text = Resources.Load<TextAsset>("Texts/Robot 2Actions");
        scriptCode = text.text;
        robot2Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        //Robot3
        GameObject robot3Object = robotManager.CreateRobot(-7, 2);
        robotManager.TurnRobotStartingDirection(robot3Object);
        robotManager.TurnRobotStartingDirection(robot3Object);
        BasicArm tool3 = new BasicArm(robot3Object.GetComponent<Robot>());
        Boat leg3 = new Boat(robot3Object.GetComponent<Robot>());
        BasicSensor sensor3 = new BasicSensor(robot3Object.GetComponent<Robot>());
        GroundSensor sensor3_2 = new GroundSensor(robot3Object.GetComponent<Robot>());
        text = Resources.Load<TextAsset>("Texts/Robot 3Actions");
        scriptCode = text.text;
        robot3Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        //Robot4
        GameObject robot4Object = robotManager.CreateRobot(-7, -2);
        robotManager.TurnRobotStartingDirection(robot4Object);
        robotManager.TurnRobotStartingDirection(robot4Object);
        BasicArm tool4 = new BasicArm(robot4Object.GetComponent<Robot>());
        BasicLeg leg4 = new BasicLeg(robot4Object.GetComponent<Robot>());
        BasicSensor sensor4 = new BasicSensor(robot4Object.GetComponent<Robot>());
        text = Resources.Load<TextAsset>("Texts/Robot 4Actions");
        scriptCode = text.text;
        robot4Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        //Robot5
        GameObject robot5Object = robotManager.CreateRobot(-2, -3);
        robotManager.TurnRobotStartingDirection(robot5Object);
        BasicArm tool5 = new BasicArm(robot5Object.GetComponent<Robot>());
        SpiderLeg leg5 = new SpiderLeg(robot5Object.GetComponent<Robot>());
        BasicSensor sensor5 = new BasicSensor(robot5Object.GetComponent<Robot>());
        text = Resources.Load<TextAsset>("Texts/Robot 5Actions");
        scriptCode = text.text;
        robot5Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        //Robot6
        GameObject robot6Object = robotManager.CreateRobot(4, -1);
        WeldingTool tool6 = new WeldingTool(robot6Object.GetComponent<Robot>());
        SpiderLeg leg6 = new SpiderLeg(robot6Object.GetComponent<Robot>());
        BasicSensor sensor6 = new BasicSensor(robot6Object.GetComponent<Robot>());
        Scanner sensor6_2 = new Scanner(robot6Object.GetComponent<Robot>());
        text = Resources.Load<TextAsset>("Texts/Robot 6Actions");
        scriptCode = text.text;
        robot6Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        //Robot7
        GameObject robot7Object = robotManager.CreateRobot(0, -1);
        robotManager.TurnRobotStartingDirection(robot7Object);
        robotManager.TurnRobotStartingDirection(robot7Object);
        robotManager.TurnRobotStartingDirection(robot7Object);
        BasicArm tool7 = new BasicArm(robot7Object.GetComponent<Robot>());
        SpiderLeg leg7 = new SpiderLeg(robot7Object.GetComponent<Robot>());
        BasicSensor sensor7 = new BasicSensor(robot7Object.GetComponent<Robot>());
        text = Resources.Load<TextAsset>("Texts/Robot 7Actions");
        scriptCode = text.text;
        robot7Object.GetComponent<Robot>().ChangeScriptCode(scriptCode);
    }

    public void OnSpeedSliderChange() {
        ChangeTimeMultiplier(speedSlider.value);
    }

    /// <summary>
    /// Verändert den timeMultiplier, damit die Spielzeit schneller oder langsamer vergeht.
    /// </summary>
    /// <param name="multiplier"></param>
    private void ChangeTimeMultiplier(float multiplier) {
        timeMultiplier = multiplier;
    }
	
	// Update is called once per frame
	void Update () {
		if(isPaused) {
            return;
        }
        timer += Time.deltaTime * timeMultiplier;
        if(timer >= timePerTurn) {
            currentTurn++;
            timer = 0;
            PrepareTurn();
            ExecuteTurn();
        }
        robotManager.AdjustRobotObjects(timer / timePerTurn);
        worldObjectManager.AdjustWorldObjects(timer / timePerTurn);
	}

    /// <summary>
    /// Wechselt zwischen Pause und Play.
    /// </summary>
    public void TogglePause() {
        if(isPaused) {
            Play();
        } else {
            Pause();
        }
    }

    /// <summary>
    /// Startet das Spiel.
    /// </summary>
    private void Play() {
        Debug.Log("Play!");
        isPaused = false;
        playButtonText.text = "<b>Pause</b>";
        if (currentTurn == 0) {
            robotManager.StartRobotScripts();
            worldObjectManager.StartWorldObjectScripts();
        }
    }

    /// <summary>
    /// Pausiert das Spiel.
    /// </summary>
    private void Pause() {
        isPaused = true;
        Debug.Log("Pause!");
        playButtonText.text = "<b>Play</b>";
    }

    /// <summary>
    /// Setzt das Spiel auf Runde 0 zurück und pausiert das Spiel.
    /// </summary>
    public void Stop() {
        Debug.Log("Stop!");
        if(!isPaused) {
            Pause();
        }
        timer = 0;
        currentTurn = 0;
        Object[] objects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in objects) {
            go.SendMessage("OnStop", SendMessageOptions.DontRequireReceiver);
        }
        robotManager.ResetRobots();
        worldObjectManager.RemoveAllWorldObjects();
    }

    /// <summary>
    /// Bereitet alles Nötige für die derzeite Runde vor.
    /// </summary>
    private void PrepareTurn() {
        robotManager.AdjustRobotObjects(1);
        worldObjectManager.AdjustWorldObjects(1);

        robotManager.AdjustRobotAnimationVariables();
        worldObjectManager.AdjustWorldObjectAnimationVariables();
    }

    /// <summary>
    /// Führt alle nötigen Schritte für die derzeitige Runde aus.
    /// </summary>
    private void ExecuteTurn() {
        Debug.Log("----- Turn " + currentTurn + "-----");
        Object[] objects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in objects) {
            go.SendMessage("OnNewTurn", SendMessageOptions.DontRequireReceiver);
        }
        worldObjectManager.ConnectAllAvailableWorldObjects();
        robotManager.PerformRobotActionsForTurn();
        worldObjectManager.PerformWorldObjectActionsForTurn();
        robotManager.CheckForRobotTerrainCompatability();
        worldObjectManager.CheckForWorldObjectTerrainCompatability();
        if (CheckForVictory()) {
            Debug.Log("Gewonnen!");
            Pause();
            victoryPanelManager.OnVictory();
        }
    }

    /// <summary>
    /// Überprüft, ob alle Ziele erfüllt worden sind.
    /// </summary>
    /// <returns></returns>
    private bool CheckForVictory() {
        Object[] objects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in objects) {
            if(go.GetComponent<Goal>() && !go.GetComponent<Goal>().isFulfilled) {
                return false;
            }
        }
        return true;
    }
}
