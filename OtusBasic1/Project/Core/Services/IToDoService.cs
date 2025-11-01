namespace OtusBasic1;

public interface IToDoService
{
    IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
    IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
    IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);
    ToDoItem Add(ToDoUser user, string name);
    void MarkCompleted(Guid id);
    void Delete(Guid id);
}