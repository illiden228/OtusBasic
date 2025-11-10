using OtusBasic1.Project.Core.Entities;

namespace OtusBasic1.Project.Core.DataAccess;

public interface IUserRepository
{
    Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct);
    Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
    Task Add(ToDoUser user, CancellationToken ct);
}