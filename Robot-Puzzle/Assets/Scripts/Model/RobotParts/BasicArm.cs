using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArm : RobotPart {

    /// <summary>
    /// Erstellt den neuen Arm und fügt ihn gleich zum angegebenen Roboter hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public BasicArm(Robot robot) {
        type = PartType.Tool;
        robot.AddPart(this);
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public BasicArm() {
        type = PartType.Tool;
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
    public override Dictionary<string, Action> GetActionList() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Setzt die 'attachedTo'-Variable auf null zurück
    /// </summary>
    /// <param name="robot"></param>
    public override void RemoveFrom(Robot robot) {
        attachedTo = null;
    }

}
