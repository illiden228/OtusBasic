namespace OtusBasic1.Project.TelegramBot.Commands;

public class TelegramBotCommand
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public BaseCommand Handler { get; private set; }

    public TelegramBotCommand(string name, string description, BaseCommand handler)
    {
        Name = name;
        Description = description;
        Handler = handler;
    }
}