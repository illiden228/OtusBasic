namespace OtusBasic1;

public class InMemoryToDoRepository : IToDoRepository
{
    private readonly List<ToDoItem> _tasks = new();
    
    public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
    {
        return _tasks.Where(x => x.User.UserId == userId).ToList();
    }

    public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
    {
        return _tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active)
                .Select(x => x.Copy())
                .ToList();
    }

    public ToDoItem? Get(Guid id)
    {
        return _tasks.FirstOrDefault(x => x.Id == id)?.Copy();
    }

    public void Add(ToDoItem item)
    {
        _tasks.Add(item.Copy());
    }

    public void Update(ToDoItem item)
    {
        var index = _tasks.FindIndex(x => x.Id == item.Id);
        _tasks[index] = item.Copy();
    }

    public void Delete(Guid id)
    {
        _tasks.RemoveAll(x => x.Id == id);
    }

    public bool ExistsByName(Guid userId, string name)
    {
        return _tasks.Any(x => x.User.UserId == userId && x.Name == name);
    }

    public int CountActive(Guid userId)
    {
        return _tasks.Count(x => x.User.UserId == userId && x.State == ToDoItemState.Active);
    }

    public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
    {
        return _tasks.Where(x => x.User.UserId == userId)
                .Where(predicate)
                .Select(x => x.Copy())
                .ToList();
    }
}