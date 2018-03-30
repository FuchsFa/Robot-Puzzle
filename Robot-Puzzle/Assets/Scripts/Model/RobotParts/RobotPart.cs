using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotPart {
    /// <summary>
    /// Der Roboter an dem dieses Teil befestigt ist.
    /// </summary>
    protected Robot attachedTo;

    /// <summary>
    /// Ein enum der möglichen Typen, die Teile sein können.
    /// </summary>
    public enum PartType { Tool, Mobility, Sensor };

    /// <summary>
    /// Der Typ des Teils.
    /// </summary>
    public PartType type;

    /// <summary>
    /// Wird aufgerufen, wenn dieses Teil an einem Roboter befestigt wird.
    /// Wird benutzt, um die Actions des Teils dem Roboter zugänglich zu machen.
    /// </summary>
    /// <param name="robot"></param>
    abstract public void AddTo(Robot robot);

    /// <summary>
    /// Wird aufgerufen, wenn dieses Teil von einem Roboter entfernt wird.
    /// Wird benutzt, um die Actions des Teils wieder unzugänglich für den Roboter zu machen.
    /// </summary>
    /// <param name="robot"></param>
    abstract public void RemoveFrom(Robot robot);

    /// <summary>
    /// Liefert alle Actions, die dieses Teil bereitstellt.
    /// </summary>
    /// <returns></returns>
    abstract public List<string> GetActionList();
}
