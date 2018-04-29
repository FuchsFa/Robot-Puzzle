using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotDetailPanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Text explanationText;

	// Use this for initialization
	void Start () {
        panel.SetActive(false);
	}

    public void OnSelectRobot() {
        panel.SetActive(true);
    }

    public void OnDeselectRobot() {
        panel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
