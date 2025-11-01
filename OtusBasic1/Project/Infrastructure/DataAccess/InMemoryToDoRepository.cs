namespace OtusBasic1;

public class InMemoryToDoRepository : IToDoRepository
{
    private readonly List<ToDoItem> _tasks = new();
    
    public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
    {
        return _tasks.Where(x => x.User.UserId == userId).ToList();
    }

    public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
    {
        return _tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active)
                .Select(x => x.Copy())
                .ToList();
    }

    public async Task<ToDoItem?> Get(Guid id, CancellationToken ct)
    {
        return _tasks.FirstOrDefault(x => x.Id == id)?.Copy();
    }

    public async Task Add(ToDoItem item, CancellationToken ct)
    {
        _tasks.Add(item.Copy());
    }

    public async Task Update(ToDoItem item, CancellationToken ct)
    {
        var index = _tasks.FindIndex(x => x.Id == item.Id);
        _tasks[index] = item.Copy();
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        _tasks.RemoveAll(x => x.Id == id);
    }

    public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
    {
        return _tasks.Any(x => x.User.UserId == userId && x.Name == name);
    }

    public async Task<int> CountActive(Guid userId, CancellationToken ct)
    {
        return _tasks.Count(x => x.User.UserId == userId && x.State == ToDoItemState.Active);
    }

    public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
    {
        return _tasks.Where(x => x.User.UserId == userId)
                .Where(predicate)
                .Select(x => x.Copy())
                .ToList();
    }
}