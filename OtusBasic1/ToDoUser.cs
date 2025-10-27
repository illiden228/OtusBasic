namespace OtusBasic1;

public class ToDoUser
{
    public Guid UserId { get; private set; }
    public string TelegramUserName { get; private set; }
    public DateTime RegisteredAt { get; private set; }

    public ToDoUser()
    {
        Init();
    }

    public ToDoUser(string telegramUserName)
    {
        TelegramUserName = telegramUserName;
        Init();
    }

    private void Init()
    {
        UserId = Guid.NewGuid();
        RegisteredAt = DateTime.UtcNow;
    }
}