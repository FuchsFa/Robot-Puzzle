using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class WorldObject : MonoBehaviour {

    [SerializeField]
    private string objectType;

    public TextAsset scriptText;
    private string scriptCode;

    private Dictionary<string, System.Func<DynValue>> actionDictionary;

    private Script script;
    private DynValue coroutine;

    private InteractiveObject myInteractiveObject;

    public WorldObjectGroup myGroup;

    /// <summary>
    /// Reihenfolge der Verbindungen: [Front, Rechts, Rückseite, Links]
    /// </summary>
    private WorldObject[] connectedWorldObjects;

    //Solange das WorldObject connective ist, verbindet es sich automatisch mit benachbarten WorldObjects, die ebenfalls connective sind.
    private bool connective;

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
        connectedWorldObjects = new WorldObject[] { null, null, null, null };
        InitializeActionDictionary();
        InitializeScript();
        myGroup = null;
        connective = false;
    }

    /// <summary>
    /// Erstellt das Dictionary, in dem alle Aktionen stehen, die von WorldObjects ausgeführt werden können.
    /// </summary>
    private void InitializeActionDictionary() {
        actionDictionary = new Dictionary<string, System.Func<DynValue>> {
            { "turnLeft", TurnLeft },
            { "turnRight", TurnRight },
            { "walk", Walk },
            { "wait", Wait }
        };
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

    /// <summary>
    /// Macht das WorldObject connective oder nicht mehr connective, je nach dem Wert der übergeben wird.
    /// </summary>
    public void OpenForConnections(bool value = true) {
        connective = value;
    }

    /// <summary>
    /// Versucht, das übergebene WorldObject mit diesem zu verbinden.
    /// </summary>
    /// <param name="other"></param>
    public void AttemptToConnectWorldObject(WorldObject other) {
        int relativeX = other.gameObject.GetComponent<InteractiveObject>().posX - GetComponent<InteractiveObject>().posX;
        int relativeY = other.gameObject.GetComponent<InteractiveObject>().posY - GetComponent<InteractiveObject>().posY;
        Vector2 relativePosition = new Vector2(relativeX, relativeY);
        int slot = GetConnectionSlotForPosition(relativePosition);
        if(connectedWorldObjects[slot] != null) {
            Debug.Log("Das angegebene WorldObject kann nicht mit diesem verbunden werden, weil der passende Slot schon belegt ist.");
            return;
        } else {
            connectedWorldObjects[slot] = other;
            if(myGroup != null) {
                if(other.myGroup != null && other.myGroup != myGroup) {
                    myGroup.MergeGroups(other.myGroup);
                } else if(other.myGroup == null) {
                    myGroup.AddObjectToGroup(other);
                }
            }
        }
    }

    /// <summary>
    /// Entfernt die Verbindung zum angegebenen WorldObject
    /// </summary>
    /// <param name="other"></param>
    public void RemoveConnectionToWorldObject(WorldObject other) {
        for (int i = 0; i < connectedWorldObjects.Length; i++) {
            if(connectedWorldObjects[i] == other) {
                connectedWorldObjects[i] = null;
                myGroup.RemoveObjectFromGroup(other);
                break;
            }
        }
    }

    /// <summary>
    /// Überprüft die Verbindungen des WorldObjects und gibt zurück, in welcher absoluten Richtung diese Verbindungen liegen.
    /// Die Reihenfolge im zurückgegebenen Array ist: [Norden, Osten, Süden, Westen]
    /// </summary>
    /// <returns></returns>
    public bool[] GetAbsoluteConnectionDirections() {
        bool[] temp = new bool[] { false, false, false, false };
        foreach (WorldObject obj in connectedWorldObjects) {
            if(obj == null) {
                continue;
            }
            int relativeX = obj.gameObject.GetComponent<InteractiveObject>().posX - GetComponent<InteractiveObject>().posX;
            int relativeY = obj.gameObject.GetComponent<InteractiveObject>().posY - GetComponent<InteractiveObject>().posY;
            if(relativeY > 0) {
                temp[0] = true;
            } else if(relativeY < 0) {
                temp[2] = true;
            } else if(relativeX > 0) {
                temp[1] = true;
            } else if(relativeX < 0) {
                temp[3] = true;
            }
        }
        return temp;
    }

    /// <summary>
    /// Gibt den Objekttypen als string zurück.
    /// </summary>
    /// <returns></returns>
    public string GetObjectType() {
        return objectType;
    }

    /// <summary>
    /// Gibt an, ob das WorldObject mit anderen Objekten verbunden werden kann.
    /// </summary>
    /// <returns></returns>
    public bool IsConnective() {
        return connective;
    }

    /// <summary>
    /// Findet alle WorldObjects, die mit diesem WorldObject zusammenhängen. Egal ob direkt oder indirekt. (Gibt auch sich selbst zurück)
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetAllConnectedWorldObjects() {
        List<WorldObject> objects = new List<WorldObject>();
        objects.Add(this);
        if(myGroup != null) {
            foreach(WorldObject worldObject in myGroup.objects) {
                objects.Add(worldObject);
            }
        }
        return objects;
    }

    /// <summary>
    /// Gibt die direkt verbundenen WorldObjects zurück.
    /// </summary>
    /// <returns></returns>
    public WorldObject[] GetConnectedWorldObjects() {
        return connectedWorldObjects;
    }

    /// <summary>
    /// Gibt den Index für das connectedWorldObject array zurück, den ein WorldObject an der übergebenen Stelle einnehmen würde.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private int GetConnectionSlotForPosition(Vector2 pos) {
        if(pos == GetComponent<InteractiveObject>().direction) {
            //Front
            return 0;
        } else if(pos == (GetComponent<InteractiveObject>().direction * -1)) {
            //Rückseite
            return 2;
        } else if(pos == new Vector2(GetComponent<InteractiveObject>().direction.y, -1 * GetComponent<InteractiveObject>().direction.x)) {
            //Rechts
            return 1;
        } else if (pos == new Vector2(-1 * GetComponent<InteractiveObject>().direction.y, GetComponent<InteractiveObject>().direction.x)) {
            //Links
            return 3;
        } else {
            //Kann nicht verbunden werden.
            return -1;
        }
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

    /// <summary>
    /// Lässt das WorldObject 1 Runde lang still stehen.
    /// </summary>
    /// <returns></returns>
    public DynValue Wait() {
        Debug.Log(gameObject.name + " waits.");
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }
}
