using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectGroup : MonoBehaviour {

    public List<WorldObject> objects = new List<WorldObject>();

    public void AddObjectToGroup(WorldObject obj) {
        objects.Add(obj);
        obj.transform.SetParent(this.transform);
        obj.myGroup = this;
    }

    public void RemoveObjectFromGroup(WorldObject obj) {
        if(objects.Contains(obj)) {
            objects.Remove(obj);
        }
    }

    public void MergeGroups(WorldObjectGroup other) {
        foreach(WorldObject obj in other.objects) {
            obj.myGroup = null;
            AddObjectToGroup(obj);
        }
        other.objects.Clear();
        Destroy(other.gameObject);
    }

    private Vector2 GetDistanceToObjectInGroup(WorldObject target) {
        return new Vector2(target.transform.position.x - this.transform.position.x, target.transform.position.y - this.transform.position.y);
    }

    public void MoveToObjectInGroup(WorldObject target) {
        Vector2 wayToTravel = GetDistanceToObjectInGroup(target);
        transform.position = new Vector3(transform.position.x + wayToTravel.x, transform.position.y + wayToTravel.y);
        GetComponent<InteractiveObject>().posX = (int)(transform.position.x - 0.5f);
        GetComponent<InteractiveObject>().posY = (int)(transform.position.y - 0.5f);
        foreach(WorldObject obj in objects) {
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x - wayToTravel.x, obj.transform.localPosition.y - wayToTravel.y);
        }
    }
}
