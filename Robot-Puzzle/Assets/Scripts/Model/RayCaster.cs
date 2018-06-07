using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour {

    public LayerMask collisionMask;
    public Collider2D myCollider;
    public Vector2 raycastOrigin;

    private void Awake() {
        if(gameObject.tag == "Robot") {
            myCollider = GetComponent<CircleCollider2D>();
        }
    }

    // Use this for initialization
    void Start () {
        raycastOrigin = new Vector2(0.5f, 0.5f);
	}

    /// <summary>
    /// Überprüft, ob im Tile vor dem Objekt(in angegebener Richtung) etwas ist, mit dem es zusammenstoßen würde.
    /// </summary>
    /// <returns></returns>
    public bool CheckForCollisionsInDirection(Vector2 dir) {
        bool collided = false;

        InteractiveObject obj = GetComponent<InteractiveObject>();
        raycastOrigin = new Vector2(transform.position.x + (dir.x * 0.6f), transform.position.y + (dir.y * 0.6f));
        if(GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            raycastOrigin += obj.direction;
        }
        collided = CheckForCollision(raycastOrigin, dir);

        return collided;
    }

    /// <summary>
    /// Überprüft, ob vom angegebenen Punkt etwas in der angegebenen Richtung ist, mit dem das Objekt zusammenstoßen würde.
    /// </summary>
    /// <param name="raycastOrigin"></param>
    /// <param name="raycastDirection"></param>
    /// <returns></returns>
    private bool CheckForCollision(Vector2 raycastOrigin, Vector2 raycastDirection, float distance = 0.3f) {
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, distance, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.red, 0.3f);
        if (hit) {
            if (hit.collider != myCollider) {
                //Debug.Log(gameObject.name + " ist mit " + hit.transform.gameObject.name + " zusammengestoßen.");
                bool haveGroup = GetComponent<WorldObject>().myGroup != null;
                if (GetComponent<WorldObject>() && hit.transform.GetComponent<WorldObject>() && haveGroup && GetComponent<WorldObject>().myGroup.objects.Contains(hit.transform.GetComponent<WorldObject>())) {
                    //Debug.Log("Aber " + hit.transform.gameObject.name + " gehört zu seiner objectGroup und kann daher nicht mit ihm kollidieren.");
                } else {
                    return !hit.transform.GetComponent<InteractiveObject>().Movable;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Überprüft, ob das Object, das der Roboter gerade hält mit etwas zusammenstößt.
    /// </summary>
    /// <returns></returns>
    public bool CheckForCollisionOfGrabbedObject() {
        if(GetComponent<Robot>() == null || GetComponent<Robot>().GrabbedObject == null) {
            return false;
        }
        bool collided = false;

        if(GetComponent<Robot>().GrabbedObject.GetComponent<CircleCollider2D>().IsTouchingLayers(collisionMask)) {
            collided = true;
        }

        return collided;
    }

    /// <summary>
    /// Überprüft, ob im Tile vor dem Objekt(in Blickrichtung) etwas ist, das gegriffen werden kann.
    /// </summary>
    /// <returns></returns>
    public InteractiveObject CheckForGrabableObject(float distance = 0.3f) {
        InteractiveObject interactableObject = null;

        InteractiveObject obj = GetComponent<InteractiveObject>();
        raycastOrigin = new Vector2(transform.position.x + (obj.direction.x * 0.6f), transform.position.y + (obj.direction.y * 0.6f));
        Vector2 raycastDirection = obj.direction;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, distance, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.red, 0.3f);
        if (hit) {
            if (hit.collider != myCollider && hit.transform.GetComponent<InteractiveObject>() != null) {
                if(hit.transform.GetComponent<InteractiveObject>().Grabable == true) {
                    Debug.Log(gameObject.name + " hat ein Objekt zum Greifen gefunden: " + hit.transform.gameObject.name);
                    GameObject foundObject = hit.transform.gameObject;
                    if (foundObject.GetComponent<WorldObject>() && foundObject.GetComponent<WorldObject>().myGroup != null) {
                        interactableObject = foundObject.GetComponent<WorldObject>().myGroup.GetComponent<InteractiveObject>();
                        foundObject.GetComponent<WorldObject>().myGroup.MoveToObjectInGroup(foundObject.GetComponent<WorldObject>());
                    } else {
                        interactableObject = hit.transform.GetComponent<InteractiveObject>();
                    }
                } else {
                    Debug.Log(gameObject.name + " hat ein Objekt gefunden: " + hit.transform.gameObject.name + ", aber es kann nicht gegriffen werden.");
                }
            }
        }

        return interactableObject;
    }

    /// <summary>
    /// Überprüft, ob im Tile vor dem Objekt(in Bewegungsrichtung) etwas ist, das geschoben werden kann.
    /// </summary>
    /// <returns></returns>
    public InteractiveObject CheckForPushableObject(Vector2 dir, float distance = 0.3f) {
        InteractiveObject interactableObject = null;

        InteractiveObject obj = GetComponent<InteractiveObject>();
        raycastOrigin = new Vector2(transform.position.x + (dir.x * 0.6f), transform.position.y + (dir.y * 0.6f));
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            raycastOrigin += obj.direction;
        }
        //Vector2 raycastDirection = obj.direction;
        Vector2 raycastDirection = dir;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, distance, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.blue, 0.3f);
        if (hit) {
            if (hit.collider != myCollider && hit.transform.GetComponent<InteractiveObject>() != null) {
                if (hit.transform.GetComponent<InteractiveObject>().Movable == true) {
                    //Debug.Log(gameObject.name + " hat ein Objekt zum Schieben gefunden: " + hit.transform.gameObject.name);
                    if(hit.transform.GetComponent<WorldObject>()) {
                        if(GetComponent<WorldObject>() && GetComponent<WorldObject>().myGroup != null && GetComponent<WorldObject>().myGroup.objects.Contains(hit.transform.GetComponent<WorldObject>())) {
                            //Debug.Log("Aber " + hit.transform.gameObject.name + " gehört zu seiner objectGroup und kann daher nicht geschoben werden.");
                        } else if(hit.transform.GetComponent<WorldObject>().myGroup != null) {
                            interactableObject = hit.transform.GetComponent<WorldObject>().myGroup.GetComponent<InteractiveObject>();
                        } else {
                            interactableObject = hit.transform.GetComponent<InteractiveObject>();
                        }
                    } else {
                        interactableObject = hit.transform.GetComponent<InteractiveObject>();
                    }
                }
                else {
                    Debug.Log(gameObject.name + " hat ein Objekt gefunden: " + hit.transform.gameObject.name + ", aber es kann nicht geschoben werden.");
                }
            }
        }

        return interactableObject;
    }

    /// <summary>
    /// Überprüft, ob es in der angegebenen Richtung ein WorldObject gibt.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public WorldObject CheckForWorldObject(Vector2 dir, float distance = 0.3f) {
        WorldObject worldObject = null;

        InteractiveObject obj = GetComponent<InteractiveObject>();
        raycastOrigin = new Vector2(transform.position.x + (dir.x * 0.6f), transform.position.y + (dir.y * 0.6f));
        if (GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            raycastOrigin += obj.direction;
        }
        //Vector2 raycastDirection = obj.direction;
        Vector2 raycastDirection = dir;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, distance, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.cyan, 0.3f);
        if (hit) {
            if (hit.collider != myCollider && hit.transform.GetComponent<WorldObject>() != null) {
                worldObject = hit.transform.gameObject.GetComponent<WorldObject>();
            }
        }

        return worldObject;
    }

    /// <summary>
    /// Überprüft, ob es in der angegebenen Richtung ein InteractiveObject gibt.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public InteractiveObject CheckForInteractiveObject(Vector2 dir, float distance = 0.3f) {
        InteractiveObject interactiveObject = null;

        InteractiveObject obj = GetComponent<InteractiveObject>();
        raycastOrigin = new Vector2(transform.position.x + (dir.x * 0.6f), transform.position.y + (dir.y * 0.6f));
        if(GetComponent<Robot>() && GetComponent<Robot>().GrabbedObject != null) {
            raycastOrigin += obj.direction;
        }
        Vector2 raycastDirection = dir;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, distance, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.magenta, 0.3f);
        if(hit) {
            if(hit.collider != myCollider && hit.transform.GetComponent<InteractiveObject>() != null) {
                interactiveObject = hit.transform.gameObject.GetComponent<InteractiveObject>();
            }
        }

        return interactiveObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
