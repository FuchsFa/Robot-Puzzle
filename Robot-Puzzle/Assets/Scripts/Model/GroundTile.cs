using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GroundTile : Tile {

    /// <summary>
    /// Die Sammlung der möglichen Terrain Types, die Tiles haben können.
    /// </summary>
    public enum TerrainType { solid, liquid };

    /// <summary>
    /// Legt fest, welche Objekte auf dem Tile liegen können und welche Beine Roboter benötigen, um auf dem Tile zu stehen.
    /// </summary>
    public TerrainType terrainType;

    /// <summary>
    /// Tags, die Sensoren benutzen können, um das Tile zu identifizieren.
    /// </summary>
    public string[] tags;

#if UNITY_EDITOR

    /// <summary>
    /// Erstellt ein neues GroundTile.
    /// </summary>
    [MenuItem("Assets/Create/GroundTile")]
    public static void CreateGroundTile() {
        string path = EditorUtility.SaveFilePanelInProject("Save Ground Tile", "New Ground Tile", "Asset", "Save Ground Tile", "Assets/Resources/Tiles");
        if(path == "") {
            return;
        }
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GroundTile>(), path);
    }
#endif
}
