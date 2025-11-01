namespace OtusBasic1;

public class ToDoReportService : IToDoReportService
{
    private IToDoRepository _repository;

    public ToDoReportService(IToDoRepository repository)
    {
        _repository = repository;
    }

    public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
    {
        var tasks = _repository.GetAllByUserId(userId);
        var total = tasks.Count();
        int completed = tasks.Count(x => x.State == ToDoItemState.Completed);
        int active = _repository.CountActive(userId);
        DateTime generatedAt = DateTime.UtcNow;
        return (total, completed, active, generatedAt);
    }
}