using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : RobotPart {
    private const int myCost = 150;

    /// <summary>
    /// Erstellt den Scanner und fügt ihn dem angegebenen Roboter hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public Scanner(Robot robot) {
        type = PartType.Sensor;
        robot.AddPart(this);
        cost = myCost;
    }

    /// <summary>
    /// Default Constructor.
    /// </summary>
    public Scanner() {
        type = PartType.Sensor;
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
        return new List<string>(new string[] { "scanSurroundings" });
    }

    /// <summary>
    /// Setzt die 'attachedTo'-Variable auf null zurück
    /// </summary>
    /// <param name="robot"></param>
    public override void RemoveFrom(Robot robot) {
        attachedTo = null;
    }
}
