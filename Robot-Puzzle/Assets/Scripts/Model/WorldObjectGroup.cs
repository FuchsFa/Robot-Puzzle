using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectGroup : MonoBehaviour {

    public List<WorldObject> objects = new List<WorldObject>();

    /// <summary>
    /// Fügt das übergebene WorldObject zu objects hinzu und fügt dessen GameObject als Child zu diesem GameObject hinzu.
    /// </summary>
    /// <param name="obj"></param>
    public void AddObjectToGroup(WorldObject obj) {
        objects.Add(obj);
        obj.transform.SetParent(this.transform);
        obj.myGroup = this;
    }

    /// <summary>
    /// Entfernt das angegebene WorldObject aus objects.
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveObjectFromGroup(WorldObject obj) {
        if(objects.Contains(obj)) {
            objects.Remove(obj);
            obj.transform.SetParent(this.transform.parent);
        }
    }

    /// <summary>
    /// Fügt alle WorldObjects aus der übergebenen Gruppe zu dieser Gruppe hinzu und löscht die andere Gruppe.
    /// </summary>
    /// <param name="other"></param>
    public void MergeGroups(WorldObjectGroup other) {
        foreach(WorldObject obj in other.objects) {
            obj.myGroup = null;
            AddObjectToGroup(obj);
        }
        other.objects.Clear();
        Destroy(other.gameObject);
    }

    /// <summary>
    /// Überprüft, ob mindestens eins der WorldObjects in der Gruppe gegriffen wird.
    /// </summary>
    /// <returns></returns>
    public bool isGroupBeingGrabbed() {
        foreach(WorldObject worldObj in objects) {
            if(worldObj.GetComponent<InteractiveObject>().grabbedBy != null) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gibt den Weg zum übergebenen WorldObject als Vector zurück.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private Vector2 GetDistanceToObjectInGroup(WorldObject target) {
        return new Vector2(target.transform.position.x - this.transform.position.x, target.transform.position.y - this.transform.position.y);
    }

    /// <summary>
    /// Bewegt sich zur Position des angegebenen WorldObjects und passt die relativen Positionen der WorldObjects in der eigenen Gruppe an,
    /// damit sich deren absolute Position nicht ändert.
    /// </summary>
    /// <param name="target"></param>
    public void MoveToObjectInGroup(WorldObject target) {
        Vector2 wayToTravel = GetDistanceToObjectInGroup(target);
        transform.position = new Vector3(transform.position.x + wayToTravel.x, transform.position.y + wayToTravel.y);
        GetComponent<InteractiveObject>().posX = (int)(transform.position.x - 0.5f);
        GetComponent<InteractiveObject>().posY = (int)(transform.position.y - 0.5f);
        GetComponent<InteractiveObject>().AdjustAnimationVariables();
        foreach(WorldObject obj in objects) {
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x - wayToTravel.x, obj.transform.localPosition.y - wayToTravel.y);
        }
    }
}
