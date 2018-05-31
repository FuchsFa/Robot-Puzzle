using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotDetailPanelManager : MonoBehaviour {

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private Text robotNameText;

    [SerializeField]
    private Text totalCostDisplaytext;

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
    [SerializeField] private Button buttonToolGrab;
    [SerializeField] private Button buttonToolWeld;
    [SerializeField] private Button buttonToolShredder;
    //Mobillity:
    [SerializeField] private Button buttonMobilityDefault;
    [SerializeField] private Button buttonMobilitySpider;
    [SerializeField] private Button buttonMobilityBoat;
    //Sensors:
    [SerializeField] private Button buttonSensorDefault;
    [SerializeField] private Button buttonSensorGround;
    [SerializeField] private Button buttonSensorScanner;

    [SerializeField]
    private Text explanationTextField;
    private Dictionary<string, TextAsset> explanationTexts;

    private List<RobotPart> parts;

    // Use this for initialization
    void Start () {
        panel.SetActive(false);
        robotPartButtons = new Button[] {
            buttonToolGrab, buttonToolWeld, buttonToolShredder,
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
        LoadExplanationTexts();
    }

    /// <summary>
    /// Lädt alle TextAssets aus dem RobotPartDescriptions Ordner uns speichert sie in einem Dictionary.
    /// </summary>
    private void LoadExplanationTexts() {
        explanationTexts = new Dictionary<string, TextAsset>();
        TextAsset[] texts = Resources.LoadAll<TextAsset>("Texts/RobotPartDescriptions");
        foreach(TextAsset t in texts) {
            explanationTexts.Add(t.name, t);
        }
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler einen Roboter auswählt.
    /// </summary>
    public void OnSelectRobot() {
        panel.SetActive(true);
        robotNameText.text = RobotManager.Instance.selectedRobot.name;
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
        foreach (RobotPart part in parts) {
            if (part is BasicArm) {
                buttonToolGrab.colors = equippedColors;
            } else if (part is WeldingTool) {
                buttonToolWeld.colors = equippedColors;
            } else if (part is ShreddingTool) {
                buttonToolShredder.colors = equippedColors;
            } else if (part is BasicLeg) {
                buttonMobilityDefault.colors = equippedColors;
            } else if (part is SpiderLeg) {
                buttonMobilitySpider.colors = equippedColors;
            } else if (part is Boat) {
                buttonMobilityBoat.colors = equippedColors;
            } else if (part is BasicSensor) {
                buttonSensorDefault.colors = equippedColors;
            } else if (part is GroundSensor) {
                buttonSensorGround.colors = equippedColors;
            } else if (part is Scanner) {
                buttonSensorScanner.colors = equippedColors;
            }
        }
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler auf einen Button klickt, der zu einem Part gehört.
    /// Fügt das Teil dann zum derzeitig ausgewählten Roboter hinzu und passt die Buttons an.
    /// </summary>
    /// <param name="partName"></param>
    public void PartButtonClick(string partName) {
        RobotPart partToAdd = CreatePartWithName(partName);
        AddPartToSelectedRobot(partToAdd);
        AdjustButtonColors();
    }

    /// <summary>
    /// Erstellt ein neues Teil mit den übergebenen Namen.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private RobotPart CreatePartWithName(string name) {
        switch (name) {
            case "BasicArm":
                return new BasicArm();
            case "WeldingTool":
                return new WeldingTool();
            case "ShreddingTool":
                return new ShreddingTool();
            case "BasicLeg":
                return new BasicLeg();
            case "SpiderLeg":
                return new SpiderLeg();
            case "Boat":
                return new Boat();
            case "BasicSensor":
                return new BasicSensor();
            case "GroundSensor":
                return new GroundSensor();
            case "Scanner":
                return new Scanner();
            default:
                break;
        }
        Debug.LogError("Es gibt keinen RobotPart mit dem Namen '" + name + "'.");
        return null;
    }

    /// <summary>
    /// Lädt den zum übergebenen Teil-Namen passenden Text und zeigt ihn im explanationText-Feld an.
    /// </summary>
    /// <param name="partName"></param>
    public void ShowExplanationTextForPart(string partName) {
        if(!explanationTexts.ContainsKey(partName)) {
            explanationTextField.text = "???";
            return;
        }
        string text = explanationTexts[partName].text;
        explanationTextField.text = text;
    }

    /// <summary>
    /// Fügt das übergebene Teil zum derzeitig ausgewählten Roboter hinzu.
    /// </summary>
    /// <param name="part"></param>
    private void AddPartToSelectedRobot(RobotPart part) {
        RobotManager.Instance.AddPartToRobot(RobotManager.Instance.selectedRobot, part);
        totalCostDisplaytext.text = "Total Cost:\n<color=yellow>" + RobotManager.Instance.GetTotalRobotCost() + "$</color>";
    }
}
