using OtusBasic1.Project.Core.DataAccess;
using OtusBasic1.Project.Core.Entities;

namespace OtusBasic1.Project.Infrastructure.DataAccess;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<ToDoUser?> _users = new();
    
    public Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
    {
        var result = _users.FirstOrDefault(x => x.UserId == userId);
        return Task.FromResult(result);
    }

    public Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
    {
        var result = _users.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
        return Task.FromResult(result);
    }

    public Task Add(ToDoUser? user, CancellationToken ct)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
}