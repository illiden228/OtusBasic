namespace OtusBasic1;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<ToDoUser> users = new();
    
    public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
    {
        return users.FirstOrDefault(x => x.UserId == userId);
    }

    public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
    {
        return users.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
    }

    public async Task Add(ToDoUser user, CancellationToken ct)
    {
        users.Add(user);
    }
}