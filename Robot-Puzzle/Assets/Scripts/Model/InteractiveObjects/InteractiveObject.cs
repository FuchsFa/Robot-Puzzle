using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour {

    /// <summary>
    /// Der Roboter, der dieses Objekt greift.
    /// </summary>
    public Robot grabbedBy;

    /// <summary>
    /// Gibt an, ob das Objekt bewegt(z.B. geschoben) werden kann.
    /// </summary>
    [SerializeField]private bool movable;

    /// <summary>
    /// Gibt an, ob das Objekt von einem Roboter gegriffen werden kann.
    /// </summary>
    [SerializeField]private bool grabable;

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
