using OtusBasic1.Project.Core.DataAccess;
using OtusBasic1.Project.Core.Entities;

namespace OtusBasic1.Project.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
    {
        var user = new ToDoUser(telegramUserName, telegramUserId);
        await _repository.Add(user, ct);
    }

    public async Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct)
    {
        return await _repository.GetUserByTelegramUserId(telegramUserId, ct);
    }
}