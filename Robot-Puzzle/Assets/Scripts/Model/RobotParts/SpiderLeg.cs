using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLeg : RobotPart {

    /// <summary>
    /// Erstellt die Beine und fügt sie gleich zum Roboter hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public SpiderLeg(Robot robot) {
        type = PartType.Mobility;
        robot.AddPart(this);
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SpiderLeg() {
        type = PartType.Mobility;
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
        return new List<string>(new string[] { "shred" });
    }

    /// <summary>
    /// Setzt die 'attachedTo'-Variable auf null zurück
    /// </summary>
    /// <param name="robot"></param>
    public override void RemoveFrom(Robot robot) {
        attachedTo = null;
    }
}
