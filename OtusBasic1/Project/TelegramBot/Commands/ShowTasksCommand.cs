using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class ShowTasksCommand : BaseCommand
{
    private readonly IToDoService _toDoService;

    public ShowTasksCommand(ICommandService commandService, IToDoService toDoService) : base(commandService)
    {
        _toDoService = toDoService;
    }
    
    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var activeTasks = await _toDoService.GetActiveByUserId(user.UserId, cancellationToken);
        var tasks = ToDoUtils.LineTasks(activeTasks, false);
        if (string.IsNullOrWhiteSpace(tasks))
            tasks = "У вас нет задач";
        
        var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(update.Message.Chat, tasks, replyMarkup: markup, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
    }
}