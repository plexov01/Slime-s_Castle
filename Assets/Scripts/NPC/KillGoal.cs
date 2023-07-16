
[System.Serializable]
public class KillGoal : Goal
{
    public int mobID;

    public override void Init(Quest quest)
    {
        base.Init(quest);
        scr_EventSystem.instance.mobDeath.AddListener(MobKilled);
    }
    
    public override void Uninit()
    {
        base.Uninit();
        scr_EventSystem.instance.mobDeath.RemoveListener(MobKilled);

    }

    private void MobKilled(int mobID)
    {
        if (mobID == this.mobID)
        {
            currentAmount++;
            Evaluate();
        }
    }
}
