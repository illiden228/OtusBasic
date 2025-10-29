namespace OtusBasic1;

public class ToDoService : IToDoService
{
    private const int TASK_COUNT_LIMIT = 1;
    private const int TASK_LENGTH_LIMIT = 5;
    
    private readonly List<ToDoItem> _tasks = new();
    
    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
    {
        return _tasks.Where(x => x.User.UserId == userId).ToList();
    }

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
    {
        return _tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
    }

    public ToDoItem Add(ToDoUser user, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название задачи не может быть пустым. Введите /addtask Название задачи");
        
        if (name.Length > TASK_LENGTH_LIMIT)
            throw new ArgumentException($"Название задачи не должно превышать {TASK_LENGTH_LIMIT}");
        
        if (_tasks.Count(x => x.State == ToDoItemState.Active) >= TASK_COUNT_LIMIT)
            throw new ArgumentException($"Количество задач не должно превышать {TASK_COUNT_LIMIT}");
        
        if (_tasks.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateTaskException(name);
        
        var newTask = new ToDoItem(user, name);
        _tasks.Add(newTask);
        return newTask;
    }

    public void MarkCompleted(Guid id)
    {
        var completeTask = _tasks.FirstOrDefault(x => x.Id == id);
        
        if (completeTask == null)
            throw new ArgumentException($"Не существует задачи с Id {id}");
        
        completeTask.Complete();
    }

    public void Delete(Guid id)
    {
        var removeTask = _tasks.FirstOrDefault(x => x.Id == id);
        
        if (removeTask == null)
            throw new ArgumentException($"Не существует задачи с Id {id}");

        _tasks.Remove(removeTask);
    }
}