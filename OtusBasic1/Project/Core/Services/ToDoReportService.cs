namespace OtusBasic1;

public class ToDoReportService : IToDoReportService
{
    private IToDoRepository _repository;

    public ToDoReportService(IToDoRepository repository)
    {
        _repository = repository;
    }

    public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken ct)
    {
        var tasks = await _repository.GetAllByUserId(userId, ct);
        var total = tasks.Count();
        int completed = tasks.Count(x => x.State == ToDoItemState.Completed);
        int active = await _repository.CountActive(userId,ct);
        DateTime generatedAt = DateTime.UtcNow;
        return (total, completed, active, generatedAt);
    }
}