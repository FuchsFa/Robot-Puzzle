using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

    [SerializeField]
    private string worldObjectType;

    private GameObject worldObjectPrefab;

    [SerializeField]
    private SpriteRenderer worldObjectPreview;

    [SerializeField]
    private int spawnInterval;
    private int counter;

    [SerializeField]
    private Text counterDisplay;

    [SerializeField]
    private LayerMask collisionMask;

	// Use this for initialization
	void Start () {
        counter = spawnInterval - 1;
        worldObjectPrefab = GameStateManager.Instance.worldObjectManager.GetPrefabFromDictionary(worldObjectType);
        worldObjectPreview.sprite = worldObjectPrefab.GetComponent<SpriteRenderer>().sprite;
        counterDisplay.text = "" + spawnInterval;
        GetComponent<InteractiveObject>().SetStartingPositionAndRotation((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f), new Vector2(0, -1));
	}

    /// <summary>
    /// Erhöht den Zähler um eins. Wenn der Zähler den Grenzwert erreicht hat, wird er auf 0 zurückgesetzt und eine Kopie des Prefabs wird auf dem Feld des Spawners erstellt.
    /// </summary>
    public void OnNewTurn() {
        counter++;
        counterDisplay.text = "" + (spawnInterval - counter);
        if(counter >= spawnInterval) {
            counter = 0;
            if(IsSpaceFree()) {
                GameStateManager.Instance.worldObjectManager.CreateWorldObject(worldObjectType, (int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));
            }
        }
    }

    /// <summary>
    /// Setzt den Spawner auf seine Anfangswerte zurück.
    /// </summary>
    public void OnStop() {
        counter = spawnInterval - 1;
        counterDisplay.text = "" + spawnInterval;
    }

    /// <summary>
    /// Überprüft, ob der Spawner ein neues Objekt auf seinem Feld platzieren darf.
    /// </summary>
    /// <returns></returns>
    private bool IsSpaceFree() {
        bool temp = true;

        Vector2 raycastOrigin = new Vector2(transform.position.x, transform.position.y);
        Vector2 raycastDirection = new Vector2(0, -1);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, 0.3f, collisionMask);
        Debug.DrawRay(raycastOrigin, raycastDirection, Color.green, 0.5f);
        if (hit) {
            Debug.Log(name + " versucht ein Objekt zu spawnen, aber es ist kein Platz wegen " + hit.transform.name);
            temp = false;
        }

        return temp;
    }
}
