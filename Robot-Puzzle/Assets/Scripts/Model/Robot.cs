using System.Text;
using System.Collections;
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
    //private InteractiveObject sensedObject;

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
        actionDictionary.Add("wait", Wait);
        actionDictionary.Add("grab", GrabObject);
        actionDictionary.Add("release", ReleaseObject);
        actionDictionary.Add("walk", Walk);
        actionDictionary.Add("sense", CheckForInteractiveObjectInFront);
        actionDictionary.Add("weld", Weld);
        actionDictionary.Add("shred", Shred);
        actionDictionary.Add("checkGround", CheckGroundTileInFront);
        actionDictionary.Add("scanSurroundings", ScanSurroundings);
    }

    /// <summary>
    /// Füllt die 'allowedActionNames' Liste, in der alle Aktionen stehen, die der Roboter ausführen darf.
    /// </summary>
    private void GetAllowedActionNames() {
        allowedActionNames = new List<string>();
        allowedActionNames.Add("turnLeft");
        allowedActionNames.Add("turnRight");
        allowedActionNames.Add("wait");
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
        script.Options.DebugPrint = s => { ConsolePanelManager.Instance.LogStringToInGameConsole(s); };

        //ActionDictionary Globals
        foreach(string key in actionDictionary.Keys) {
            script.Globals[key] = (System.Func<DynValue>)actionDictionary[key];
        }
        //Sonstige Globals
        script.Globals["move"] = (System.Func<string, DynValue>)Move;

        scriptCode = "";
    }

    /// <summary>
    /// Gibt den derzeitigen script code als string zurück.
    /// </summary>
    /// <returns></returns>
    public string GetScriptCode() {
        return scriptCode;
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
        Debug.Log("Start Lua Script for " + gameObject.name);
        if(scriptCode == "") {
            return;
        }

        try {
            script.DoString(scriptCode);
        }
        catch (MoonSharp.Interpreter.SyntaxErrorException) {
            ConsolePanelManager.Instance.LogErrorToInGameConsole("Syntax error in " + name + "'s script.");
            GameStateManager.Instance.Stop();
            return;
        }
        

        coroutine = null;
        DynValue function = script.Globals.Get("action");
        coroutine = script.CreateCoroutine(function);
        coroutine.Coroutine.AutoYieldCounter = 200;
    }

    /// <summary>
    /// Überprüft, ob der Roboter auch die benötigten Teil hat, um das Skript auszuführen.
    /// </summary>
    /// <returns></returns>
    public bool IsLuaScriptValid(string code) {
        bool valid = true;
        GetAllowedActionNames();
        string temp = RemoveSpecialCharactersAndNumbers(code);
        string[] words = temp.Split(new char[] { ' ', '\n' });
        foreach(string word in words) {
            if((actionDictionary.ContainsKey(word) || word == "move") && !allowedActionNames.Contains(word)) {
                Debug.Log("***Word '" + word + "' is not a valid part of a lua script.");
                valid = false;
            }
        }

        return valid;
    }

    /// <summary>
    /// Entfernt alle Sonderzeichen und zahlen aus einem Text.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string RemoveSpecialCharactersAndNumbers(string text) {
        StringBuilder sb = new StringBuilder();
        foreach(char c in text) {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == ' ') || (c == '\n')) {
                sb.Append(c);
            }
        }
        return sb.ToString();
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
        if(scriptCode == "") {
            return;
        }
        coroutine = null;
        DynValue function = script.Globals.Get("action");
        if(function.Type == DataType.Function || function.Type == DataType.ClrFunction) {
            coroutine = script.CreateCoroutine(function);
        }
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
        if(AlreadyHasPartOfClass(part) == null) {
            parts.Add(part);
            part.AddTo(this);
        }
    }

    /// <summary>
    /// Überprüft, ob der Roboter bereits ein Teil mit der selben Klasse(z.B. Welder oder Boat) hat.
    /// </summary>
    /// <param name="partToCheck"></param>
    /// <returns></returns>
    public RobotPart AlreadyHasPartOfClass(RobotPart partToCheck) {
        foreach(RobotPart part in parts) {
            if(partToCheck.GetType() == part.GetType()) {
                return part;
            }
        }
        return null;
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

    /// <summary>
    /// Gibt eine Liste mit allen RobotParts, die am Roboter befestigt sind, zurück.
    /// </summary>
    /// <returns></returns>
    public List<RobotPart> GetRobotPartList() {
        return parts;
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
        //Debug.Log(gameObject.name + " turns right.");
        myInteractiveObject.TurnRight();
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Lässt den Roboter eine Runde lang still stehen.
    /// </summary>
    /// <returns></returns>
    public DynValue Wait() {
        //Debug.Log(gameObject.name + " waits.");
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    //Tool-spezifische Aktionen

    /// <summary>
    /// Wenn ein interactives Objekt vor dem Roboter liegt, wird es gegriffen,sofern noch kein anderes Objekt gehalten wird.
    /// </summary>
    public DynValue GrabObject() {
        //Debug.Log(gameObject.name + " tries to grab.");
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
        //Debug.Log(gameObject.name + " releases its grabbed object.");
        if(grabbedObject != null) {
            grabbedObject.OnRelease();
            grabbedObject = null;
        }
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Versucht ein Objekt vor sich zu schweißen.
    /// </summary>
    /// <returns></returns>
    public DynValue Weld() {
        //Debug.Log(name + " versucht, etwas zu schweißen.");
        WorldObject objectToWeld = GetComponent<RayCaster>().CheckForWorldObject(GetComponent<InteractiveObject>().direction);
        if(objectToWeld != null) {
            objectToWeld.OpenForConnections(true);
        }
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Versucht ein Objekt vor sich zu zerstören.
    /// </summary>
    /// <returns></returns>
    public DynValue Shred() {
        //Debug.Log(name + " versucht, etwas zu zerstören.");
        WorldObject objectToDestroy = GetComponent<RayCaster>().CheckForWorldObject(GetComponent<InteractiveObject>().direction);
        if(objectToDestroy != null) {
            WorldObjectManager.Instance.RemoveWorldObject(objectToDestroy.gameObject);
        }
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    //Mobility-spezifische Aktionen

    /// <summary>
    /// Lässt den Roboter einen Schritt in Blickrichtung gehen.
    /// </summary>
    public DynValue Walk() {
        //Debug.Log(gameObject.name + " walks.");
        myInteractiveObject.Move(myInteractiveObject.direction);
        return DynValue.NewYieldReq(new DynValue[] { coroutine });
    }

    /// <summary>
    /// Läss den Roboter einen Schritt in die angegebene Richtung gehen.
    /// </summary>
    /// <param name="directionName"></param>
    /// <returns></returns>
    public DynValue Move(string directionName) {
        //Debug.Log(gameObject.name + " moves " + directionName);
        Vector2 direction = new Vector2();
        switch (directionName) {
            case "north":
                direction = new Vector2(0, 1);
                break;
            case "east":
                direction = new Vector2(1, 0);
                break;
            case "south":
                direction = new Vector2(0, -1);
                break;
            case "west":
                direction = new Vector2(-1, 0);
                break;
            default:
                break;
        }
        myInteractiveObject.Move(direction);
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
            } else if(part is SpiderLeg) {
                SpiderLeg leg = part as SpiderLeg;
                if(leg.TerrainCompatability == tile.terrainType) {
                    canWalk = true;
                    break;
                }
            } else if (part is Boat) {
                Boat leg = part as Boat;
                if (leg.TerrainCompatability == tile.terrainType) {
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
    public DynValue CheckForInteractiveObjectInFront() {
        //Debug.Log(gameObject.name + " activates its sensor.");
        Table sensedData = new Table(script);

        InteractiveObject temp = GetComponent<RayCaster>().CheckForInteractiveObject(myInteractiveObject.direction);
        if(temp != null) {
            //Wenn ein Interaktives Objekt vor dem Roboter steht wird es analysiert.
            sensedData["x"] = temp.posX;
            sensedData["y"] = temp.posY;
            sensedData["direction"] = temp.GetFacingAngle(temp.direction);
            sensedData["movable"] = temp.Movable;
            sensedData["grabable"] = temp.Grabable;
            sensedData["color"] = "none";
            if(temp.grabbedBy != null) {
                sensedData["grabbedBy"] = temp.grabbedBy.name;
            } else {
                sensedData["grabbedBy"] = "none";
            }

            Robot tempBot = temp.GetComponent<Robot>();
            if (tempBot != null) {
                sensedData["type"] = "Robot";
            }

            WorldObject tempObj = temp.GetComponent<WorldObject>();
            if (tempObj != null) {
                sensedData["type"] = tempObj.objectType;
                sensedData["color"] = tempObj.objectColor.ToString();
            }
        } else if(GetComponent<RayCaster>().CheckForCollisionsInDirection(myInteractiveObject.direction)) {
            //Wenn kein Interaktives Objekt vor dem Roboter steht, er aber trotzdem kollidieren würde, muss eine Wand vor ihm sein.
            sensedData["type"] = "Wall";
            sensedData["movable"] = false;
            sensedData["grabable"] = false;
            sensedData["grabbedBy"] = "none";
            sensedData["direction"] = -1;
            sensedData["color"] = "none";
            sensedData["x"] = myInteractiveObject.posX + myInteractiveObject.direction.x;
            sensedData["y"] = myInteractiveObject.posY + myInteractiveObject.direction.y;
        } else {
            //Wenn auch keine Kollision stattfinden würde, ist das Tile vor dem Roboter leer.
            sensedData["type"] = "Nothing";
            sensedData["movable"] = false;
            sensedData["grabable"] = false;
            sensedData["grabbedBy"] = "none";
            sensedData["direction"] = -1;
            sensedData["color"] = "none";
            sensedData["x"] = myInteractiveObject.posX + myInteractiveObject.direction.x;
            sensedData["y"] = myInteractiveObject.posY + myInteractiveObject.direction.y;
        }

        return DynValue.NewTable(sensedData);
    }

    /// <summary>
    /// Überprüft das Groundtile vor dem Roboter.
    /// </summary>
    /// <returns></returns>
    public DynValue CheckGroundTileInFront() {
        //Debug.Log(name + " checks the Ground Tile in its front.");
        Table sensedData = new Table(script);

        int x = myInteractiveObject.posX + (int)myInteractiveObject.direction.x;
        int y = myInteractiveObject.posY + (int)myInteractiveObject.direction.y;
        GroundTile groundTile = RobotManager.Instance.tilemap.GetTile<GroundTile>(new Vector3Int(x, y, 0));
        if(groundTile != null) {
            sensedData["terrainType"] = groundTile.terrainType.ToString();
            Table tagTable = new Table(script);
            foreach(string tag in groundTile.tags) {
                tagTable.Append(DynValue.NewString(tag));
            }
            sensedData["tags"] = tagTable;
        }

        return DynValue.NewTable(sensedData);
    }

    /// <summary>
    /// Gibt eine Liste mit den Positionen und Typen aller (Roboter und) WorldObjects auf der Karte zurück.
    /// </summary>
    /// <returns></returns>
    public DynValue ScanSurroundings() {
        //Debug.Log(name + " scans its surroundings.");
        Table sensedData = new Table(script);

        List<WorldObject> temp = WorldObjectManager.Instance.GetWorldObjectList();
        foreach(WorldObject worldObject in temp) {
            Table worldObjectInfo = new Table(script);
            InteractiveObject interactiveObject = worldObject.GetComponent<InteractiveObject>();
            worldObjectInfo["type"] = worldObject.objectType;
            worldObjectInfo["movable"] = interactiveObject.Movable;
            worldObjectInfo["grabable"] = interactiveObject.Grabable;
            if(interactiveObject.grabbedBy != null) {
                worldObjectInfo["grabbedBy"] = interactiveObject.grabbedBy.name;
            } else {
                worldObjectInfo["grabbedBy"] = "none";
            }
            worldObjectInfo["direction"] = interactiveObject.GetFacingAngle(interactiveObject.direction);
            worldObjectInfo["x"] = interactiveObject.posX;
            worldObjectInfo["y"] = interactiveObject.posY;
            sensedData.Append(DynValue.NewTable(worldObjectInfo));
        }

        return DynValue.NewTable(sensedData);
    }

}
