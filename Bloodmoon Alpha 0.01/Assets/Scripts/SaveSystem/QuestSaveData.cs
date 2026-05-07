using System.Collections.Generic;

[System.Serializable]
public struct QuestManagerData
{
    public List<QuestEntryData> quests;
}

[System.Serializable]
public struct QuestEntryData
{
    public string questId;
    public IQuest.QuestState state;
    public List<bool> stepDone;
    public List<int> requirementProgress;
}
