namespace OtusBasic1;

public interface IToDoReportService
{
    Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken ct);
}