using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class WorldObject : MonoBehaviour {

    public TextAsset scriptText;
    private string scriptCode;

    private Dictionary<string, System.Func<DynValue>> actionDictionary;

    private Script script;
    private DynValue coroutine;

    private InteractiveObject myInteractiveObject;

    [SerializeField]
    private GroundTile.TerrainType[] terrainCompatability;

    /// <summary>
    /// Initialisiert das WorldObject mit den übergebenen Parametern.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InitializeWorldObject(Vector2 dir, int x = 0, int y = 0) {
        myInteractiveObject = GetComponent<InteractiveObject>();
        myInteractiveObject.ChangeStartingPosition(x, y);
        myInteractiveObject.ChangeStartingDirection(dir);
        InitializeActionDictionary();
        InitializeScript();
    }

    /// <summary>
    /// Erstellt das Dictionary, in dem alle Aktionen stehen, die von WorldObjects ausgeführt werden können.
    /// </summary>
    private void InitializeActionDictionary() {
        actionDictionary = new Dictionary<string, System.Func<DynValue>>();
        actionDictionary.Add("turnLeft", TurnLeft);
        actionDictionary.Add("turnRight", TurnRight);
        actionDictionary.Add("walk", Walk);
        actionDictionary.Add("wait", Wait);
    }

    /// <summary>
    /// Erstellt das Script-Objekt, in dem das Akionsskript des WorldObjects gespeichert wird.
    /// Übergibt die Aktionen im actionDictionary ans Skript.
    /// </summary>
    private void InitializeScript() {
        script = new Script(CoreModules.Preset_HardSandbox);
        script.Options.DebugPrint = s => { Debug.Log(s); };//TODO: Debug-Nachrichten auf eine in-game lesbare Konsole schreiben.

        foreach (string key in actionDictionary.Keys) {
            script.Globals[key] = (System.Func<DynValue>)actionDictionary[key];
        }
        if(scriptText) {
            scriptCode = scriptText.text;
        } else {
            scriptCode = "";
        }
    }

    /// <summary>
    /// Startet eine neue Coroutine für das Skript des WorldObjects.
    /// </summary>
    public void StartLuaScript() {
        Debug.Log("Start Lua Script for " + gameObject.name);
        if(scriptCode == "") {
            return;
        }
        script.DoString(scriptCode);

        coroutine = null;
        DynValue function = script.Globals.Get("action");
        coroutine = script.CreateCoroutine(function);
    }

    /// <summary>
    /// Lässt das Skript des WorldObjects weiterlaufen nachdem es mit einem yield angehalten wurde.
    /// </summary>
    public void ResumeAction() {
        if (coroutine.Coroutine.State == CoroutineState.Dead) {
            return;
        }
        coroutine.Coroutine.Resume();
    }

    /// <summary>
    /// Überprüft, ob das WorldObject noch Aktionen in seiner Coroutine hat.
    /// </summary>
    /// <returns></returns>
    public bool HasActionsLeft() {
        if (scriptCode == "") {
            return false;
        }
        return coroutine.Coroutine.State != CoroutineState.Dead;
    }

    /// <summary>
    /// Löscht die derzeitige Instanz der Coroutine für das Skript des WorldObjects und erstellt eine danach eine neue Coroutine.
    /// </summary>
    public void RestartLuaScript() {
        coroutine = null;
        DynValue function = script.Globals.Get("action");
        coroutine = script.CreateCoroutine(function);
    }

    //Aktionen:

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
        foreach (GroundTile.TerrainType type in terrainCompatability) {
            if(type == tile.terrainType) {
                canWalk = true;
            }
        }
        return canWalk;
    }

    public DynValue Wait() {
        Debug.Log(gameObject.name + " waits.");
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }
}
