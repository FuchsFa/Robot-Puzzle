using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryPanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject panelObject;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private int[] targetNumberOfTurns;

    [SerializeField]
    private int[] targetRobotCost;

    [SerializeField]
    private int[] targetCodeLength;

    private static int maxScorePerCategory = 5;

    private int scoreSpeed;
    private int scoreCost;
    private int scoreCode;

	// Use this for initialization
	void Start () {
        scoreSpeed = 0;
        scoreCost = 0;
        scoreCode = 0;
	}

    /// <summary>
    /// Lädt das Level neu.
    /// </summary>
    public void RestartLevel() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// Lädt das Hauptmenü.
    /// </summary>
    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Wird aufgerufen, wenn alle Ziele des Levels erfüllt sind.
    /// </summary>
    public void OnVictory() {
        CalculateScore();
        ChangeScoreText();
        ShowScorePanel();
    }

    /// <summary>
    /// Berechnet den erreichten Score in den einzelnen Kategorien.
    /// </summary>
    private void CalculateScore() {
        int turns = GameStateManager.Instance.currentTurn;
        for (int i = 0; i < targetNumberOfTurns.Length; i++) {
            if(turns <= targetNumberOfTurns[i]) {
                scoreSpeed = maxScorePerCategory - i;
                break;
            }
        }
        Debug.Log("Speed Score: " + scoreSpeed);
        int cost = RobotManager.Instance.GetTotalRobotCost();
        for (int i = 0; i < targetRobotCost.Length; i++) {
            if(cost <= targetRobotCost[i]) {
                scoreCost = maxScorePerCategory - i;
                break;
            }
        }
        Debug.Log("Cost Score: " + scoreCost);
        int codeLength = RobotManager.Instance.GetTotalCodeLength();
        for (int i = 0; i < targetCodeLength.Length; i++) {
            if(codeLength <= targetCodeLength[i]) {
                scoreCode = maxScorePerCategory - i;
                break;
            }
        }
        Debug.Log("Code Score: " + scoreCode);
    }

    /// <summary>
    /// Passt den score-Tex an, damit der richtige Score angezeigt wird.
    /// </summary>
    private void ChangeScoreText() {
        string temp = "";

        temp += "Speed: \n" + GetStarText(scoreSpeed) + "\n";
        temp += "Cost: \n" + GetStarText(scoreCost) + "\n";
        temp += "Code length: \n" + GetStarText(scoreCode);

        scoreText.text = temp;
    }

    /// <summary>
    /// Gibt für den übergebenen Score einen string zurück, der für jeden erreichten Punkt einen goldenen Stern
    /// und für jeden möglichen aber nicht erreichten Punkt einen Platzhalter enthält.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    private string GetStarText(int score) {
        string temp = "";
        for (int i = 0; i < 5; i++) {
            if(score > i) {
                //TODO: Zeichen durch icons ersetzen.
                temp += "|";
            } else {
                temp += ".";
            }
        }

        return temp;
    }

    private void ShowScorePanel() {
        panelObject.SetActive(true);
    }
}
