using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot {

    private int posX;
    private int posY;
    private Vector2 direction;

    private int oldX;
    private int oldY;
    private Vector2 oldDirection;

    private int startX;
    private int startY;
    private Vector2 startDirection;

    private List<RobotPart> parts;

    /// <summary>
    /// Platziert den Roboter an Stelle 0/0 mit Blick nach Süden.
    /// </summary>
    public Robot() {
        posX = oldX = startX = 0;
        posY = oldY = startY = 0;
        //Standardmäßig sehen Roboter nach Süden.
        direction = oldDirection = startDirection = new Vector2(0, -1);
        parts = new List<RobotPart>();
    }

    /// <summary>
    /// Platziert den Roboter an der angegebenen Stelle mit der angegebenen Ausrichtung
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dir">Ausrichtung des Roboters</param>
    public Robot(int x, int y, Vector2 dir) {
        posX = oldX = startX = x;
        posY = oldY = startY = y;
        direction = oldDirection = startDirection = dir;
        parts = new List<RobotPart>();
    }

    /// <summary>
    /// Passt die Startposition an die angegebenen Koordinaten an.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ChangeStartingPosition(int x, int y) {
        posX = oldX = startX = x;
        posY = oldY = startY = y;
    }

    /// <summary>
    /// Ändert die Startausrichtung. Sollte im Spiel nicht direkt aufgerufen werden. Stattdessen TurnStartingDirection verwenden.
    /// </summary>
    /// <param name="dir"></param>
    private void ChangeStartingDirection(Vector2 dir) {
        direction = oldDirection = startDirection = dir;
    }

    /// <summary>
    /// Dreht die Startausrichtung des Roboters um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnStartingDirection() {
        Vector2 dir = new Vector2(startDirection.y, -startDirection.x);
        ChangeStartingDirection(dir);
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

    /// <summary>
    /// Dreht die zurzeitige Ausrichtung des Roboters um 90° gegen den Uhrzeigersinn.
    /// </summary>
    public void TurnLeft() {
        oldDirection = direction;
        direction = new Vector2(-oldDirection.y, oldDirection.x);
    }

    /// <summary>
    /// Dreht die zurzeitige Ausrichtung des Roboters um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnRight() {
        oldDirection = direction;
        direction = new Vector2(oldDirection.y, -oldDirection.x);
    }
	
}
