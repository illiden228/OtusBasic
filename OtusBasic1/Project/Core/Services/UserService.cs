namespace OtusBasic1;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
    {
        var user = new ToDoUser(telegramUserName, telegramUserId);
        _repository.Add(user);
        return user;
    }

    public ToDoUser? GetUser(long telegramUserId)
    {
        return _repository.GetUserByTelegramUserId(telegramUserId);
    }
}