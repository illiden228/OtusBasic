using OtusBasic1.Project.Core.DataAccess;
using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Exceptions;

namespace OtusBasic1.Project.Core.Services;

public class ToDoService : IToDoService
{
    private const int TASK_COUNT_LIMIT = 10;
    private const int TASK_LENGTH_LIMIT = 50;

    private readonly IToDoRepository _repository;

    public ToDoService(IToDoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
    {
        return await _repository.GetAllByUserId(userId, ct);
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
    {
        return await _repository.GetActiveByUserId(userId, ct);
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct)
    {
        return await _repository.Find(user.UserId, x => x.Name.StartsWith(namePrefix), ct);
    }

    public async Task<ToDoItem> Add(ToDoUser user, string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название задачи не может быть пустым. Введите /addtask Название задачи");
        
        if (name.Length > TASK_LENGTH_LIMIT)
            throw new ArgumentException($"Название задачи не должно превышать {TASK_LENGTH_LIMIT}");

        var activeUsers = await _repository.GetActiveByUserId(user.UserId, ct);
        if (activeUsers.Count >= TASK_COUNT_LIMIT)
            throw new ArgumentException($"Количество задач не должно превышать {TASK_COUNT_LIMIT}");

        var existsUser = await _repository.ExistsByName(user.UserId, name, ct);
        if (existsUser)
            throw new DuplicateTaskException(name);
        
        var newTask = new ToDoItem(user, name);
        await _repository.Add(newTask, ct);
        return newTask;
    }

    public async Task MarkCompleted(Guid id, CancellationToken ct)
    {
        var completeTask = await _repository.Get(id, ct);
        
        if (completeTask == null)
            throw new ArgumentException($"Не существует задачи с Id {id}");
        
        completeTask.Complete();
        await _repository.Update(completeTask, ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await _repository.Delete(id, ct);
    }
}