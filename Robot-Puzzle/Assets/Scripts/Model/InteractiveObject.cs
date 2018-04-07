using System.Collections;
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
        gameObject.transform.position = new Vector3(posX + 0.5f, posY + 0.5f);
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
            InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
            grabbedObject.gameObject.transform.position = transform.position + new Vector3(direction.x, direction.y);
            //TODO: Jetzt überprüfen, ob das getragene Objekt etwas schiebt.
        }
        direction = new Vector2(-oldDirection.y, oldDirection.x);
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
    }

    /// <summary>
    /// Dreht das Objekt um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnRight() {
        oldDirection = direction;
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
            grabbedObject.gameObject.transform.position = transform.position + new Vector3(direction.x, direction.y);
            //TODO: Jetzt überprüfen, ob das getragene Objekt etwas schiebt.
        }
        direction = new Vector2(oldDirection.y, -oldDirection.x);
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
    }

    /// <summary>
    /// Gibt die Blickrichtung als float zurück. Süden is 0, Norden ist 180.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    private float GetFacingAngle(Vector2 dir) {
        if (dir.y == 1) {
            return 180;
        }
        else if (dir.y == -1) {
            return 0;
        }
        else if (dir.x == 1) {
            return 90;
        }
        else if (dir.x == -1) {
            return -90;
        }
        return 0;
    }

    /// <summary>
    /// Bewegt das Objekt ein Feld in die angegebene Richtung.
    /// </summary>
    /// <param name="moveDir"></param>
    public void Move(Vector2 moveDir) {
        Debug.Log(gameObject.name + " bewegt sich.");
        if(movable && grabbedBy == null) {
            RayCaster raycaster = GetComponent<RayCaster>();
            if(raycaster.CheckForPushableObject() != null) {
                Push(raycaster.CheckForPushableObject(), moveDir);
            }
            if(raycaster.CheckForCollisionsInDirection(moveDir)) {
                Debug.LogError(gameObject.name + " kann sich nicht in die angegebene Richtung bewegen, weil es zur Kollision kommen würde.");
                return;
            }
            oldX = posX;
            oldY = posY;
            posX += (int)moveDir.x;
            posY += (int)moveDir.y;
            if(GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
                InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
                grabbedObject.gameObject.transform.position = new Vector3(grabbedObject.posX + moveDir.x + 0.5f, grabbedObject.posY + moveDir.y + 0.5f);
            }
            gameObject.transform.position = new Vector3(posX + 0.5f, posY + 0.5f);
            Debug.Log(gameObject.name + " neue Position: " + posX + "/" + posY);
        }
    }

    /// <summary>
    /// Bewegt das angegebene Objekt in die angegebene Richtung.
    /// </summary>
    /// <param name="target"></param>
    private void Push(InteractiveObject target, Vector2 dir) {
        if(target.movable && target.grabbedBy == null) {
            Debug.Log(gameObject.name + " schiebt " + target.gameObject.name + ".");
            target.Move(dir);
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
