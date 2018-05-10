using System;
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
    private Color32 keywordColor;

    private List<string> keywords;

    [SerializeField]
    private Color32 commentColor;
    [SerializeField]
    private Color32 stringColor;
    [SerializeField]
    private Color32 numberColor;

    //Der Script code des ausgewählten Roboters ohne Änderungen.
    private string oldScriptCode;

    //Wird auf true gesetzt, wenn ein comment im Skriptcode hervorgehoben wird.
    private bool singleLineComment;
    private bool multiLineComment;

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter auswählt.
    /// </summary>
    public void OnSelectRobot() {
        singleLineComment = false;
        multiLineComment = false;
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
        InitializeKeywords();
        StartCoroutine(CheckSyntaxForWholeText());
    }

    private void InitializeKeywords() {
        string[] temp = new string[] {
            "and", "break", "do", "else", "elseif", "end",
            "false", "for", "function", "goto", "if", "in",
            "local", "nil", "not", "or", "repeat", "return",
            "then", "true", "until", "while"
        };
        keywords = new List<string>(temp);
    }

    /// <summary>
    /// Geht das ganze Skript des ausgewählten Roboters Wort für Wort durch und überprüft, ob und wie es hervorgehoben werden soll.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckSyntaxForWholeText() {
        //Debug.Log("**Start Syntax Check");
        TMP_TextInfo textInfo = editor.textComponent.textInfo;

        while (true) {
            int characterCount = textInfo.characterCount;
            if (characterCount == 0) {
                yield return new WaitForSeconds(0.25f);
                continue;
            }

            int wordCount = textInfo.wordCount;
            
            for (int i = 0; i < wordCount; i++) {
                int charCount = textInfo.wordInfo[i].characterCount;
                if(charCount == 0) {
                    continue;
                }
                //Debug.Log(textInfo.wordInfo[i].GetWord());
                CheckSyntax(textInfo, textInfo.wordInfo[i]);
            }
            //Debug.Log("**End Syntax Check");
            yield return new WaitForSeconds(0.25f);
        }
    }

    /// <summary>
    /// Überprüft, ob und wie das übergebene Wort des übergebenen Texts hervorgehoben werden soll.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    private void CheckSyntax(TMP_TextInfo textInfo, TMP_WordInfo word) {
        Color32[] newVertexColors;
        Color32 c0 = editor.textComponent.color;

        if(!singleLineComment && !multiLineComment) {
            CheckForCommentStart(textInfo, word);
        }

        for (int i = 0; i < word.characterCount; i++) {
            int currentCharacter = word.firstCharacterIndex + i;
            
            if(singleLineComment || multiLineComment) {
                c0 = commentColor;
            } else if(Char.IsDigit(textInfo.characterInfo[word.firstCharacterIndex].character) && Char.IsDigit(textInfo.characterInfo[currentCharacter].character)) {
                c0 = numberColor;
            } else if(keywords.Contains(word.GetWord())) {
                c0 = keywordColor;
            } else {
                c0 = editor.textComponent.color;
            }

            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            if (textInfo.characterInfo[currentCharacter].isVisible) {
                

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                editor.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }

        if(singleLineComment || multiLineComment) {
            CheckForCommentEnd(textInfo, word);
        }
    }

    private void CheckForCommentStart(TMP_TextInfo textInfo, TMP_WordInfo word) {
        if(textInfo.characterInfo[word.firstCharacterIndex].character == '-' && textInfo.characterInfo[word.firstCharacterIndex + 1].character == '-') {
            if(textInfo.characterInfo[word.firstCharacterIndex + 2].character == '[' && textInfo.characterInfo[word.firstCharacterIndex + 3].character == '[') {
                multiLineComment = true;
            } else {
                singleLineComment = true;
            }
        }
    }

    private void CheckForCommentEnd(TMP_TextInfo textInfo, TMP_WordInfo word) {
        if(singleLineComment) {
            if(textInfo.characterInfo[word.lastCharacterIndex + 1].character == '\n') {
                singleLineComment = false;
            }
        } else {
            if (textInfo.characterInfo[word.firstCharacterIndex].character == '-' && textInfo.characterInfo[word.firstCharacterIndex + 1].character == '-') {
                if (textInfo.characterInfo[word.firstCharacterIndex - 1].character == ']' && textInfo.characterInfo[word.firstCharacterIndex - 2].character == ']') {
                    multiLineComment = false;
                }
            }
        }
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
