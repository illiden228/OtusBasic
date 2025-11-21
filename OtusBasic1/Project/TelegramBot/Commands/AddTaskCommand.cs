using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class AddTaskCommand : BaseCommand, IConversationCommand
{
    private readonly IToDoService _toDoService;

    public AddTaskCommand(ICommandService commandService, IToDoService toDoService) : base(commandService)
    {
        _toDoService = toDoService;
    }

    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var input = update.Message.Text;
        var taskName = input.Replace("/addtask", "").Trim();
        if (string.IsNullOrWhiteSpace(taskName))
        {
            await CommandService.AddAwaitConversation(new CommandConversation(user, this, null), cancellationToken);
            await botClient.SendMessage(update.Message.Chat, $"Введите название задачи", replyMarkup: ReplyMarkup.RemoveKeyboard, cancellationToken:cancellationToken);
            return;
        }
        
        await CreateTask(taskName, user, botClient, update, cancellationToken);
    }

    public async Task ContinueConversation(ToDoUser? user, ITelegramBotClient botClient, Update update, ConversationContext context,
        CancellationToken cancellationToken = default)
    {
        var taskName = update.Message.Text;
        if (string.IsNullOrWhiteSpace(taskName))
        {
            await CommandService.AddAwaitConversation(new CommandConversation(user, this, null), cancellationToken);
            await botClient.SendMessage(update.Message.Chat, $"Неудачное имя, попробуйте еще раз", cancellationToken:cancellationToken);
            return;
        }
        
        await CreateTask(taskName, user, botClient, update, cancellationToken);
    }

    private async Task CreateTask(string taskName, ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var task = await _toDoService.Add(user, taskName, cancellationToken);
        var markup  = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(update.Message.Chat, $"Создана новая задача \"{task.Name}\"", replyMarkup: markup, cancellationToken:cancellationToken);
    }
}