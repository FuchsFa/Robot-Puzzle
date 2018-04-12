using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

    [SerializeField]
    private string worldObjectType;

    private GameObject worldObjectReference;

    [SerializeField]
    private SpriteRenderer worldObjectPreview;

    private int currentAmount;
    [SerializeField]
    private int amountNeeded;

    [SerializeField]
    private Text display;

    [SerializeField]
    private LayerMask collisionMask;

    public bool isFulfilled;

    // Use this for initialization
    void Start () {
        isFulfilled = false;
        currentAmount = 0;
        display.text = currentAmount + "/" + amountNeeded;
        worldObjectReference = GameStateManager.Instance.worldObjectManager.GetPrefabFromDictionary(worldObjectType);
        worldObjectPreview.sprite = worldObjectReference.GetComponent<SpriteRenderer>().sprite;
        GetComponent<InteractiveObject>().SetStartingPositionAndRotation((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f), new Vector2(0, -1));
    }

    /// <summary>
    /// Überprüft, ob auf dem Feld des Goals ein Objekt liegt, dass mit seiner Referenz übereinstimmt. Wenn ja, wird das Objekt entfernt und der Zähler um eins erhöht.
    /// </summary>
    public void OnNewTurn() {
        Vector2 raycastOrigin = new Vector2(transform.position.x, transform.position.y);
        Vector2 raycastDirection = new Vector2(0, -1);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, 0.3f, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.cyan, 0.5f);
        if (hit) {
            if (CompareToReference(hit.transform.gameObject) && hit.transform.gameObject.GetComponent<InteractiveObject>().IsReadyForOutput()) {
                currentAmount++;
                GameStateManager.Instance.worldObjectManager.RemoveWorldObject(hit.transform.gameObject);
                if (currentAmount >= amountNeeded) {
                    currentAmount = amountNeeded;
                    isFulfilled = true;
                }
                display.text = currentAmount + "/" + amountNeeded;
            }
        }
    }

    /// <summary>
    /// Vergleicht das übergebene Objekt mit dem Referenzobjekt.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool CompareToReference(GameObject obj) {
        if(obj.GetComponent<WorldObject>() == null) {
            return false;
        }
        if(obj.GetComponent<WorldObject>().GetType() != worldObjectReference.GetType()) {
            return false;
        }
        //TODO: Überprüfen, ob sonstige Faktoren, wie etwa Rotation und Verbindungen mit anderen Objekten stimmen.
        return true;
    }

    /// <summary>
    /// Setzt das Ziel auf seine Anfangswerte zurück.
    /// </summary>
    public void OnStop() {
        isFulfilled = false;
        currentAmount = 0;
        display.text = currentAmount + "/" + amountNeeded;
    }
}
