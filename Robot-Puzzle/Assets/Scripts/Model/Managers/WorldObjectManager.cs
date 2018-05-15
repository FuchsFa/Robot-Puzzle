using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour {

    public static WorldObjectManager Instance { get; protected set; }

    [SerializeField]
    private Dictionary<string, GameObject> prefabDictionary;

    [SerializeField]
    private GameObject groupObjectPrefab;

    private GameStateManager gameStateManager;

    private List<GameObject> worldObjects;

    private List<GameObject> worldObjectGroups;

    // Use this for initialization
    void Start () {
        Instance = this;
        gameStateManager = GetComponent<GameStateManager>();
        worldObjects = new List<GameObject>();
        worldObjectGroups = new List<GameObject>();
        InitializePrefabDictionary();
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
        worldObject.transform.SetParent(this.transform);
        return worldObject;
    }

    /// <summary>
    /// Entfernt das angegebene Objekt aus der worldObjects-Liste und zerstört es.
    /// </summary>
    /// <param name="worldObject"></param>
    public void RemoveWorldObject(GameObject worldObject) {
        if(worldObjects.Contains(worldObject)) {
            worldObjects.Remove(worldObject);
            Destroy(worldObject);
        }
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
                foreach(GameObject objectToConnectTo in worldObjects) {
                    if(objectToConnectTo != obj && objectToConnectTo.GetComponent<WorldObject>().IsConnective()) {
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
        if(Vector3.Distance(a.transform.position, b.transform.position) > 1) {
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
        Destroy(group);
    }
}
