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
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
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
        gameObject.transform.position = new Vector3(posX + 0.5f, posY + 0.5f);
        gameObject.transform.rotation = Quaternion.AngleAxis(GetFacingAngle(direction), Vector3.forward);
    }

    /// <summary>
    /// Überprüft, ob das Objekt bereit ist, von einem Goal entfernt zu werden.
    /// </summary>
    /// <returns></returns>
    public bool IsReadyForOutput() {
        if (grabbedBy == null) {
            if(GetComponent<WorldObject>() && GetComponent<WorldObject>().myGroup) {
                return GetComponent<WorldObject>().myGroup.GetComponent<InteractiveObject>().IsReadyForOutput();
            } else {
                return true;
            }
        }
        else {
            return false;
        }
    }

    /// <summary>
    /// Dreht das Objekt um 90° gegen den Uhrzeigersinn.
    /// </summary>
    public void TurnLeft() {
        Vector2 temp = oldDirection;
        oldDirection = direction;
        direction = new Vector2(-oldDirection.y, oldDirection.x);
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            List<InteractiveObject> grabbedObjects = new List<InteractiveObject>();
            InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
            grabbedObjects.Add(grabbedObject);
            if(grabbedObject.GetComponent<WorldObjectGroup>()) {
                foreach(WorldObject obj in grabbedObject.GetComponent<WorldObjectGroup>().objects) {
                    grabbedObjects.Add(obj.GetComponent<InteractiveObject>());
                }
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
            List<InteractiveObject> grabbedObjects = new List<InteractiveObject>();
            InteractiveObject grabbedObject = GetComponent<Robot>().GrabbedObject;
            grabbedObjects.Add(grabbedObject);
            if (grabbedObject.GetComponent<WorldObjectGroup>()) {
                foreach (WorldObject obj in grabbedObject.GetComponent<WorldObjectGroup>().objects) {
                    grabbedObjects.Add(obj.GetComponent<InteractiveObject>());
                }
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
    public float GetFacingAngle(Vector2 dir) {
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
            if(GetComponent<RayCaster>()) {
                RayCaster raycaster = GetComponent<RayCaster>();
                InteractiveObject interactiveObject = raycaster.CheckForPushableObject(direction);
                if (interactiveObject != null) {
                    Push(interactiveObject, moveDir);
                }
                if (raycaster.CheckForCollisionsInDirection(moveDir)) {
                    Debug.LogError(gameObject.name + " kann sich nicht in die angegebene Richtung bewegen, weil es zur Kollision kommen würde.");
                    return;
                }
            }
            oldX = posX;
            oldY = posY;
            posX += (int)moveDir.x;
            posY += (int)moveDir.y;
            if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
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
        grabbedBy = grabbingRobot;
        relativeX = posX - grabbedBy.GetComponent<InteractiveObject>().posX;
        relativeY = posY - grabbedBy.GetComponent<InteractiveObject>().posY;
        relativeDirection = direction - grabbedBy.GetComponent<InteractiveObject>().direction;
        Debug.Log(grabbingRobot.gameObject.name + " greift " + gameObject.name + " Relative Position: " + relativeX + "/" + relativeY);
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
        if(GetComponent<WorldObject>() && GetComponent<WorldObject>().myGroup != null) {
            //Wenn das Objekt ein WorldObject is und Teil einer WorldObjectGroup ist, übernimmt die Gruppe die Bewegung.
            return;
        }
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
        if((oldX != posX || oldY != posY || oldDirection != direction) && GetComponent<WorldObjectGroup>()) {
            //Wenn das Objekt eine WorldObjectGroup ist, müssen die einzelnen Teile davon auf Pushes und Kollisionen überprüft werden.
            CheckWorldObjectGroupCollision();
        }
    }

    /// <summary>
    /// Überprüft die einzelnen Teile einer WorldObjectGroup auf Pushes und Kollisionen.
    /// </summary>
    private void CheckWorldObjectGroupCollision() {
        foreach (WorldObject worldObject in GetComponent<WorldObjectGroup>().objects) {
            Collider2D collider = worldObject.GetComponent<Collider2D>();
            RayCaster raycaster = worldObject.GetComponent<RayCaster>();
            Collider2D[] detectedCollider = new Collider2D[1];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.layerMask = raycaster.collisionMask;
            contactFilter.useLayerMask = true;

            /*GameObject test = new GameObject();
            test.layer = LayerMask.NameToLayer("Goals");
            Debug.Log("!!!!!!!!!" + contactFilter.IsFilteringLayerMask(test));*/

            collider.OverlapCollider(contactFilter, detectedCollider);
            if(detectedCollider[0] != null && detectedCollider[0].GetComponent<InteractiveObject>()) {
                Debug.Log("*" + worldObject.gameObject.name + " Overlap mit " + detectedCollider[0].name);
                InteractiveObject interactiveObject = detectedCollider[0].GetComponent<InteractiveObject>();
                Vector2 dir = detectedCollider[0].transform.position - worldObject.transform.position;
                float angleBetweenObjects = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //Debug.Log("**Winkel dazwischen: " + angleBetweenObjects);
                Debug.DrawLine(detectedCollider[0].transform.position, worldObject.transform.position, Color.green);
                Vector2 pushDirection = GetDirectionVectorForRoundedAngle(angleBetweenObjects + 90); //Es werden 90° addiert, weil angleBetweenObjects einen anderen Standard-Winkel hat als der Rest des Spiels.
                if (CanPushObject(worldObject.GetComponent<InteractiveObject>(), interactiveObject, pushDirection)) {
                    worldObject.GetComponent<InteractiveObject>().Push(interactiveObject, pushDirection);
                } else if (interactiveObject.GetComponent<RayCaster>().CheckForCollisionsInDirection(pushDirection)) {
                    Debug.LogError(gameObject.name + " kann sich nicht in die angegebene Richtung bewegen, weil es zur Kollision kommen würde.");
                }
                
            }
        }
    }

    /// <summary>
    /// Überprüf, ob das erste übergebene WorldObject das Zweite schieben darf.
    /// </summary>
    /// <param name="pusher"></param>
    /// <param name="target"></param>
    /// <param name="pushDirection"></param>
    /// <returns></returns>
    private bool CanPushObject(InteractiveObject pusher, InteractiveObject target, Vector2 pushDirection) {
        if(!target.Movable) {
            Debug.Log("***" + pusher.name + " kann " + target.name + " nicht schieben, weil " + target.name + " nicht bewegbar ist.");
            return false;
        }
        if(target.GetComponent<RayCaster>().CheckForCollisionsInDirection(pushDirection)) {
            Debug.LogError("***" + target.name + " kann sich nicht in die angegebene Richtung bewegen, weil es zur Kollision kommen würde.");
            return false;
        }
        if(Vector2.Distance(pusher.transform.position, target.transform.position) >= 0.9f) {
            Debug.Log("***" + pusher.name + " kann " + target.name + " nicht schieben, weil die beiden zu weit voneinander entfernt sind.");
            return false;
        }
        if(Vector2.Distance(pusher.transform.position, new Vector2(target.posX + 0.5f, target.posY + 0.5f)) >= 1) {
            Debug.Log("***" + pusher.name + " kann " + target.name + " nicht schieben, weil " + pusher.name + " zu weit von " + target.name + "s Zielposition(" + target.posX + "/" + target.posY + ") entfernt ist(Distanz: " + Vector2.Distance(pusher.transform.position, new Vector2(target.posX, target.posY)) + ")");
            return false;
        }
        if(target.posX == pusher.oldX && target.posY == pusher.oldY) {
            Debug.Log("***" + pusher.name + " kann " + target.name + " nicht schieben, weil es von ihm geschoben wurde.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Gibt den Richtungsvektor für den übergebenen Winkel zurück. Der Winkel wird dafür auf die nähesten 90° gerundet.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private Vector2 GetDirectionVectorForRoundedAngle(float angle) {
        float roundedAngle = Mathf.Round(angle / 90) * 90;
        //Debug.Log("***Gerundeter Winkel: " + roundedAngle);
        if(roundedAngle == 0) {
            return new Vector2(0, -1);
        } else if(roundedAngle == 90 || roundedAngle == -270) {
            return new Vector2(1, 0);
        } else if(roundedAngle == 180 || roundedAngle == -180) {
            return new Vector2(0, 1);
        } else if(roundedAngle == 270 || roundedAngle == -90) {
            return new Vector2(-1, 0);
        }
        return new Vector2(0, -1);
    }

    /// <summary>
    /// Passt die Positions- und Rotationsvariablen an, damit das GameObject richtig animiert wird.
    /// </summary>
    public void AdjustAnimationVariables() {
        oldDirection = direction;
        oldX = posX;
        oldY = posY;
        //Debug.Log("Animationsvariablen für '" + gameObject.name + "': oldX-" + oldX + ", oldY-" + oldY + ", oldDirection: " + GetFacingAngle(oldDirection));
    }
}
