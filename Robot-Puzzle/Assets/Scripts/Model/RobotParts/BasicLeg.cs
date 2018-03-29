using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLeg : RobotPart {

    //TODO: TerrainType hinzufügen

    /// <summary>
    /// Erstellt das Bein und fügt es dem angegebenen Roboer hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public BasicLeg(Robot robot) {
        type = PartType.Mobility;
        robot.AddPart(this);
    }

    /// <summary>
    /// Default Constructor.
    /// </summary>
    public BasicLeg() {
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
