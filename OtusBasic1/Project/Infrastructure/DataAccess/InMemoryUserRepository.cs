namespace OtusBasic1;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<ToDoUser> users = new();
    
    public ToDoUser? GetUser(Guid userId)
    {
        return users.FirstOrDefault(x => x.UserId == userId);
    }

    public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
    {
        return users.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
    }

    public void Add(ToDoUser user)
    {
        users.Add(user);
    }
}