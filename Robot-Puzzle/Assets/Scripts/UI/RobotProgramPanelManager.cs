using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotProgramPanelManager : MonoBehaviour {

    [SerializeField]
    private TMP_InputField editor;

    [SerializeField]
    private TextMeshProUGUI lineNumberText;

    [SerializeField]
    private ScrollRect lineNumberScrollRect;

    [SerializeField]
    private Button saveButton;

    [SerializeField]
    private Button discardButton;

    [SerializeField]
    private Color highlightedColor;

    [SerializeField]
    private List<string> wordsToHighlight;

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
    /// Passt die Zeilenzahlen an den Code im Editor an.
    /// </summary>
    public void AdjustLineNumbers() {
        string temp = "";
        string[] codeLines = editor.text.Split('\n');
        for (int i = 0; i < codeLines.Length; i++) {
            temp += (i + 1) + "\n";
        }

        lineNumberText.text = temp;
    }

    public void CheckSyntaxForWholeText() {
        TMP_TextInfo textInfo = editor.textComponent.textInfo;
        TMP_WordInfo wordInfo = textInfo.wordInfo[1];
        TMP_CharacterInfo charInfo;

        for (int i = 0; i < wordInfo.characterCount; i++) {
            charInfo = textInfo.characterInfo[wordInfo.firstCharacterIndex + i];
            charInfo.color = highlightedColor;
        }
        editor.textComponent.ForceMeshUpdate();
    }

    /// <summary>
    /// Überprüft, ob das übergebene Wort hervorgehoben werden soll.
    /// Wenn ja, dann gibt es das Wort zusammen mit den nötigen Rich-Text-Tags zurück, sonst wird nur das Wort zurückgegeben
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string CheckSyntax(string text) {
        if(wordsToHighlight.Contains(text)) {
            text = "<color= #" + ColorUtility.ToHtmlStringRGB(highlightedColor) + ">" + text + "</color>";
        }
        return text;
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

    private void Update() {
        AdjustLineNumberScrollPosition();
    }

    /// <summary>
    /// Passt die derzeitige Scroll-Position des LineNumber Felds an das Editor-Textfeld an.
    /// </summary>
    private void AdjustLineNumberScrollPosition() {
        lineNumberScrollRect.verticalNormalizedPosition = 1 - editor.verticalScrollbar.value;
    }
}
