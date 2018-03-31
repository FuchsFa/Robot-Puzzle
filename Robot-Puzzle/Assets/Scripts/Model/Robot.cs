using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class Robot {

    //Positions-spezifische Felder:
    private int posX;
    private int posY;
    private Vector2 direction;

    private int oldX;
    private int oldY;
    private Vector2 oldDirection;

    private int startX;
    private int startY;
    private Vector2 startDirection;

    //Allgemeine Felder:
    private List<RobotPart> parts;

    private Dictionary<string, System.Action> actionDictionary;
    private List<string> allowedActionNames;

    private Script script;

    //Aktions-spezifische Felder:

    //Für Grab()
    private InteractiveObject grabbedObject;

    //Für Sense()
    private InteractiveObject sensedObject;

    /// <summary>
    /// Platziert den Roboter an Stelle 0/0 mit Blick nach Süden.
    /// </summary>
    public Robot() {
        posX = oldX = startX = 0;
        posY = oldY = startY = 0;
        //Standardmäßig sehen Roboter nach Süden.
        direction = oldDirection = startDirection = new Vector2(0, -1);
        parts = new List<RobotPart>();
        InitializeActionDictionary();
        GetAllowedActionNames();
        InitializeScript();
    }

    /// <summary>
    /// Platziert den Roboter an der angegebenen Stelle mit der angegebenen Ausrichtung
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dir">Ausrichtung des Roboters</param>
    public Robot(int x, int y, Vector2 dir) {
        posX = oldX = startX = x;
        posY = oldY = startY = y;
        direction = oldDirection = startDirection = dir;
        parts = new List<RobotPart>();
        InitializeActionDictionary();
        GetAllowedActionNames();
        InitializeScript();
    }

    /// <summary>
    /// Initialisiert den Roboter mit den übergebenen Parametern.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InitializeRobot(Vector2 dir, int x = 0, int y = 0) {
        ChangeStartingPosition(x, y);
        ChangeStartingDirection(dir);
        parts = new List<RobotPart>();
        InitializeActionDictionary();
        GetAllowedActionNames();
        InitializeScript();
    }

    /// <summary>
    /// Erstellt das Dictionary, in dem alle Aktionen stehen, die der Roboter ausführen kann.
    /// </summary>
    private void InitializeActionDictionary() {
        actionDictionary = new Dictionary<string, System.Action>();
        actionDictionary.Add("turnLeft", TurnLeft);
        actionDictionary.Add("turnRight", TurnRight);
        actionDictionary.Add("grab", GrabObject);
        actionDictionary.Add("release", ReleaseObject);
        actionDictionary.Add("walk", Walk);
        actionDictionary.Add("sense", Sense);
    }

    /// <summary>
    /// Füllt die 'allowedActionNames' Liste, in der alle Aktionen stehen, die der Roboter ausführen darf.
    /// </summary>
    private void GetAllowedActionNames() {
        allowedActionNames = new List<string>();
        foreach(RobotPart part in parts) {
            allowedActionNames.AddRange(part.GetActionList());
        }
    }

    /// <summary>
    /// Erstellt das Script-Object in dem der Programmcode des Roboters gespeichert wird.
    /// Übergibt die Aktionen im actionDictionary an das Script.
    /// </summary>
    private void InitializeScript() {
        script = new Script(CoreModules.Preset_HardSandbox);

        foreach(string key in actionDictionary.Keys) {
            script.Globals[key] = (System.Action)actionDictionary[key];
        }
    }

    /// <summary>
    /// Passt die Startposition an die angegebenen Koordinaten an.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ChangeStartingPosition(int x, int y) {
        posX = oldX = startX = x;
        posY = oldY = startY = y;
    }

    /// <summary>
    /// Ändert die Startausrichtung. Sollte im Spiel nicht direkt aufgerufen werden. Stattdessen TurnStartingDirection verwenden.
    /// </summary>
    /// <param name="dir"></param>
    private void ChangeStartingDirection(Vector2 dir) {
        direction = oldDirection = startDirection = dir;
    }

    /// <summary>
    /// Dreht die Startausrichtung des Roboters um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnStartingDirection() {
        Vector2 dir = new Vector2(startDirection.y, -startDirection.x);
        ChangeStartingDirection(dir);
    }

    /// <summary>
    /// Fügt das angegebene Teil zur Teileliste des Roboters hinzu.
    /// Teile der Typen 'Tool' und 'Mobility' ersetzen bereits hinzugefügte Teile des gleichen Typs.
    /// </summary>
    /// <param name="part"></param>
    public void AddPart(RobotPart part) {
        if(part.type != RobotPart.PartType.Sensor) {
            RemovePartsOfSameType(part.type);
        }
        parts.Add(part);
        part.AddTo(this);
    }

    /// <summary>
    /// Entfernt alle Teile des übergebenen Typs aus der Teileliste des Roboters.
    /// </summary>
    /// <param name="partType"></param>
    private void RemovePartsOfSameType(RobotPart.PartType partType) {
        List<RobotPart> partsToRemove = new List<RobotPart>();
        foreach(RobotPart part in parts) {
            if(part.type == partType) {
                partsToRemove.Add(part);
            }
        }
        foreach(RobotPart part in partsToRemove) {
            RemovePart(part);
        }
    }

    /// <summary>
    /// Entfernt das angebene Teil von der Teileliste des Roboters, sofern es in dessen Teileliste ist.
    /// </summary>
    /// <param name="part"></param>
    public void RemovePart(RobotPart part) {
        if(parts.Contains(part)) {
            parts.Remove(part);
            part.RemoveFrom(this);
        } else {
            Debug.LogError("Das Teil kann nicht vom Roboter entfernt werden, weil es nicht ein Teil von dessen Teileliste ist.");
        }
        
    }

    //Aktionen:

    //Grundlegende Aktionen

    /// <summary>
    /// Dreht die zurzeitige Ausrichtung des Roboters um 90° gegen den Uhrzeigersinn.
    /// </summary>
    public void TurnLeft() {
        oldDirection = direction;
        direction = new Vector2(-oldDirection.y, oldDirection.x);
    }

    /// <summary>
    /// Dreht die zurzeitige Ausrichtung des Roboters um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnRight() {
        oldDirection = direction;
        direction = new Vector2(oldDirection.y, -oldDirection.x);
    }

    //Tool-spezifische Aktionen

    /// <summary>
    /// Wenn ein interactives Objekt vor dem Roboter liegt, wird es gegriffen,sofern noch kein anderes Objekt gehalten wird.
    /// </summary>
    public void GrabObject() {
        if (grabbedObject != null) {
            //Wenn bereits ein Objekt gegriffen wird, passiert nichts.
            return;
        }
        InteractiveObject objectToGrab = CheckForObjectToGrab();
        if (objectToGrab != null) {
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

    //Mobility-spezifische Aktionen

    /// <summary>
    /// Lässt den Roboter einen Schritt in Blickrichtung gehen.
    /// </summary>
    public void Walk() {
        //TODO: implementieren
    }

    //Sensor-spezifische Aktionen

    /// <summary>
    /// Überprüft, ob auf dem Feld vor dem Roboter ein interaktives Objekt liegt.
    /// </summary>
    public void Sense() {
        //TODO: implementieren
    }

}
