using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotDetailPanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject panel;

    //Button-Farben:
    private ColorBlock unequippedColors;
    [SerializeField] private Color32 unequippedNormal;
    [SerializeField] private Color32 unequippedHighlighted;
    [SerializeField] private Color32 unequippedPressed;
    [SerializeField] private Color32 unequippedDisabled;

    private ColorBlock equippedColors;
    [SerializeField] private Color32 equippedNormal;
    [SerializeField] private Color32 equippedHighlighted;
    [SerializeField] private Color32 equippedPressed;
    [SerializeField] private Color32 equippedDisabled;

    //Buttons:
    private Button[] robotPartButtons;
    //Tools:
    [SerializeField] private Button buttonToolNone;
    [SerializeField] private Button buttonToolGrab;
    [SerializeField] private Button buttonToolWeld;
    //Mobillity:
    [SerializeField] private Button buttonMobilityDefault;
    [SerializeField] private Button buttonMobilitySpider;
    [SerializeField] private Button buttonMobilityBoat;
    //Sensors:
    [SerializeField] private Button buttonSensorDefault;
    [SerializeField] private Button buttonSensorGround;
    [SerializeField] private Button buttonSensorScanner;

    [SerializeField]
    private Text explanationText;

    private List<RobotPart> parts;

    // Use this for initialization
    void Start () {
        panel.SetActive(false);
        robotPartButtons = new Button[] {
            buttonToolNone, buttonToolGrab, buttonToolWeld,
            buttonMobilityDefault, buttonMobilitySpider, buttonMobilityBoat,
            buttonSensorDefault, buttonSensorGround, buttonSensorScanner
        };
        unequippedColors.normalColor = unequippedNormal;
        unequippedColors.highlightedColor = unequippedHighlighted;
        unequippedColors.pressedColor = unequippedPressed;
        unequippedColors.disabledColor = unequippedDisabled;
        unequippedColors.colorMultiplier = 1;

        equippedColors.normalColor = equippedNormal;
        equippedColors.highlightedColor = equippedHighlighted;
        equippedColors.pressedColor = equippedPressed;
        equippedColors.disabledColor = equippedDisabled;
        equippedColors.colorMultiplier = 1;
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter auswählt.
    /// </summary>
    public void OnSelectRobot() {
        panel.SetActive(true);
        AdjustButtonColors();
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter abwählt.
    /// </summary>
    public void OnDeselectRobot() {
        panel.SetActive(false);
    }

    /// <summary>
    /// Passt die Farben der Buttons an, um dem Spieler zu zeigen, welche Teile der ausgewählte Roboter ausgerüstet hat.
    /// </summary>
    private void AdjustButtonColors() {
        foreach(Button btn in robotPartButtons) {
            btn.colors = unequippedColors;
        }
        parts = RobotManager.Instance.selectedRobot.GetComponent<Robot>().GetRobotPartList();
        AdjustToolButtonColors();
        AdjustOtherButtonColors();
    }

    /// <summary>
    /// Überprüft alle ausgerüsteten Tools des ausgewählten Roboters und passt die Farben der entsprechenden Buttons an.
    /// </summary>
    private void AdjustToolButtonColors() {
        foreach(RobotPart part in parts) {
            if(part is BasicArm) {
                buttonToolGrab.colors = equippedColors;
                return;
            } else if (part is WeldingTool) {
                buttonToolWeld.colors = equippedColors;
                return;
            }
        }
        buttonToolNone.colors = equippedColors;
    }

    /// <summary>
    /// Überprüft alle ausgerüsteten Teile des ausgewählten Roboters und passt die Farben der entsprechenden Buttons an.
    /// </summary>
    private void AdjustOtherButtonColors() {
        foreach (RobotPart part in parts) {
            if (part is BasicLeg) {
                buttonMobilityDefault.colors = equippedColors;
            } else if (part is BasicSensor) {
                buttonSensorDefault.colors = equippedColors;
            }
        }
    }
}
