using UnityEngine;
using System.Collections.Generic;

public class IQuest : MonoBehaviour {
    public enum QuestType {
        Story,
        Extra
    }
    [Header("Quest Settings")]
    [SerializeField] protected string questId;
    [SerializeField] protected string questName;
    [SerializeField] protected List<Step> nodes;
    [SerializeField] protected QuestType qt = QuestType.Story;

    public enum QuestState {
        NotAvailable,
        canGetQuest,
        isInProgress,
        questDone
    }

    [SerializeField] protected QuestState state = QuestState.NotAvailable;

    public string QuestId => string.IsNullOrWhiteSpace(questId) ? questName : questId;
    public QuestState State => state;

    protected Inventory iv;
    protected QuestManager qm;

    protected virtual void ProgressQuest(){
        if (nodes == null || nodes.Count == 0) {
            return;
        }

        foreach (var node in nodes) {
            if (node.done) {
                continue;
            }

            if (StepDone(node)) {
                node.done = true;
            }

            break;
        }

        if (AreAllStepsDone()) {
            CompleteQuest();
        }

    }
    void OnEnable() {
        qm = FindAnyObjectByType<QuestManager>();
        iv = FindAnyObjectByType<Inventory>();

        if (qm != null) {
            qm.RegisterQuest(this);
        }
    }

    [ContextMenu("Test")]
    public void GetCurrentProgress() {
        int prog = 0;
        int count = nodes != null ? nodes.Count : 0;

        if (count == 0) {
            Debug.Log($"Current Progress for the Quest: {questName}, is 0/0 done!");
            return;
        }

        foreach (var node in nodes) {
            if (node.done) {
                prog += 1;
            }
        }

        Debug.Log($"Current Progress for the Quest: {questName}, is {prog}/{count} done!");
    }
    
    protected bool StepDone(Step node) {
        if (node.reqs == null || node.reqs.Count == 0) {
            return true;
        }

        foreach (var req in node.reqs) {
            if (req == null || !req.IsMet()) {
                return false;
            }
        }

        return true;
    }

    protected bool AreAllStepsDone() {
        if (nodes == null || nodes.Count == 0) {
            return false;
        }

        foreach (var node in nodes) {
            if (!node.done) {
                return false;
            }
        }

        return true;
    }

    protected void CompleteQuest() {
        SetState(QuestState.questDone);
        Debug.Log("Completed");
    }

    public void SetState(QuestState newState) {
        state = newState;
        if (qm != null) {
            qm.SyncQuestStateList(this);
        }
    }

    public List<bool> GetStepDoneFlags() {
        List<bool> doneFlags = new List<bool>();

        if (nodes == null) {
            return doneFlags;
        }

        for (int i = 0; i < nodes.Count; i++) {
            doneFlags.Add(nodes[i] != null && nodes[i].done);
        }

        return doneFlags;
    }

    public List<int> GetRequirementCurrentAmounts() {
        List<int> progress = new List<int>();

        if (nodes == null) {
            return progress;
        }

        for (int i = 0; i < nodes.Count; i++) {
            Step node = nodes[i];
            if (node == null || node.reqs == null) {
                continue;
            }

            for (int j = 0; j < node.reqs.Count; j++) {
                if (node.reqs[j] != null) {
                    progress.Add(node.reqs[j].currentAmount);
                }
            }
        }

        return progress;
    }

    public void LoadProgress(List<bool> doneFlags, List<int> requirementProgress) {
        if (nodes == null) {
            return;
        }

        int reqIndex = 0;

        for (int i = 0; i < nodes.Count; i++) {
            Step node = nodes[i];
            if (node == null) {
                continue;
            }

            if (doneFlags != null && i < doneFlags.Count) {
                node.done = doneFlags[i];
            }

            if (node.reqs == null) {
                continue;
            }

            for (int j = 0; j < node.reqs.Count; j++) {
                if (node.reqs[j] == null) {
                    continue;
                }

                if (requirementProgress != null && reqIndex < requirementProgress.Count) {
                    node.reqs[j].currentAmount = requirementProgress[reqIndex];
                }
                reqIndex++;
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
    Talk,
    Quest
}

[System.Serializable]
public class Requirement {
    public RequirementType type;
    public string targetId;
    public int neededAmount = 1;
    public int currentAmount = 0;

    public bool CanBeDone() => currentAmount >= neededAmount;
}
