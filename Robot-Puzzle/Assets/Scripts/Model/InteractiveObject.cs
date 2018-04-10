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

    //Wenn dieses Objekt gegriffen wird, geben die folgenden Felder die Position relativ zum greifenden Objekt an.
    private int relativeX;
    private int relativeY;
    private Vector2 relativeDirection;

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
        Vector2 temp = oldDirection;
        oldDirection = direction;
        direction = new Vector2(-oldDirection.y, oldDirection.x);
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
            //TODO: Jetzt überprüfen, ob das getragene Objekt etwas schiebt.
            InteractiveObject pushableObject = grabbedObject.gameObject.GetComponent<RayCaster>().CheckForPushableObject(direction);
            if(pushableObject != null) {
                grabbedObject.Push(pushableObject, direction);
            }
            if (grabbedObject.GetComponent<RayCaster>().CheckForCollisionsInDirection(direction)) {
                Debug.LogError(gameObject.name + " kann sich nicht nach links drehen, weil sein getragenes Objekt kollidieren würde.");
                direction = oldDirection;
                oldDirection = temp;
                return;
            }
            grabbedObject.AdjustRelativePosition("Left");
            grabbedObject.MoveToRelativePosition();
        }
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
    }

    /// <summary>
    /// Dreht das Objekt um 90° im Uhrzeigersinn.
    /// </summary>
    public void TurnRight() {
        Vector2 temp = oldDirection;
        oldDirection = direction;
        direction = new Vector2(oldDirection.y, -oldDirection.x);
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
            
            //TODO: Jetzt überprüfen, ob das getragene Objekt etwas schiebt.
            InteractiveObject pushableObject = grabbedObject.gameObject.GetComponent<RayCaster>().CheckForPushableObject(direction);
            if (pushableObject != null) {
                grabbedObject.Push(pushableObject, direction);
            }
            if (grabbedObject.GetComponent<RayCaster>().CheckForCollisionsInDirection(direction)) {
                Debug.LogError(gameObject.name + " kann sich nicht nach rechts drehen, weil sein getragenes Objekt kollidieren würde.");
                direction = oldDirection;
                oldDirection = temp;
                return;
            }
            grabbedObject.AdjustRelativePosition("Right");
            grabbedObject.MoveToRelativePosition();
        }
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
    }

    /// <summary>
    /// Passt die relative Position an das Objekt, welches dieses Objekt greift, an.
    /// Wird nach jeder Drehung aufgerufen.
    /// </summary>
    /// <param name="turnDir"></param>
    public void AdjustRelativePosition(string turnDir) {
        if(turnDir == "Right") {
            int temp = relativeX;
            relativeX = relativeY;
            relativeY = -temp;
        } else if(turnDir == "Left") {
            int temp = relativeX;
            relativeX = -relativeY;
            relativeY = temp;
        }
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
            InteractiveObject interactiveObject = raycaster.CheckForPushableObject(direction);
            if (interactiveObject != null) {
                Push(interactiveObject, moveDir);
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
                grabbedObject.MoveToRelativePosition();
            }
            gameObject.transform.position = new Vector3(posX + 0.5f, posY + 0.5f);
            Debug.Log(gameObject.name + " neue Position: " + posX + "/" + posY);
        }
    }

    /// <summary>
    /// Bewegt das Objekt zu seiner relativen Posiion und passt seine Drehung an.
    /// </summary>
    public void MoveToRelativePosition() {
        oldX = posX;
        oldY = posY;
        posX = grabbedBy.GetComponent<InteractiveObject>().posX + relativeX;
        posY = grabbedBy.GetComponent<InteractiveObject>().posY + relativeY;
        oldDirection = direction;
        direction = grabbedBy.GetComponent<InteractiveObject>().direction + relativeDirection;
        gameObject.transform.position = new Vector3(posX + 0.5f, posY + 0.5f);
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
        Debug.Log(gameObject.name + " moved to relative position: X" + oldX + "->" + posX + "; Y" + oldY + "->" + posY);
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
    /// Speichert den greifenden Roboter in der 'grabbedBy'  Variable. Speichert die relative Position.
    /// </summary>
    /// <param name="grabbingRobot"></param>
    public void OnGrab(Robot grabbingRobot) {
        Debug.Log(grabbingRobot.gameObject.name + " greift " + gameObject.name);
        grabbedBy = grabbingRobot;
        relativeX = posX - grabbedBy.GetComponent<InteractiveObject>().posX;
        relativeY = posY - grabbedBy.GetComponent<InteractiveObject>().posY;
        relativeDirection = direction - grabbedBy.GetComponent<InteractiveObject>().direction;
    }

    /// <summary>
    /// Wird aufgerufen, wenn das Objekt losgelassen wird.
    /// Setzt die 'grabbedBy' Variable auf null zurück.
    /// </summary>
    public void OnRelease() {
        grabbedBy = null;
    }

    /// <summary>
    /// Passt das GameObject an, damit es graduell zwischen der alten un der neuen Position animiert.
    /// </summary>
    /// <param name="percentage"></param>
    public void AdjustGameObject(float percentage) {
        if(percentage > 1) {
            percentage = 1;
        }
        if(oldX != posX && oldY != posY) {
            //Wenn beide Koordinaten unterschiedlich sind, heisst das, dass das Objekt getragen wird und der träger sich gedreht hat. Das Objekt muss also auf einer Kurve bewegt werden.
            //Kontrollpunkte zur Bestimmung der Kurve festlegen:
            int controlX = oldX + relativeX;
            int controlY = oldY + relativeY;
            //Punkt auf der Kurve bestimmen:
            float curveX = (((1 - percentage) * (1 - percentage)) * oldX) + (2 * percentage * (1 - percentage) * controlX) + ((percentage * percentage) * posX) + 0.5f;
            float curveY = (((1 - percentage) * (1 - percentage)) * oldY) + (2 * percentage * (1 - percentage) * controlY) + ((percentage * percentage) * posY) + 0.5f;
            gameObject.transform.position = new Vector3(curveX, curveY);
        } else if(oldX != posX || oldY != posY) {
            gameObject.transform.position = Vector2.Lerp(new Vector2(oldX, oldY), new Vector2(posX, posY), percentage) + new Vector2(0.5f, 0.5f);
        }
        if(oldDirection != direction) {
            Quaternion currentRotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
            Quaternion oldRotation = Quaternion.AngleAxis(GetFacingAngle(oldDirection), Vector3.forward);
            gameObject.transform.rotation = Quaternion.Lerp(oldRotation, currentRotation, percentage);
        }
    }

    /// <summary>
    /// Passt die Positions- und Rotationsvariablen an, damit das GameObject richtig animiert wird.
    /// </summary>
    public void AdjustAnimationVariables() {
        oldDirection = direction;
        oldX = posX;
        oldY = posY;
    }
}
