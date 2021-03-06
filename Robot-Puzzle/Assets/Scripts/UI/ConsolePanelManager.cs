﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsolePanelManager : MonoBehaviour {

    public static ConsolePanelManager Instance { get; protected set; }

    [SerializeField]
    private GameObject consolePanel;

    [SerializeField]
    private Text consoleText;

    [SerializeField]
    private TMP_InputField inputField;

    private string consoleContent;

    private bool enableDevCommands = false;

	// Use this for initialization
	void Start () {
        Instance = this;
        consoleText.text = "";
        consoleContent = "";
        inputField.onEndEdit.AddListener(LogStringFromInputField);
	}

    /// <summary>
    /// Logt den im inputField eingegebenen Text in die Konsole und selektiert das inputField danach wieder.
    /// </summary>
    /// <param name="text"></param>
    public void LogStringFromInputField(string text) {
        LogStringToInGameConsole(text);
        if (text.StartsWith("/")) {
            ProcessCommand(text);
        }
        inputField.text = "";
        inputField.Select();
        inputField.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current));
    }

    private void ProcessCommand(string command) {
        if(command == "/dev" && Application.isEditor) {
            enableDevCommands = true;
            LogStringToInGameConsole("Developer mode enabled.");
        }
        if(enableDevCommands) {
            if(command == "/solve") {
                GameStateManager.Instance.TestSolve();
            }
            if(command == "/save") {
                RobotManager.Instance.SaveCurrentRobotScripts();
            }
            if(command == "/win") {
                GameStateManager.Instance.victoryPanelManager.OnVictory();
            }
        }
    }

    /// <summary>
    /// Schreibt den übergebenen string in die Debug-Konsole
    /// </summary>
    /// <param name="text"></param>
    public void LogStringToInGameConsole(string text) {
        if(text.Length == 0) {
            return;
        }
        //Debug.Log("Trying to log '" + text + "' to the ingame console.");

        consoleContent += "\n";
        consoleContent += text;
        consoleText.text = consoleContent;
    }

    /// <summary>
    /// Schreibt den übergebenen string in Gelb in die Debug-Konsole
    /// </summary>
    /// <param name="text"></param>
    public void LogWarningToInGameConsole(string text) {
        consoleContent += "\n";
        consoleContent += "<color=yellow>" + text + "</color>";
        consoleText.text = consoleContent;
    }

    /// <summary>
    /// Schreibt den übergebenen string in Rot in die Debug-Konsole
    /// </summary>
    /// <param name="text"></param>
    public void LogErrorToInGameConsole(string text) {
        consoleContent += "\n";
        consoleContent += "<color=red>" + text + "</color>";
        consoleText.text = consoleContent;
    }

    /// <summary>
    /// Löscht den Text der Konsole
    /// </summary>
    public void ClearInGameConsole() {
        consoleContent = "";
        consoleText.text = consoleContent;
    }

    /// <summary>
    /// Zeigt das Konsolenfenster
    /// </summary>
    public void ShowConsolePanel() {
        consolePanel.SetActive(true);
    }

    /// <summary>
    /// Versteckt das Konsolenfenster
    /// </summary>
    public void HideConsolePanel() {
        consolePanel.SetActive(false);
    }
}
