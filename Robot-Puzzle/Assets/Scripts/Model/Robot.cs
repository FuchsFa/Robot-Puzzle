﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class Robot : MonoBehaviour {

    //Allgemeine Felder:
    private List<RobotPart> parts;

    private string scriptCode;

    private Dictionary<string, System.Func<DynValue>> actionDictionary;
    private List<string> allowedActionNames;

    private Script script;
    private DynValue coroutine;

    private InteractiveObject myInteractiveObject;

    //Aktions-spezifische Felder:

    //Für Grab()
    private InteractiveObject grabbedObject;

    public InteractiveObject GrabbedObject {
        get {
            return grabbedObject;
        }

        private set {
            grabbedObject = value;
        }
    }

    //Für Sense()
    private InteractiveObject sensedObject;

    /// <summary>
    /// Initialisiert den Roboter mit den übergebenen Parametern.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InitializeRobot(Vector2 dir, int x = 0, int y = 0) {
        myInteractiveObject = GetComponent<InteractiveObject>();
        myInteractiveObject.ChangeStartingPosition(x, y);
        myInteractiveObject.ChangeStartingDirection(dir);
        parts = new List<RobotPart>();
        InitializeActionDictionary();
        GetAllowedActionNames();
        InitializeScript();
    }

    /// <summary>
    /// Setzt den Roboter auf seine Anfangsposition und -drehung zurück und startet sein Skript neu.
    /// </summary>
    public void ResetRobot() {
        myInteractiveObject.ResetPositionAndRotation();
        RestartLuaScript();
    }

    /// <summary>
    /// Erstellt das Dictionary, in dem alle Aktionen stehen, die der Roboter ausführen kann.
    /// </summary>
    private void InitializeActionDictionary() {
        actionDictionary = new Dictionary<string, System.Func<DynValue>>();
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
        script.Options.DebugPrint = s => { Debug.Log(s); };//TODO: Debug-Nachrichten auf eine in-game lesbare Konsole schreiben.

        foreach(string key in actionDictionary.Keys) {
            script.Globals[key] = (System.Func<DynValue>)actionDictionary[key];
        }
        scriptCode = "";
    }

    /// <summary>
    /// Ändert das Skript des Roboters.
    /// </summary>
    /// <param name="code"></param>
    public void ChangeScriptCode(string code) {
        scriptCode = code;
        Debug.Log(gameObject.name + "'s script has been changed to: " + code);
    }

    /// <summary>
    /// Startet eine neue Coroutine für das Skript des Roboters.
    /// </summary>
    public void StartLuaScript() {
        Debug.Log("Start Lua Script");
        if(scriptCode == "") {
            return;
        }
        script.DoString(scriptCode);

        coroutine = null;
        DynValue function = script.Globals.Get("action");
        coroutine = script.CreateCoroutine(function);
    }

    /// <summary>
    /// Lässt das Skript des Roboters weiterlaufen nachdem es mit einem yield angehalten wurde.
    /// </summary>
    public void ResumeAction() {
        if(coroutine.Coroutine.State == CoroutineState.Dead) {
            return;
        }
        coroutine.Coroutine.Resume();
    }

    /// <summary>
    /// Überprüft, ob der Roboter noch Aktionen in seiner Coroutine hat.
    /// </summary>
    /// <returns></returns>
    public bool HasActionsLeft() {
        if(scriptCode == "") {
            return false;
        }
        return coroutine.Coroutine.State != CoroutineState.Dead;
    }

    /// <summary>
    /// Löscht die derzeitige Instanz der Coroutine für das Skript des Roboters und erstellt eine danach eine neue Coroutine.
    /// </summary>
    public void RestartLuaScript() {
        coroutine = null;
        DynValue function = script.Globals.Get("action");
        coroutine = script.CreateCoroutine(function);
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
    public DynValue TurnLeft() {
        Debug.Log(gameObject.name + " turns left.");
        myInteractiveObject.TurnLeft();
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Dreht die zurzeitige Ausrichtung des Roboters um 90° im Uhrzeigersinn.
    /// </summary>
    public DynValue TurnRight() {
        Debug.Log(gameObject.name + " turns right.");
        myInteractiveObject.TurnRight();
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    //Tool-spezifische Aktionen

    /// <summary>
    /// Wenn ein interactives Objekt vor dem Roboter liegt, wird es gegriffen,sofern noch kein anderes Objekt gehalten wird.
    /// </summary>
    public DynValue GrabObject() {
        Debug.Log(gameObject.name + " tries to grab.");
        if (grabbedObject != null) {
            //Wenn bereits ein Objekt gegriffen wird, passiert nichts.
            return DynValue.NewYieldReq(new DynValue[] { coroutine });
        }
        InteractiveObject objectToGrab = CheckForObjectToGrab();
        if (objectToGrab != null) {
            grabbedObject = objectToGrab;
            objectToGrab.OnGrab(this);
        }
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Überprüft, ob vor dem Roboter ein Objekt liegt, das gegriffen werden kann.
    /// </summary>
    /// <returns>Gibt das Objekt zurück, welches gegriffen werden soll. Gibt null zurück, wenn kein greifbares Objekt vorhanden ist.</returns>
    private InteractiveObject CheckForObjectToGrab() {
        return GetComponent<RayCaster>().CheckForGrabableObject();
    }

    /// <summary>
    /// Lässt das derzeit gehaltene Objekt los.
    /// </summary>
    public DynValue ReleaseObject() {
        Debug.Log(gameObject.name + " releases its grabbed object.");
        grabbedObject.OnRelease();
        grabbedObject = null;
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    //Mobility-spezifische Aktionen

    /// <summary>
    /// Lässt den Roboter einen Schritt in Blickrichtung gehen.
    /// </summary>
    public DynValue Walk() {
        Debug.Log(gameObject.name + " walks.");
        myInteractiveObject.Move(myInteractiveObject.direction);
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Überprüft alle RobotParts auf ihre TerrainCompatability.
    /// Wenn die TerrainTypen mit dem TerrainTypen des übergebenen tiles übereinstimmen, wird true, sonst wird false zurückgegeben.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool CanWalkOn(GroundTile tile) {
        bool canWalk = false;
        foreach(RobotPart part in parts) {
            if(part is BasicLeg) {
                BasicLeg leg = part as BasicLeg;
                if(leg.TerrainCompatability == tile.terrainType) {
                    canWalk = true;
                    break;
                }
            }
        }
        return canWalk;
    }

    //Sensor-spezifische Aktionen

    /// <summary>
    /// Überprüft, ob auf dem Feld vor dem Roboter ein interaktives Objekt liegt.
    /// </summary>
    public DynValue Sense() {
        Debug.Log(gameObject.name + " activates its sensor.");
        //TODO: implementieren
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

}
