using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotProgramPanelManager : MonoBehaviour {

    [SerializeField]
    private TMP_InputField editor;

    [SerializeField]
    private Button saveButton;

    [SerializeField]
    private Button discardButton;

    //Der Script code des ausgewählten Roboters ohne Änderungen.
    private string oldScriptCode;

	// Use this for initialization
	void Start () {
		
	}

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter auswählt.
    /// </summary>
    public void OnSelectRobot() {
        oldScriptCode = RobotManager.Instance.selectedRobot.GetComponent<Robot>().GetScriptCode();
        editor.textComponent.text = oldScriptCode;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
