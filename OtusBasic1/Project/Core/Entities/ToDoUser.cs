namespace OtusBasic1.Project.Core.Entities;

public class ToDoUser
{
    public Guid UserId { get; private set; }
    public string TelegramUserName { get; private set; }
    public long TelegramUserId { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    public ToDoUser(string telegramUserName, long telegramUserId)
    {
        TelegramUserName = telegramUserName;
        TelegramUserId = telegramUserId;
        Init();
    }

    private void Init()
    {
        UserId = Guid.NewGuid();
        RegisteredAt = DateTime.UtcNow;
    }
}