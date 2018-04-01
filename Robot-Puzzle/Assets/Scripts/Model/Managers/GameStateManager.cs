using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    private RobotManager robotManager;

    private float timePerTurn = 1;
    private float timer;
    private float timeMultiplier;

    public int currentTurn;

    public bool isPaused;

	// Use this for initialization
	void Start () {
        timer = 0;
        currentTurn = 0;
        timeMultiplier = 1;
        isPaused = true;
        robotManager = GetComponent<RobotManager>();
	}

    /// <summary>
    /// Verändert den timeMultiplier, damit die Spielzeit schneller oder langsamer vergeht.
    /// </summary>
    /// <param name="multiplier"></param>
    public void ChangeTimeMultiplier(float multiplier) {
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
            ExecuteTurn();
        }
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
        isPaused = false;
        Debug.Log("Play!");
    }

    /// <summary>
    /// Pausiert das Spiel.
    /// </summary>
    private void Pause() {
        isPaused = true;
        Debug.Log("Pause!");
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
        //TODO: Jetzt die Roboter und Objekte zurücksetzen.
    }

    /// <summary>
    /// Führt alle nötigen Schritte für die derzeitige Runde aus.
    /// </summary>
    private void ExecuteTurn() {
        //TODO: Jetzt die Aktionen für die Runde ausführen.
    }
}
