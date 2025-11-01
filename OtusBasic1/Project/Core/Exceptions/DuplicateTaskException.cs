using System.Runtime.Serialization;

[Serializable]
public class DuplicateTaskException : Exception
{
    public string Task { get; }

    public DuplicateTaskException(string task)
        : base($"Задача '{task}' уже существует")
    {
        Task = task;
    }

    protected DuplicateTaskException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}