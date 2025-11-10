using System.Runtime.Serialization;

namespace OtusBasic1.Project.Core.Exceptions;

[Serializable]
public class TaskLengthLimitException : Exception
{
    public int TaskLength { get; }
    public int TaskLengthLimit { get; }

    public TaskLengthLimitException(int taskLength, int taskLengthLimit)
        : base($"Длина задачи '{taskLength}' превышает максимально допустимое значение {taskLengthLimit}")
    {
        TaskLength = taskLength;
        TaskLengthLimit = taskLengthLimit;
    }

    protected TaskLengthLimitException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}