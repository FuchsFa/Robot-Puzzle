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
    private Color32 highlightedColor;

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

    private void Start() {
        StartCoroutine(CheckSyntaxForWholeText());
    }

    IEnumerator CheckSyntaxForWholeText() {
        TMP_TextInfo textInfo = editor.textComponent.textInfo;
        int currentCharacter = 0;

        Color32[] newVertexColors;
        Color32 c0 = editor.textComponent.color;

        while (true) {
            int characterCount = textInfo.characterCount;
            if (characterCount == 0) {
                yield return new WaitForSeconds(0.25f);
                continue;
            }
            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            if (textInfo.characterInfo[currentCharacter].isVisible) {
                c0 = highlightedColor;

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                editor.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            yield return new WaitForSeconds(0.05f);
        }
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
