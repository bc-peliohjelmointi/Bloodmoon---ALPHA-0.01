using System.Collections.Generic;
using UnityEngine;

public class SaveMyQuests : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    void OnEnable() {
        if (questManager == null) {
            questManager = GetComponent<QuestManager>();
        }
        if (questManager == null) {
            questManager = FindAnyObjectByType<QuestManager>();
        }
    }

    public void Save(ref QuestManagerData data) {
        data.quests = new List<QuestEntryData>();
        List<IQuest> sceneQuests = GetAllSceneQuests();
        for (int i = 0; i < sceneQuests.Count; i++) {
            IQuest quest = sceneQuests[i];
            if (quest == null) {
                continue;
            }

            QuestEntryData entry = new QuestEntryData {
                questId = quest.QuestId,
                state = quest.State,
                stepDone = quest.GetStepDoneFlags(),
                requirementProgress = quest.GetRequirementCurrentAmounts()
            };
            data.quests.Add(entry);
        }

        Debug.Log($"Saved {data.quests.Count} quests.");
    }

    public void Load(QuestManagerData data) {
        if (data.quests == null) {
            return;
        }

        List<IQuest> sceneQuests = GetAllSceneQuests();
        for (int i = 0; i < data.quests.Count; i++) {
            QuestEntryData entry = data.quests[i];
            IQuest quest = FindQuest(entry.questId, sceneQuests);
            if (quest == null) {
                continue;
            }

            quest.LoadProgress(entry.stepDone, entry.requirementProgress);
            quest.SetState(entry.state);
        }

        if (questManager != null) {
            questManager.RebuildProgressLists();
        }

        Debug.Log($"Loaded {data.quests.Count} quests from save.");
    }

    private IQuest FindQuest(string questId, List<IQuest> sceneQuests) {
        for (int i = 0; i < sceneQuests.Count; i++) {
            IQuest quest = sceneQuests[i];
            if (quest != null && quest.QuestId == questId) {
                return quest;
            }
        }
        return null;
    }

    private List<IQuest> GetAllSceneQuests() {
        List<IQuest> quests = new List<IQuest>();

        if (questManager != null && questManager.allQuests != null) {
            for (int i = 0; i < questManager.allQuests.Count; i++) {
                IQuest quest = questManager.allQuests[i];
                if (quest != null && !quests.Contains(quest)) {
                    quests.Add(quest);
                }
            }
        }

        IQuest[] foundQuests = Object.FindObjectsByType<IQuest>(FindObjectsSortMode.None);
        for (int i = 0; i < foundQuests.Length; i++) {
            IQuest quest = foundQuests[i];
            if (quest != null && !quests.Contains(quest)) {
                quests.Add(quest);
                if (questManager != null) {
                    questManager.RegisterQuest(quest);
                }
            }
        }

        return quests;
    }
}
