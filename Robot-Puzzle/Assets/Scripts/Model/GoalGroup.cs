using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGroup : MonoBehaviour {

    public List<Goal> goals = new List<Goal>();

    private void Start() {
        foreach(Goal goal in goals) {
            goal.myGroup = this;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartOutputForMyGoals() {
        if(AreMyGoalsReady()) {
            foreach(Goal goal in goals) {
                goal.TakeGameObjectForOutput(goal.lastCheckedObject);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool AreMyGoalsReady() {
        foreach(Goal goal in goals) {
            if(!goal.hasFittingObject) {
                return false;
            }
        }
        return true;
    }
}
