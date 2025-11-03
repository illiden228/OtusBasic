namespace OtusBasic1.Project.Core.Entities;

public class ToDoItem
{
    public Guid Id { get; private set; }
    public ToDoUser User { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ToDoItemState State { get; private set; }
    public DateTime? StateChangedAt { get; private set; }

    public ToDoItem(ToDoUser user, string name)
    {
        User = user;
        Name = name;

        Init();
    }

    public ToDoItem(ToDoItem item)
    {
        Id = item.Id;
        User = item.User;
        Name = item.Name;
        CreatedAt = item.CreatedAt;
        State = item.State;
        StateChangedAt = item.StateChangedAt;
    }

    private void Init()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        State = ToDoItemState.Active;
    }

    public void Complete()
    {
        StateChangedAt = DateTime.UtcNow;
        State = ToDoItemState.Completed;
    }

    public ToDoItem Copy()
    {
        return new ToDoItem(this);
    }
}