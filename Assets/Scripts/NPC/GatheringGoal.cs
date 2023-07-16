
[System.Serializable]
public class GatheringGoal : Goal
{
    public int itemID;

    public override void Init(Quest quest)
    {
        base.Init(quest);
        //QuestSystem.instance.mushroomCollected.AddListener(ItemCollected);
    }
    
    public override void Uninit()
    {
        base.Uninit();
        //QuestSystem.instance.mushroomCollected.RemoveListener(ItemCollected);
    }

    private void ItemCollected(int itemID)
    {
        if (itemID == this.itemID)
        {
            currentAmount++;
            Evaluate();
        }
    }
}
