﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLeg : RobotPart {

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
    /// Erstellt das Bein und fügt es dem angegebenen Roboer hinzu.
    /// </summary>
    /// <param name="robot"></param>
    public BasicLeg(Robot robot) {
        type = PartType.Mobility;
        terrainCompatability = GroundTile.TerrainType.solid;
        robot.AddPart(this);
    }

    /// <summary>
    /// Default Constructor.
    /// </summary>
    public BasicLeg() {
        type = PartType.Mobility;
        terrainCompatability = GroundTile.TerrainType.solid;
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
