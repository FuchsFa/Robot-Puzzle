using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour {

    [SerializeField]
    private GameObject robotPrefab;

    private GameStateManager gameStateManager;

    private List<GameObject> robots;

	// Use this for initialization
	void Start () {
        gameStateManager = GetComponent<GameStateManager>();
        robots = new List<GameObject>();
	}

    /// <summary>
    /// Erstellt einen neuen Roboter an den übergebenen Koordinaten.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private GameObject CreateRobot(int x, int y) {
        GameObject robotObject = Instantiate(robotPrefab);
        Robot bot = robotObject.GetComponent<Robot>();
        bot.InitializeRobot(new Vector2(0, -1), x, y);
        robotObject.transform.position = new Vector3(x + 0.5f, y + 0.5f);
        robots.Add(robotObject);
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
    private void TurnRobotStartingDirection(GameObject robotObject) {
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
        foreach(GameObject robotObject in robots) {
            Robot bot = robotObject.GetComponent<Robot>();
            bot.StartLuaScript();
        }
    }

    /// <summary>
    /// Passt die Animationsvariablen an.
    /// Führt das Lua-Skript jedes Roboters weiter aus, bis es wieder yield zurückgibt.
    /// Wird zu Beginn jeder Runde aufgerufen.
    /// </summary>
    public void PerformRobotActionsForTurn() {
        foreach(GameObject robotObject in robots) {
            robotObject.GetComponent<InteractiveObject>().AdjustAnimationVariables();
        }
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
}
