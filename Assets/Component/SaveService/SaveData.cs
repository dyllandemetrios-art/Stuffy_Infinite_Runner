namespace Components.SaveService
{
    /// <summary>Serializable data container holding persistent player stats across sessions.</summary>
    [System.Serializable]
    public class SaveData
    {
        public int RunCount;
        public int BestTime;
    }
}