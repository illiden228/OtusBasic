using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class InfoCommand : BaseCommand
{
    private readonly string _version;
    private readonly string _dateOfCreation;

    public InfoCommand(ICommandService commandService, string version, string dateOfCreation) : base(commandService)
    {
        _version = version;
        _dateOfCreation = dateOfCreation;
    }

    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(update.Message.Chat, $"Программа создана {_dateOfCreation}\nТекущая версия: v.{_version}\n", replyMarkup: markup, cancellationToken: cancellationToken);
    }
}