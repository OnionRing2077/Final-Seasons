using System.Collections.Generic;

public static class TaskDatabase
{
    public static List<PlayerTask> allTasks = new List<PlayerTask>()
    {
        new PlayerTask {
            taskName = "Clean Area",
            taskId = "CLEAN_AREA"
        },
        new PlayerTask { 
            taskId = "deliver_document", 
            taskName = "Deliver Document" },

    };
}

