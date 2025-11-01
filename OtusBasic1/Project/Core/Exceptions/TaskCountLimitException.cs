using System.Runtime.Serialization;

[Serializable]
public class TaskCountLimitException : Exception
{
    public int TaskCountLimit { get; }

    public TaskCountLimitException(int taskCountLimit)
        : base($"Превышено максимальное количество задач равное {taskCountLimit}")
    {
        TaskCountLimit = taskCountLimit;
    }

    protected TaskCountLimitException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}