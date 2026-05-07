using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour {
    [Header("Quests")]
    [SerializeField] public List<IQuest> allQuests;

    [Header("Progress")]
    [SerializeField] public List<IQuest> availableQuests;
    [SerializeField] public List<IQuest> currentQuests;
    [SerializeField] public List<IQuest> completedQuests;

    void Awake() {
        EnsureLists();
    }

    private void EnsureLists() {
        if (allQuests == null) allQuests = new List<IQuest>();
        if (availableQuests == null) availableQuests = new List<IQuest>();
        if (currentQuests == null) currentQuests = new List<IQuest>();
        if (completedQuests == null) completedQuests = new List<IQuest>();
    }

    public void RegisterQuest(IQuest quest) {
        if (quest == null) {
            return;
        }

        EnsureLists();

        if (!allQuests.Contains(quest)) {
            allQuests.Add(quest);
        }

        SyncQuestStateList(quest);
    }

    public void SyncQuestStateList(IQuest quest) {
        if (quest == null) {
            return;
        }

        EnsureLists();

        availableQuests.Remove(quest);
        currentQuests.Remove(quest);
        completedQuests.Remove(quest);

        switch (quest.State) {
            case IQuest.QuestState.canGetQuest:
                availableQuests.Add(quest);
                break;
            case IQuest.QuestState.isInProgress:
                currentQuests.Add(quest);
                break;
            case IQuest.QuestState.questDone:
                completedQuests.Add(quest);
                break;
        }
    }

    public void RebuildProgressLists() {
        EnsureLists();
        availableQuests.Clear();
        currentQuests.Clear();
        completedQuests.Clear();

        for (int i = 0; i < allQuests.Count; i++) {
            if (allQuests[i] != null) {
                SyncQuestStateList(allQuests[i]);
            }
        }
    }
}
