using UnityEngine;
using System.Collections.Generic;
using System;

public class IQuest : MonoBehaviour {
    public enum QuestType {
        Story,
        Extra
    }
    [Header("Quest Settings")]
    [SerializeField] protected string questName;
    [SerializeField] protected List<Step> nodes;
    [SerializeField] protected QuestType qt = QuestType.Story;

    private void Node(int id, string name, bool done) {
        
    }

    void OnEnable() {
        foreach (var node in nodes) {
            Node(node.id, node.name, node.done);
        }
    }

    protected virtual void GiveQuest() {
        
    }

    [ContextMenu("Test")]
    public void GetCurrentProgress() {
        int prog = 0;
        foreach (var node in nodes) {
            if (node.done == true) {
                prog += 1;
            }
            else {
                int count = nodes.Count;
                Debug.Log($"Current Progress for the Quest: {questName}, is {prog}/{count} done!");
                break;
            }
        }
    }

}

[System.Serializable]
public class Step {
    public int id = 1;
    public string name = "Quest";
    public bool done = false;
    public List<Requirement> reqs;
}

public enum RequirementType
{
    Item,
    Kill,
    Talk
}

[System.Serializable]
public class Requirement {
    public RequirementType type;
    public string targetId;
    public int neededAmount = 1;
    public int currentAmount = 0;

    public bool IsMet() => currentAmount >= neededAmount;
}