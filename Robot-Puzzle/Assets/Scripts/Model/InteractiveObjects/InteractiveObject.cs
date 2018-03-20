using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject {

    private int posX;
    private int posY;

    /// <summary>
    /// Gibt an, ob das Objekt bewegt(z.B. geschoben) werden kann.
    /// </summary>
    private bool movable;

    /// <summary>
    /// Gibt an, ob das Objekt von einem Roboter gegriffen werden kann.
    /// </summary>
    private bool grabable;

    //TODO: TerrainType hinzufügen
}
