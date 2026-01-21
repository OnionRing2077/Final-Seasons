[System.Serializable]
public class PlayerTask
{
    public string taskName;
    public string taskId;
    public bool completed;
    public bool isFake; // 👈 เพิ่ม
    public bool hasItem;
}
