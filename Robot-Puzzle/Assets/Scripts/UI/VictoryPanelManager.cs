using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryPanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject panelObject;

    [SerializeField]
    private TextMeshProUGUI scoreText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnVictory() {
        CalculateScore();
        ChangeScoreText();
        ShowScorePanel();
    }

    private void CalculateScore() {

    }

    private void ChangeScoreText() {

    }

    private void ShowScorePanel() {

    }
}
