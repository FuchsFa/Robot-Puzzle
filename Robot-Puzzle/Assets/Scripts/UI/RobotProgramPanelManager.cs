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

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter auswählt.
    /// </summary>
    public void OnSelectRobot() {
        oldScriptCode = RobotManager.Instance.selectedRobot.GetComponent<Robot>().GetScriptCode();
        editor.text = oldScriptCode;
    }

    /// <summary>
    /// Speichert die Änderungen im Code.
    /// </summary>
    public void SaveChanges() {
        oldScriptCode = editor.text;
        RobotManager.Instance.selectedRobot.GetComponent<Robot>().ChangeScriptCode(oldScriptCode);
    }
	
    /// <summary>
    /// Verwirft die Änderung im Code.
    /// </summary>
    public void DiscardChanges() {
        editor.textComponent.text = oldScriptCode;
    }
}
