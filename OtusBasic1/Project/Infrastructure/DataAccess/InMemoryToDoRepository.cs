using OtusBasic1.Project.Core.DataAccess;
using OtusBasic1.Project.Core.Entities;

namespace OtusBasic1.Project.Infrastructure.DataAccess;

public class InMemoryToDoRepository : IToDoRepository
{
    private readonly List<ToDoItem> _tasks = new();
    
    public Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
    {
        var result = _tasks.Where(x => x.User.UserId == userId).ToList();
        return Task.FromResult<IReadOnlyList<ToDoItem>>(result);
    }

    public Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
    {
        var result = _tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active)
                                    .Select(x => x.Copy())
                                    .ToList();
        return Task.FromResult<IReadOnlyList<ToDoItem>>(result);
    }

    public Task<ToDoItem?> Get(Guid id, CancellationToken ct)
    {
        var result = _tasks.FirstOrDefault(x => x.Id == id)?.Copy();
        return Task.FromResult(result);
    }

    public Task Add(ToDoItem item, CancellationToken ct)
    {
        _tasks.Add(item.Copy());
        return Task.CompletedTask;
    }

    public Task Update(ToDoItem item, CancellationToken ct)
    {
        var index = _tasks.FindIndex(x => x.Id == item.Id);
        _tasks[index] = item.Copy();
        return Task.CompletedTask;
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        _tasks.RemoveAll(x => x.Id == id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
    {
        var result = _tasks.Any(x => x.User.UserId == userId && x.Name == name);
        return Task.FromResult(result);
    }

    public Task<int> CountActive(Guid userId, CancellationToken ct)
    {
        var result = _tasks.Count(x => x.User.UserId == userId && x.State == ToDoItemState.Active);
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
    {
        
        var result = _tasks.Where(x => x.User.UserId == userId)
                                        .Where(predicate)
                                        .Select(x => x.Copy())
                                        .ToList();
        return Task.FromResult<IReadOnlyList<ToDoItem>>(result);
    }
}