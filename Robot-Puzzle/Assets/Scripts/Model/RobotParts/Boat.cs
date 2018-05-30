using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : RobotPart {
    private const int myCost = 100;

    GroundTile.TerrainType terrainCompatability;

    public GroundTile.TerrainType TerrainCompatability {
        get {
            return terrainCompatability;
        }

        protected set {
            terrainCompatability = value;
        }
    }

    /// <summary>
    /// Erstellt das Boot und fügt es gleich zum Roboter hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public Boat(Robot robot) {
        type = PartType.Mobility;
        terrainCompatability = GroundTile.TerrainType.liquid;
        robot.AddPart(this);
        cost = myCost;
    }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Boat() {
        type = PartType.Mobility;
        terrainCompatability = GroundTile.TerrainType.liquid;
        cost = myCost;
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
    public override List<string> GetActionList() {
        return new List<string>(new string[] { "walk" });
    }

    /// <summary>
    /// Setzt die 'attachedTo'-Variable auf null zurück
    /// </summary>
    /// <param name="robot"></param>
    public override void RemoveFrom(Robot robot) {
        attachedTo = null;
    }
}
