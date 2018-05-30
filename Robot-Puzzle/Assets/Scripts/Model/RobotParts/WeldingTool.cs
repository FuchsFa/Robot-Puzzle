using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeldingTool : RobotPart {
    private const int myCost = 75;

    /// <summary>
    /// Erstellt das Schweißgerät und fügt es gleich zum Roboter hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public WeldingTool(Robot robot) {
        type = PartType.Tool;
        robot.AddPart(this);
        cost = myCost;
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public WeldingTool() {
        type = PartType.Tool;
        cost = myCost;
    }

    /// <summary>
    /// Speichert den Roboter, zu dem dieses Teil hinzugefügt wird in der 'attachedTo'-Variable.
    /// </summary>
    /// <param name="robot"></param>
    public override void AddTo(Robot robot) {
        attachedTo = robot;
    }

    /// <summary>
    /// Gibt ein Dictionary zurück mit allen Funktionen, die ein Roboter durch dieses Teil ausführen kann.
    /// </summary>
    /// <returns></returns>
    public override List<string> GetActionList() {
        return new List<string>(new string[] { "weld" });
    }

    /// <summary>
    /// Setzt die 'attachedTo'-Variable auf null zurück
    /// </summary>
    /// <param name="robot"></param>
    public override void RemoveFrom(Robot robot) {
        attachedTo = null;
    }
}
