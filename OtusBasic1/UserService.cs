namespace OtusBasic1;

public class UserService : IUserService
{
    private readonly Dictionary<long, ToDoUser> _users = new();
    
    public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
    {
        var user = new ToDoUser(telegramUserName, telegramUserId);
        _users.Add(telegramUserId, user);
        return user;
    }

    public ToDoUser? GetUser(long telegramUserId)
    {
        if(_users.TryGetValue(telegramUserId, out var user))
            return user;
        
        return null;
    }
}