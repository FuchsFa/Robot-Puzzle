using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArm : RobotPart {
    private InteractiveObject grabbedObject;

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

    /// <summary>
    /// Wenn ein interactives Objekt vor dem Roboter liegt, wird es gegriffen,sofern noch kein anderes Objekt gehalten wird.
    /// </summary>
    public void GrabObject() {
        if (grabbedObject != null) {
            //Wenn bereits ein Objekt gegriffen wird, passiert nichts.
            return;
        }
        InteractiveObject objectToGrab = CheckForObjectToGrab();
        if(objectToGrab != null) {
            grabbedObject = objectToGrab;
        }
    }

    /// <summary>
    /// Überprüft, ob vor dem Roboter ein Objekt liegt, das gegriffen werden kann.
    /// </summary>
    /// <returns>Gibt das Objekt zurück, welches gegriffen werden soll. Gibt null zurück, wenn kein greifbares Objekt vorhanden ist.</returns>
    private InteractiveObject CheckForObjectToGrab() {
        //TODO: implementieren
        return null;
    }

    /// <summary>
    /// Lässt das derzeit gehaltene Objekt los.
    /// </summary>
    public void ReleaseObject() {
        grabbedObject = null;
    }

}
