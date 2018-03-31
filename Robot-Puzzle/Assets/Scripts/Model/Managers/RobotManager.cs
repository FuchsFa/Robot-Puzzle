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
    private void CreateRobot(int x, int y) {
        GameObject robotObject = Instantiate(robotPrefab);
        Robot bot = robotObject.GetComponent<Robot>();
        bot.InitializeRobot(new Vector2(0, -1), x, y);
        robotObject.transform.position = new Vector3(x, y);
        robots.Add(robotObject);
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
        Robot bot = robotObject.GetComponent<Robot>();
        bot.ChangeStartingPosition(x, y);
    }

    /// <summary>
    /// Dreht den angegebenen Roboter um 90° im Uhrzeigersinn.
    /// </summary>
    /// <param name="robotObject"></param>
    private void TurnRobotStartingDirection(GameObject robotObject) {
        Robot bot = robotObject.GetComponent<Robot>();
        bot.TurnStartingDirection();
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
