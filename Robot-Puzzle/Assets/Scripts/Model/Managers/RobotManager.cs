﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class RobotManager : MonoBehaviour {

    public static RobotManager Instance { get; protected set; }

    public bool RobotPlacementActive {
        get {
            return robotPlacementActive;
        }

        set {
            robotPlacementActive = value;
        }
    }

    [SerializeField]
    private GameObject robotPrefab;

    public const int robotCost = 100;

    [SerializeField]
    private GameObject previewObject;

    private GameStateManager gameStateManager;

    private List<GameObject> robots;

    private bool robotPlacementActive;

    public GameObject selectedRobot;

    [SerializeField]
    private RobotDetailPanelManager panelManager;

    [SerializeField]
    private RobotProgramPanelManager programManager;

    public Tilemap tilemap;

	// Use this for initialization
	void Start () {
        Instance = this;
        robotPlacementActive = false;
        gameStateManager = GetComponent<GameStateManager>();
        robots = new List<GameObject>();
	}

    /// <summary>
    /// Erstellt einen neuen Roboter an den übergebenen Koordinaten.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public GameObject CreateRobot(int x, int y) {
        GameObject robotObject = Instantiate(robotPrefab);
        Robot bot = robotObject.GetComponent<Robot>();
        bot.InitializeRobot(new Vector2(0, -1), x, y);
        robotObject.transform.position = new Vector3(x + 0.5f, y + 0.5f);
        robots.Add(robotObject);
        robotObject.name = "Robot " + robots.Count;
        return robotObject;
    }

    /// <summary>
    /// Entfernt den angegebenen Roboter aus dem Spiel.
    /// </summary>
    /// <param name="robotObject"></param>
    private void RemoveRobot(GameObject robotObject) {
        if(robots.Contains(robotObject)) {
            robots.Remove(robotObject);
            Destroy(robotObject);
        }
    }

    /// <summary>
    /// Ändert die Anfangkoordinaten des angegebenen Roboters.
    /// </summary>
    /// <param name="robotObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void ChangeRobotSartingPosition(GameObject robotObject, int x, int y) {
        InteractiveObject obj = robotObject.GetComponent<InteractiveObject>();
        obj.ChangeStartingPosition(x, y);
    }

    /// <summary>
    /// Dreht den angegebenen Roboter um 90° im Uhrzeigersinn.
    /// </summary>
    /// <param name="robotObject"></param>
    public void TurnRobotStartingDirection(GameObject robotObject) {
        InteractiveObject obj = robotObject.GetComponent<InteractiveObject>();
        obj.TurnStartingDirection();
    }

    /// <summary>
    /// Fügt das übergebene Teil zum angegebenen Roboter hinzu.
    /// </summary>
    /// <param name="robotObject"></param>
    /// <param name="part"></param>
    public void AddPartToRobot(GameObject robotObject, RobotPart part) {
        Robot bot = robotObject.GetComponent<Robot>();
        bot.AddPart(part);
    }

    /// <summary>
    /// Entfernt das übergebene Teil vom angegebenen Roboter.
    /// </summary>
    /// <param name="robotObject"></param>
    /// <param name="part"></param>
    public void RemovePartFromRobot(GameObject robotObject, RobotPart part) {
        Robot bot = robotObject.GetComponent<Robot>();
        bot.RemovePart(part);
    }

    /// <summary>
    /// Erstellt einen Roboter an Stelle 0, 0 und fügt einen BasicArm, ein BasicLeg und einen BasicSensor zu iihm hinzu.
    /// </summary>
    public GameObject CreateDefaultRobot() {
        GameObject robotObject = CreateRobot(0, 0);
        BasicArm arm = new BasicArm();
        BasicLeg leg = new BasicLeg();
        BasicSensor sensor = new BasicSensor();
        AddPartToRobot(robotObject, arm);
        AddPartToRobot(robotObject, leg);
        AddPartToRobot(robotObject, sensor);
        return robotObject;
    }

    /// <summary>
    /// Ruft die StartLuaScript Funktion jedes Roboter auf, damit diese ihre Aktionen ausführen können.
    /// Wird aufgerufen, wenn der Spieler auf 'Play' drückt.
    /// </summary>
    public void StartRobotScripts() {
        bool scriptNotValid = false;
        foreach(GameObject robotObject in robots) {
            Robot bot = robotObject.GetComponent<Robot>();
            if(!bot.IsLuaScriptValid(bot.GetScriptCode())) {
                scriptNotValid = true;
            }
            bot.StartLuaScript();
        }
        if(scriptNotValid) {
            //TODO: Error-Nachricht auf die ingamekonsole schreiben.
            gameStateManager.Stop();
        }
    }

    /// <summary>
    /// Passt die Animationsvariablen von Robotern an.
    /// </summary>
    public void AdjustRobotAnimationVariables() {
        foreach (GameObject robotObject in robots) {
            robotObject.GetComponent<InteractiveObject>().AdjustAnimationVariables();
        }
    }

    /// <summary>
    /// Überprüft, ob alle Roboter auch auf dem Tile stehen dürfen, auf dem sie gerade stehen.
    /// </summary>
    public void CheckForRobotTerrainCompatability() {
        foreach(GameObject robotObject in robots) {
            Robot bot = robotObject.GetComponent<Robot>();
            int x = robotObject.GetComponent<InteractiveObject>().posX;
            int y = robotObject.GetComponent<InteractiveObject>().posY;
            GroundTile tile = tilemap.GetTile<GroundTile>(new Vector3Int(x, y, 0));
            if (!bot.CanWalkOn(tile)) {
                Debug.LogError(robotObject.name + " darf nicht auf dem Tile stehen, auf dem es gerade steht.");
                ConsolePanelManager.Instance.LogErrorToInGameConsole(robotObject.name + " can't stand on " + tile.terrainType.ToString() + " ground.");
                gameStateManager.Stop();
                return;
            }
        }
    }

    /// <summary>
    /// Führt das Lua-Skript jedes Roboters weiter aus, bis es wieder yield zurückgibt.
    /// Wird zu Beginn jeder Runde aufgerufen.
    /// </summary>
    public void PerformRobotActionsForTurn() {
        foreach(GameObject robotObject in robots) {
            Robot bot = robotObject.GetComponent<Robot>();
            if(bot.HasActionsLeft()) {
                bot.ResumeAction();
            }
        }
    }

    /// <summary>
    /// Setzt alle Roboter auf ihren Anfangsstatus zurück.
    /// </summary>
    public void ResetRobots() {
        foreach(GameObject robotObject in robots) {
            Robot bot = robotObject.GetComponent<Robot>();
            bot.ResetRobot();
        }
    }

    /// <summary>
    /// Animiert die GameObjects der Roboter, damit diese graduell an ihre derzeitige Position und Rotation angepasst werden.
    /// </summary>
    /// <param name="percentage"></param>
    public void AdjustRobotObjects(float percentage) {
        foreach(GameObject robotObject in robots) {
            robotObject.GetComponent<InteractiveObject>().AdjustGameObject(percentage);
        }
    }

    /// <summary>
    /// Aktiviert das PreviewObjekt und setzt den passenden bool-Wert auf true.
    /// </summary>
    public void StartRobotPlacement() {
        previewObject.SetActive(true);
        robotPlacementActive = true;
    }

    private void Update() {
        if(robotPlacementActive) {
            HandleRobotPlacement();
        } else {
            HandleUserInput();
        }
    }

    /// <summary>
    /// Verändert die Position des PreviewObjekts und platziert einen Roboter an dessen Stelle, wenn der Spieler den linken Mausbutton loslässt.
    /// </summary>
    private void HandleRobotPlacement() {
        if (gameStateManager.isPaused && Input.GetMouseButton(0)) {
            float xPos = Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) + 0.5f;
            float yPos = Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y) + 0.5f;
            previewObject.transform.position = new Vector3(xPos, yPos);
        }
        else if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Stop Robot Placement");
            int xPos = Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
            int yPos = Mathf.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            if (tilemap.GetTile<GroundTile>(new Vector3Int(xPos, yPos, 0))) {
                //Debug.Log("An der Stelle " + xPos + "/" + yPos + " gibt es ein Tile.");
                CreateRobot(xPos, yPos);
                RobotDetailPanelManager.Instance.AdjustTotalCostDisplay();
            }
            robotPlacementActive = false;
            previewObject.SetActive(false);
        }
    }

    /// <summary>
    /// Bearbeitet das Auswählen, Verschieben, Drehen und Löschen von Robotern durch den Spieler.
    /// </summary>
    private void HandleUserInput() {
        if(!EventSystem.current.IsPointerOverGameObject()) {
            if (gameStateManager.isPaused && Input.GetMouseButtonDown(0)) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
                //selectedRobot = null;
                if (hit) {
                    GameObject selected = hit.transform.gameObject;
                    Debug.Log("Selected " + selected.name);
                    if (selected.GetComponent<Robot>()) {
                        selectedRobot = selected;
                        panelManager.OnSelectRobot();
                        programManager.OnSelectRobot();
                    } else if (hit.transform.gameObject.layer != 5) {
                        selectedRobot = null;
                        panelManager.OnDeselectRobot();
                    }

                } else {
                    selectedRobot = null;
                    panelManager.OnDeselectRobot();
                }
            } else if (gameStateManager.isPaused && Input.GetMouseButton(0)) {
                if (selectedRobot != null) {
                    float xPos = Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) + 0.5f;
                    float yPos = Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y) + 0.5f;
                    selectedRobot.transform.position = new Vector3(xPos, yPos);
                    selectedRobot.GetComponent<InteractiveObject>().ChangeStartingPosition(Mathf.FloorToInt(xPos), Mathf.FloorToInt(yPos));
                }
            }
            if (gameStateManager.isPaused && Input.GetMouseButtonDown(1)) {
                if (selectedRobot != null) {
                    selectedRobot.GetComponent<InteractiveObject>().TurnStartingDirection();
                }
            }
        }
        if (gameStateManager.isPaused && Input.GetKeyDown(KeyCode.Delete)) {
            if (selectedRobot != null) {
                RemoveRobot(selectedRobot);
                selectedRobot = null;
                panelManager.OnDeselectRobot();
                RobotDetailPanelManager.Instance.AdjustTotalCostDisplay();
            }
        }
    }

    /// <summary>
    /// Speichert die Skripte aller Roboter auf dem Spielfeld als .txt-Dateien im Resources/Texts Ordner.
    /// </summary>
    public void SaveCurrentRobotScripts() {
        if(gameStateManager.isPaused) {
            ConsolePanelManager.Instance.LogStringToInGameConsole("Saving Robot scripts for " + robots.Count + " robots.");
            foreach(GameObject robotObject in robots) {
                string scriptCode = robotObject.GetComponent<Robot>().GetScriptCode();
                System.IO.File.WriteAllText("D:/Daten/Studium/BachelorArbeit/Robot-Puzzle/Robot-Puzzle/Assets/Resources/Texts/" + robotObject.name + "Actions.txt", scriptCode);
                ConsolePanelManager.Instance.LogStringToInGameConsole(robotObject.name + "'s script has been saved.");
            }
            ConsolePanelManager.Instance.LogStringToInGameConsole("done saving robot scripts.");
        } else {
            ConsolePanelManager.Instance.LogStringToInGameConsole("Can't save scripts in play mode.");
        }
    }

    /// <summary>
    /// Gibt die gesamten Kosten für alle Roboter und Roboterteile zurück.
    /// </summary>
    /// <returns></returns>
    public int GetTotalRobotCost() {
        int tempCost = 0;

        foreach(GameObject roboObj in robots) {
            tempCost += robotCost;
            Robot robo = roboObj.GetComponent<Robot>();
            foreach(RobotPart part in robo.GetRobotPartList()) {
                tempCost += part.cost;
            }
        }

        return tempCost;
    }

    /// <summary>
    /// Gibt die gesamte Länge aller Roboter-Skripte zurück.
    /// </summary>
    /// <returns></returns>
    public int GetTotalCodeLength() {
        int temp = 0;

        foreach (GameObject roboObj in robots) {
            Robot robo = roboObj.GetComponent<Robot>();
            int scriptLength = robo.GetScriptCode().Split('\n').Length;
            temp += scriptLength;
        }

        return temp;
    }
}
