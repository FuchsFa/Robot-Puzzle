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
    /// Überprüft, ob im Tile vor dem Roboter(in Blickrichtung) etwas ist, mit dem der Roboter zusammenstoßen würde.
    /// </summary>
    /// <returns></returns>
    public bool CheckForCollisionsInFacingDirection() {
        bool collided = false;

        Robot bot = GetComponent<Robot>();
        raycastOrigin = new Vector2(transform.position.x + (bot.direction.x * 0.6f), transform.position.y + (bot.direction.y * 0.6f));
        Vector2 raycastDirection = bot.direction;
        if(bot.GrabbedObject != null) {
            raycastOrigin += bot.direction;
        }
        collided = CheckForCollision(raycastOrigin, raycastDirection);

        return collided;
    }

    /// <summary>
    /// Überprüft, ob vom angegebenen Punkt etwas in der angegebenen Richtung ist, mit dem das Objekt zusammenstoßen würde.
    /// </summary>
    /// <param name="raycastOrigin"></param>
    /// <param name="raycastDirection"></param>
    /// <returns></returns>
    private bool CheckForCollision(Vector2 raycastOrigin, Vector2 raycastDirection) {
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, 0.3f, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.red, 0.3f);
        if (hit) {
            if (hit.collider != myCollider) {
                Debug.Log(gameObject.name + " ist mit " + hit.transform.gameObject.name + " zusammengestoßen.");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Überprüft, ob das Object, das der Roboter gerade häl mit etwas zusammenstößt.
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
    /// Überprüft, ob im Tile vor dem Roboter(in Blickrichtung) etwas ist, das der Roboter greifen kann.
    /// </summary>
    /// <returns></returns>
    public InteractiveObject CheckForGrabableObject() {
        GameObject interactableObject = null;

        Robot bot = GetComponent<Robot>();
        raycastOrigin = new Vector2(transform.position.x + (bot.direction.x * 0.6f), transform.position.y + (bot.direction.y * 0.6f));
        Vector2 raycastDirection = bot.direction;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, 0.3f, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.red, 0.3f);
        if (hit) {
            if (hit.collider != myCollider && hit.transform.GetComponent<InteractiveObject>() != null) {
                if(hit.transform.GetComponent<InteractiveObject>().Grabable == true) {
                    Debug.Log(gameObject.name + " hat ein Objekt zum Greifen gefunden: " + hit.transform.gameObject.name);
                    interactableObject = hit.transform.gameObject;
                } else {
                    Debug.Log(gameObject.name + " hat ein Objekt gefunden: " + hit.transform.gameObject.name + ", aber es kann nicht gegriffen werden.");
                }
            }
        }

        return interactableObject.GetComponent<InteractiveObject>();
    }

    /// <summary>
    /// Überprüft, ob im Tile vor dem Objekt(in Bewegungsrichtung) etwas ist, das geschoben werden kann.
    /// </summary>
    /// <returns></returns>
    public InteractiveObject CheckForPushableObject() {
        GameObject interactableObject = null;

        //TODO: implementieren, sobald interaktive Objekte sich bewegen können.

        return interactableObject.GetComponent<InteractiveObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
