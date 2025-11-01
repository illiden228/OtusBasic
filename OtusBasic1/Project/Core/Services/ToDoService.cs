namespace OtusBasic1;

public class ToDoService : IToDoService
{
    private const int TASK_COUNT_LIMIT = 10;
    private const int TASK_LENGTH_LIMIT = 50;

    private readonly IToDoRepository _repository;

    public ToDoService(IToDoRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
    {
        return _repository.GetAllByUserId(userId);
    }

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
    {
        return _repository.GetActiveByUserId(userId);
    }

    public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
    {
        return _repository.Find(user.UserId, x => x.Name.StartsWith(namePrefix));
    }

    public ToDoItem Add(ToDoUser user, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название задачи не может быть пустым. Введите /addtask Название задачи");
        
        if (name.Length > TASK_LENGTH_LIMIT)
            throw new ArgumentException($"Название задачи не должно превышать {TASK_LENGTH_LIMIT}");
        
        
        if (_repository.GetActiveByUserId(user.UserId).Count >= TASK_COUNT_LIMIT)
            throw new ArgumentException($"Количество задач не должно превышать {TASK_COUNT_LIMIT}");
        
        
        if (_repository.ExistsByName(user.UserId, name))
            throw new DuplicateTaskException(name);
        
        var newTask = new ToDoItem(user, name);
        _repository.Add(newTask);
        return newTask;
    }

    public void MarkCompleted(Guid id)
    {
        var completeTask = _repository.Get(id);
        
        if (completeTask == null)
            throw new ArgumentException($"Не существует задачи с Id {id}");
        
        completeTask.Complete();
        _repository.Update(completeTask);
    }

    public void Delete(Guid id)
    {
        _repository.Delete(id);
    }
}