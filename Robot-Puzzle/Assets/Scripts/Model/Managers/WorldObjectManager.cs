using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour {

    [SerializeField]
    private Dictionary<string, GameObject> prefabDictionary;

    private GameStateManager gameStateManager;

    private List<GameObject> worldObjects;

    // Use this for initialization
    void Start () {
        gameStateManager = GetComponent<GameStateManager>();
        worldObjects = new List<GameObject>();
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
    public void CreateWorldObject(string type, int x, int y) {
        if(!prefabDictionary.ContainsKey(type)) {
            Debug.LogError("WorldObjectManager -- Das Objekt vom Typ '" + type + "' kann nicht erstellt werden, weil der Typ nicht existiert.");
            return;
        }
        GameObject worldObject = Instantiate(prefabDictionary[type]);
        WorldObject obj = worldObject.GetComponent<WorldObject>();
        obj.InitializeWorldObject(new Vector2(0, -1), x, y);
        worldObject.transform.position = new Vector3(x + 0.5f, y + 0.5f);
        worldObjects.Add(worldObject);
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
    /// Passt die Animationsvariablen an.
    /// Führt das Lua-Skript jedes WorldObjects weiter aus, bis es wieder yield zurückgibt.
    /// Wird zu Beginn jeder Runde aufgerufen.
    /// </summary>
    public void PerformWorldObjectActionsForTurn() {
        foreach(GameObject worldObject in worldObjects) {
            worldObject.GetComponent<InteractiveObject>().AdjustAnimationVariables();
        }
        foreach (GameObject worldObject in worldObjects) {
            WorldObject obj = worldObject.GetComponent<WorldObject>();
            if(obj.HasActionsLeft()) {
                obj.ResumeAction();
            }
        }
    }

    /// <summary>
    /// Animiert die GameObjects der Roboter, damit diese graduell an ihre derzeitige Position und Rotation angepasst werden.
    /// </summary>
    /// <param name="percentage"></param>
    public void AdjustWorldObjects(float percentage) {
        foreach(GameObject worldObject in worldObjects) {
            worldObject.GetComponent<InteractiveObject>().AdjustGameObject(percentage);
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
        a.AttemptToConnectWorldObject(b);
        b.AttemptToConnectWorldObject(a);
    }
}
