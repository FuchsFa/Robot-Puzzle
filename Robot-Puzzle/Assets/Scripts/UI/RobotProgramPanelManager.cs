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
    private int commentStartIndex;

    //Wird auf true gesetzt, wenn ein string im Skriptcode hervorgehoben wird.
    private bool highlightString;
    private int stringStartIndex;

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter auswählt.
    /// </summary>
    public void OnSelectRobot() {
        singleLineComment = false;
        multiLineComment = false;
        highlightString = false;
        oldScriptCode = RobotManager.Instance.selectedRobot.GetComponent<Robot>().GetScriptCode();
        editor.text = oldScriptCode;
        StartCoroutine(CheckSyntaxForWholeText());
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
        while (true) {
            TMP_TextInfo textInfo = editor.textComponent.textInfo;
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
            singleLineComment = false;
            multiLineComment = false;
            highlightString = false;
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
        
        if(!singleLineComment && !multiLineComment) {
            CheckForCommentStart(textInfo, word);
        }

        if(!highlightString && !singleLineComment && !multiLineComment) {
            CheckForStringStart(textInfo, word);
        }

        HighlightWord(textInfo, word);

        if(highlightString) {
            CheckForStringEnd(textInfo, word);
        }

        if(singleLineComment || multiLineComment) {
            CheckForCommentEnd(textInfo, word);
        }
    }

    /// <summary>
    /// Geht das Wort Buchstabe für Buchstabe durch und färbt sie ein.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    private void HighlightWord(TMP_TextInfo textInfo, TMP_WordInfo word) {

        for (int i = 0; i < word.characterCount; i++) {
            int currentCharacter = word.firstCharacterIndex + i;

            HighlightCharacter(textInfo, word, currentCharacter);
        }
    }

    /// <summary>
    /// Wählt die passende Farbe und hebt das Symbol hervor.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    /// <param name="currentCharacterIndex"></param>
    private void HighlightCharacter(TMP_TextInfo textInfo, TMP_WordInfo word, int currentCharacterIndex) {
        Color32[] newVertexColors;
        Color32 highlightColor = editor.textComponent.color;

        if ((singleLineComment || multiLineComment) && !highlightString) {
            highlightColor = commentColor;
        } else if (highlightString) {
            highlightColor = stringColor;
        } else if (Char.IsDigit(textInfo.characterInfo[word.firstCharacterIndex].character) && Char.IsDigit(textInfo.characterInfo[currentCharacterIndex].character)) {
            highlightColor = numberColor;
        } else if (keywords.Contains(word.GetWord())) {
            highlightColor = keywordColor;
        } else {
            highlightColor = editor.textComponent.color;
        }

        int materialIndex = textInfo.characterInfo[currentCharacterIndex].materialReferenceIndex;
        newVertexColors = textInfo.meshInfo[materialIndex].colors32;

        int vertexIndex = textInfo.characterInfo[currentCharacterIndex].vertexIndex;

        if (textInfo.characterInfo[currentCharacterIndex].isVisible) {

            newVertexColors[vertexIndex + 0] = highlightColor;
            newVertexColors[vertexIndex + 1] = highlightColor;
            newVertexColors[vertexIndex + 2] = highlightColor;
            newVertexColors[vertexIndex + 3] = highlightColor;

            editor.textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }

    /// <summary>
    /// Überprüft, ob das übergebene Wort einen Kommentar beginnt.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    private void CheckForCommentStart(TMP_TextInfo textInfo, TMP_WordInfo word) {
        if(textInfo.characterInfo[word.firstCharacterIndex].character == '-' && textInfo.characterInfo[word.firstCharacterIndex + 1].character == '-') {
            if(textInfo.characterInfo[word.firstCharacterIndex + 2].character == '[' && textInfo.characterInfo[word.firstCharacterIndex + 3].character == '[') {
                multiLineComment = true;
                HighlightCharacter(textInfo, word, word.firstCharacterIndex + 2);
                HighlightCharacter(textInfo, word, word.firstCharacterIndex + 3);
            } else {
                singleLineComment = true;
            }
            commentStartIndex = word.firstCharacterIndex;
        }
    }

    /// <summary>
    /// Überprüft, ob das übergebene Wort einen Kommentar abschließt.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    private void CheckForCommentEnd(TMP_TextInfo textInfo, TMP_WordInfo word) {
        if(singleLineComment) {
            if(textInfo.characterInfo[word.lastCharacterIndex + 1].character == '\n') {
                HighlightAllCommentCharacters(textInfo, word, word.lastCharacterIndex);
                singleLineComment = false;
            }
        } else {
            if (textInfo.characterInfo[word.firstCharacterIndex].character == '-' && textInfo.characterInfo[word.firstCharacterIndex + 1].character == '-') {
                if (textInfo.characterInfo[word.firstCharacterIndex - 1].character == ']' && textInfo.characterInfo[word.firstCharacterIndex - 2].character == ']') {
                    HighlightCharacter(textInfo, word, word.firstCharacterIndex - 1);
                    HighlightCharacter(textInfo, word, word.firstCharacterIndex - 2);
                    HighlightAllCommentCharacters(textInfo, word, word.lastCharacterIndex);
                    multiLineComment = false;
                }
            }
        }
    }

    /// <summary>
    /// Hebt alle Symbole in einem Kommentar hervor.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    /// <param name="commentEndIndex"></param>
    private void HighlightAllCommentCharacters(TMP_TextInfo textInfo, TMP_WordInfo word, int commentEndIndex) {
        for (int i = commentStartIndex; i < commentEndIndex; i++) {
            HighlightCharacter(textInfo, word, i);
        }
    }

    /// <summary>
    /// Überprüft, ob das übergebene Wort einen String beginnt.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    private void CheckForStringStart(TMP_TextInfo textInfo, TMP_WordInfo word) {
        if(word.firstCharacterIndex == 0) {
            return;
        }
        if (textInfo.characterInfo[word.firstCharacterIndex - 1].character == '"') {
            highlightString = true;
            HighlightCharacter(textInfo, word, word.firstCharacterIndex - 1);
        }
    }

    /// <summary>
    /// Überprüft, ob das übergebene Wort einen String abschließt.
    /// </summary>
    /// <param name="textInfo"></param>
    /// <param name="word"></param>
    private void CheckForStringEnd(TMP_TextInfo textInfo, TMP_WordInfo word) {
        if (textInfo.characterInfo[word.lastCharacterIndex + 1].character == '"') {
            HighlightCharacter(textInfo, word, word.lastCharacterIndex + 1);
            highlightString = false;
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
        /*if(editor.isFocused && Input.GetKeyDown(KeyCode.Tab)) {
            Debug.Log("Tab pressed");
        }*/
    }

    /// <summary>
    /// Passt die derzeitige Scroll-Position des LineNumber Felds an das Editor-Textfeld an.
    /// </summary>
    private void AdjustLineNumberScrollPosition() {
        lineNumberScrollRect.verticalNormalizedPosition = 1 - editor.verticalScrollbar.value;
    }
}
