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
        //TestStart();
	}

    /// <summary>
    /// Erstellt die Für den Test nötigen Objekte und startet das Spiel.
    /// </summary>
    private void TestStart() {
        GameObject robotObject = robotManager.CreateDefaultRobot();
        robotObject.name = "Robot A";
        WeldingTool tool = new WeldingTool(robotObject.GetComponent<Robot>());
        TextAsset text = Resources.Load<TextAsset>("Texts/BotAActions");
        string scriptCode = text.text;
        robotObject.GetComponent<Robot>().ChangeScriptCode(scriptCode);

        GameObject worldObjectOne = worldObjectManager.CreateWorldObject("Ore", 0, -1);
        worldObjectOne.name = "Ore A";
        //worldObjectOne.GetComponent<WorldObject>().OpenForConnections();
        GameObject worldObjectTwo = worldObjectManager.CreateWorldObject("Ore", 1, -1);
        worldObjectTwo.name = "Ore B";
        //worldObjectTwo.GetComponent<WorldObject>().OpenForConnections();
        //worldObjectManager.ConnectWorldObjects(worldObjectOne.GetComponent<WorldObject>(), worldObjectTwo.GetComponent<WorldObject>());
        /*GameObject worldObjectThree = worldObjectManager.CreateWorldObject("Ore", -2, -1);
        worldObjectThree.name = "Ore C";
        worldObjectManager.ConnectWorldObjects(worldObjectThree.GetComponent<WorldObject>(), worldObjectTwo.GetComponent<WorldObject>());*/

        //GameObject worldObjectIngot = worldObjectManager.CreateWorldObject("Ingot", -4, 2);

        //Play();
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
