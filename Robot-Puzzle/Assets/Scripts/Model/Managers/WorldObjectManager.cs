using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldObjectManager : MonoBehaviour {

    public static WorldObjectManager Instance { get; protected set; }

    [SerializeField]
    private Dictionary<string, GameObject> prefabDictionary;

    [SerializeField]
    private GameObject groupObjectPrefab;

    [SerializeField]
    private Tilemap groundTilemap;

    private GameStateManager gameStateManager;

    private List<GameObject> worldObjects;

    private List<GameObject> startWorldObjects;

    private List<GameObject> worldObjectGroups;

    // Use this for initialization
    void Start () {
        Instance = this;
        gameStateManager = GetComponent<GameStateManager>();
        worldObjects = new List<GameObject>();
        worldObjectGroups = new List<GameObject>();
        InitializePrefabDictionary();
        InitializeStartingWorldObjects();
	}

    /// <summary>
    /// Sucht alle WorldObjects, die zu Beginn des Spiels auf dem Spielfeld sind und fügt sie in die worldObjects-Liste ein.
    /// </summary>
    private void InitializeStartingWorldObjects() {
        startWorldObjects = new List<GameObject>();
        WorldObject[] startingWorldObjects = FindObjectsOfType<WorldObject>();
        foreach(WorldObject obj in startingWorldObjects) {
            worldObjects.Add(obj.gameObject);
            startWorldObjects.Add(obj.gameObject);
            Vector2 startingDirection = obj.GetComponent<InteractiveObject>().direction;
            int startX = (int)(obj.transform.position.x - 0.5f);
            int startY = (int)(obj.transform.position.y - 0.5f);
            obj.InitializeWorldObject(startingDirection, startX, startY);
        }
    }

    /// <summary>
    /// Lädt alle Prefabs im Prefabs/WorldObjects Ordner ins prefabDictionary.
    /// </summary>
    private void InitializePrefabDictionary() {
        prefabDictionary = new Dictionary<string, GameObject>();
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/WorldObjects");
        foreach(GameObject prefab in prefabs) {
            prefabDictionary.Add(prefab.name, prefab);
            Debug.Log("WorldObjectManager -- Das Objekt '" + prefab.name + "' wurde ins prefabDictionary geladen.");
        }
    }

    /// <summary>
    /// Gibt ein Prefab des übergebenen Typs zurück, sofern dieser im prefabDictionary ist.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetPrefabFromDictionary(string type) {
        if(!prefabDictionary.ContainsKey(type)) {
            Debug.LogError("Es befindet sich kein Objekt vom Typ '" + type + "' im Prefab Dictionary");
            return null;
        } else {
            return prefabDictionary[type];
        }
    }

    /// <summary>
    /// Erstellt ein neues WorldObject des angegebenen Typs an der angegebenen Stelle.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public GameObject CreateWorldObject(string type, int x, int y) {
        if(!prefabDictionary.ContainsKey(type)) {
            Debug.LogError("WorldObjectManager -- Das Objekt vom Typ '" + type + "' kann nicht erstellt werden, weil der Typ nicht existiert.");
            return null;
        }
        GameObject worldObject = Instantiate(prefabDictionary[type]);
        WorldObject obj = worldObject.GetComponent<WorldObject>();
        obj.InitializeWorldObject(new Vector2(0, -1), x, y);
        worldObject.transform.position = new Vector3(x + 0.5f, y + 0.5f);
        worldObjects.Add(worldObject);
        worldObject.name = worldObject.GetComponent<WorldObject>().GetObjectType() + " ID: " + worldObjects.Count;
        worldObject.transform.SetParent(this.transform);
        return worldObject;
    }

    /// <summary>
    /// Entfernt das angegebene Objekt aus der worldObjects-Liste und zerstört es.
    /// </summary>
    /// <param name="worldObject"></param>
    public void RemoveWorldObject(GameObject worldObject) {
        if(worldObjects.Contains(worldObject) && !startWorldObjects.Contains(worldObject)) {
            worldObjects.Remove(worldObject);
            Destroy(worldObject);
        }
    }

    /// <summary>
    /// Entfernt alle WorldObjects und alle WorldObjectGroups;
    /// </summary>
    public void RemoveAllWorldObjects() {
        List<GameObject> tempObjects = new List<GameObject>(worldObjects);
        List<GameObject> tempGroups = new List<GameObject>(worldObjectGroups);
        foreach(GameObject group in tempGroups) {
            RemoveWorldObjectGroup(group.GetComponent<WorldObjectGroup>());
        }
        foreach(GameObject obj in tempObjects) {
            RemoveWorldObject(obj);
        }
    }

    /// <summary>
    /// Gibt eine Liste mit allen WorldObjects zurück.
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetWorldObjectList() {
        List<WorldObject> worldObjectList = new List<WorldObject>();
        foreach(GameObject obj in worldObjects) {
            worldObjectList.Add(obj.GetComponent<WorldObject>());
        }
        return worldObjectList;
    }

    /// <summary>
    /// Ruft die StartLuaScript Funktion jedes WorldObjects auf, damit diese ihre Aktionen ausführen können.
    /// Wird aufgerufen, wenn der Spieler auf 'Play' drückt.
    /// </summary>
    public void StartWorldObjectScripts() {
        foreach(GameObject worldObject in worldObjects) {
            WorldObject obj = worldObject.GetComponent<WorldObject>();
            obj.StartLuaScript();
        }
    }

    /// <summary>
    /// Passt die Animationsvariablen von WorldObjects und WorldObjectGroups an.
    /// </summary>
    public void AdjustWorldObjectAnimationVariables() {
        foreach (GameObject group in worldObjectGroups) {
            group.GetComponent<InteractiveObject>().AdjustAnimationVariables();
        }
        foreach (GameObject worldObject in worldObjects) {
            worldObject.GetComponent<InteractiveObject>().AdjustAnimationVariables();
        }
    }

    /// <summary>
    /// Überprüft, ob jedes WorldObject auch auf dem Tile stehen darf, auf dem es gerade steht.
    /// </summary>
    public void CheckForWorldObjectTerrainCompatability() {
        foreach (GameObject worldObject in worldObjects) {
            WorldObject obj = worldObject.GetComponent<WorldObject>();
            int x = worldObject.GetComponent<InteractiveObject>().posX;
            int y = worldObject.GetComponent<InteractiveObject>().posY;
            GroundTile tile = groundTilemap.GetTile<GroundTile>(new Vector3Int(x, y, 0));
            if (!obj.CanWalkOn(tile)) {
                Debug.LogError(worldObject.name + " darf nicht auf dem Tile stehen, auf dem es gerade steht!");
                ConsolePanelManager.Instance.LogErrorToInGameConsole(worldObject.name + " can't stand on " + tile.terrainType.ToString() + " ground.");
                gameStateManager.Stop();
                return;
            }
        }
    }

    /// <summary>
    /// Führt das Lua-Skript jedes WorldObjects weiter aus, bis es wieder yield zurückgibt.
    /// Wird zu Beginn jeder Runde aufgerufen.
    /// </summary>
    public void PerformWorldObjectActionsForTurn() {
        foreach (GameObject worldObject in worldObjects) {
            WorldObject obj = worldObject.GetComponent<WorldObject>();
            if(obj.HasActionsLeft()) {
                obj.ResumeAction();
            }
        }
    }

    /// <summary>
    /// Animiert die GameObjects der WorldObjects und WorldObjectGroups, damit diese graduell an ihre derzeitige Position und Rotation angepasst werden.
    /// </summary>
    /// <param name="percentage"></param>
    public void AdjustWorldObjects(float percentage) {
        foreach(GameObject group in worldObjectGroups) {
            group.GetComponent<InteractiveObject>().AdjustGameObject(percentage);
        }
        foreach(GameObject worldObject in worldObjects) {
            if(worldObject.GetComponent<WorldObject>().myGroup) {
                continue;
            }
            worldObject.GetComponent<InteractiveObject>().AdjustGameObject(percentage);
        }
    }

    /// <summary>
    /// Versucht alle WorldObjects, die connective sind, miteinander zu verbinden. Danach wird die Konnektivität aller WorldObjects zurückgesetzt.
    /// </summary>
    public void ConnectAllAvailableWorldObjects() {
        List<GameObject> connectedThisTurn = new List<GameObject>();
        foreach(GameObject obj in worldObjects) {
            if(obj.GetComponent<WorldObject>().IsConnective()) {
                //Debug.Log("obj = " + obj.name);
                foreach(GameObject objectToConnectTo in worldObjects) {
                    //Debug.Log("objectToConnectTo = " + objectToConnectTo.name);
                    if(objectToConnectTo != obj && objectToConnectTo.GetComponent<WorldObject>().IsConnective()) {
                        if(obj.GetComponent<WorldObject>().myGroup != null && objectToConnectTo.GetComponent<WorldObject>().myGroup != null && obj.GetComponent<WorldObject>().myGroup == objectToConnectTo.GetComponent<WorldObject>().myGroup) {
                            Debug.Log(obj.name + " und " + objectToConnectTo.name + " sind in der gleichen ObjectGroup");
                            continue;
                        }
                        ConnectWorldObjects(obj.GetComponent<WorldObject>(), objectToConnectTo.GetComponent<WorldObject>());
                        connectedThisTurn.Add(obj);
                    }
                }
            }
        }
        foreach(GameObject obj in connectedThisTurn) {
            obj.GetComponent<WorldObject>().OpenForConnections(false);
        }
    }

    /// <summary>
    /// Verbindet die beiden übergebenen WorldObjects, falls möglich
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public void ConnectWorldObjects(WorldObject a, WorldObject b) {
        if(Vector3.Distance(a.transform.position, b.transform.position) > 1.1f) {
            Debug.Log("Die WorldObjects '" + a.gameObject.name + "' und '" + b.gameObject.name + "' sind zu weit auseinander, um verbunden zu werden.");
            return;
        }
        if (a.myGroup == null && b.myGroup == null) {
            WorldObjectGroup group = Instantiate(groupObjectPrefab).GetComponent<WorldObjectGroup>();
            group.transform.SetParent(this.transform);
            group.GetComponent<InteractiveObject>().SetStartingPositionAndRotation(a.GetComponent<InteractiveObject>().posX, a.GetComponent<InteractiveObject>().posY, new Vector2(0, -1));
            group.transform.position = new Vector3(a.GetComponent<InteractiveObject>().posX + 0.5f, a.GetComponent<InteractiveObject>().posY + 0.5f);
            worldObjectGroups.Add(group.gameObject);
            group.AddObjectToGroup(a);
        }
        a.AttemptToConnectWorldObject(b);
        b.AttemptToConnectWorldObject(a);
    }

    /// <summary>
    /// Entfernt die übergebene WorldObjectGroup aus worldObjectGroups und zerstört sie.
    /// </summary>
    /// <param name="group"></param>
    public void RemoveWorldObjectGroup(WorldObjectGroup group) {
        worldObjectGroups.Remove(group.gameObject);
        Destroy(group.gameObject);
    }
}
