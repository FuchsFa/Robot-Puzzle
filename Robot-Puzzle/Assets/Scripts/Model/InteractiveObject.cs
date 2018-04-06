﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour {

    //Positions-spezifische Felder:
    public int posX;
    public int posY;
    public Vector2 direction;

    private int oldX;
    private int oldY;
    private Vector2 oldDirection;

    private int startX;
    private int startY;
    private Vector2 startDirection;

    /// <summary>
    /// Der Roboter, der dieses Objekt greift.
    /// </summary>
    public Robot grabbedBy;

    /// <summary>
    /// Gibt an, ob das Objekt bewegt(z.B. geschoben) werden kann.
    /// </summary>
    [SerializeField]private bool movable;

    public bool Movable {
        get {
            return movable;
        }

        private set {
            movable = value;
        }
    }

    /// <summary>
    /// Gibt an, ob das Objekt von einem Roboter gegriffen werden kann.
    /// </summary>
    [SerializeField]private bool grabable;

    public bool Grabable {
        get {
            return grabable;
        }

        private set {
            grabable = value;
        }
    }

    /// <summary>
    /// Setzt die Anfangsposition und -rotation auf die übergebenen Werte.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="dir"></param>
    public void SetStartingPositionAndRotation(int x, int y, Vector2 dir) {
        startX = oldX = posX = x;
        startY = oldY = posY = y;
        startDirection = oldDirection = direction = dir;
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
    public void ChangeStartingDirection(Vector2 dir) {
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
    /// Setzt Posiion und Rotation auf ihre jeweiligen Anfangswerte zurück.
    /// </summary>
    public void ResetPositionAndRotation() {
        posX = oldX = startX;
        posY = oldY = startY;
        direction = oldDirection = startDirection;
    }

    /// <summary>
    /// Dreht das Objekt um 90° gegen den Uhrzeigersinn.
    /// </summary>
    public void TurnLeft() {
        oldDirection = direction;
        if(GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            //TODO: Jetzt überprüfen, ob das getragene Objekt etwas schiebt.
        }
        direction = new Vector2(-oldDirection.y, oldDirection.x);
    }

    /// <summary>
    /// Dreht das Objekt um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnRight() {
        oldDirection = direction;
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            //TODO: Jetzt überprüfen, ob das getragene Objekt etwas schiebt.
        }
        direction = new Vector2(oldDirection.y, -oldDirection.x);
    }

    /// <summary>
    /// Bewegt das Objekt ein Feld in die angegebene Richtung.
    /// </summary>
    /// <param name="moveDir"></param>
    public void Move(Vector2 moveDir) {
        if(movable && grabbedBy == null) {
            RayCaster raycaster = GetComponent<RayCaster>();
            if(raycaster.CheckForPushableObject() != null) {
                //TODO: Jetzt das gefundene Objekt schieben.
            }
            oldX = posX;
            oldY = posY;
            posX += (int)moveDir.x;
            posY += (int)moveDir.y;
        }
    }

    /// <summary>
    /// Wird aufgerufen, wenn das Objekt gegriffen wird.
    /// Speichert den greifenden Roboter in der 'grabbedBy'  Variable.
    /// Sendet außerdem eine "OnGrab" Nachricht an das eigene gameObject, damit auch andere Skripte auf den Grab reagieren können.
    /// </summary>
    /// <param name="grabbingRobot"></param>
    public void OnGrab(Robot grabbingRobot) {
        grabbedBy = grabbingRobot;
        gameObject.SendMessage("OnGrab", SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Wird aufgerufen, wenn das Objekt losgelassen wird.
    /// Setzt die 'grabbedBy' Variable auf null zurück.
    /// Sendet außerdem eine "OnRelease" Nachricht an das eigene gameObject, damit auch andere Skripte auf den Release reagieren können.
    /// </summary>
    public void OnRelease() {
        grabbedBy = null;
        gameObject.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
    }
}
