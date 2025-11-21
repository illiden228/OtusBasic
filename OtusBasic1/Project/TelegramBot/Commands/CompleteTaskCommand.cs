using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class CompleteTaskCommand : BaseCommand, IConversationCommand
{
    private readonly IToDoService _toDoService;

    public CompleteTaskCommand(ICommandService commandService, IToDoService toDoService) : base(commandService)
    {
        _toDoService = toDoService;
    }
    
    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var input = update.Message.Text;
        var taskId = input.Replace("/completetask", "").Trim();

        if (string.IsNullOrWhiteSpace(taskId))
        {
            await CommandService.AddAwaitConversation(new CommandConversation(user, this, null), cancellationToken);
            var tasks = await _toDoService.GetActiveByUserId(user.UserId, cancellationToken);
            var buttons = tasks.Select(x => (x.Name.Substring(0, x.Name.Length <= 15 ? x.Name.Length : 15), x.Id.ToString())).ToList();
            buttons.Add(("Отмена", "/cancel"));
            var markup = ToDoUtils.ConvertListToInlineKeyboard(buttons, 1);
            await botClient.SendMessage(update.Message.Chat, $"Введите ID задачи", replyMarkup: markup, cancellationToken:cancellationToken);
            return;
        }
        
        if (!Guid.TryParse(taskId, out Guid taskGuid))
        {
            throw new ArgumentException("Некорректный Id задачи");
        }
         
        await CompleteTask(taskGuid, user, botClient, update, cancellationToken);
    }

    public async Task ContinueConversation(ToDoUser? user, ITelegramBotClient botClient, Update update, ConversationContext context,
        CancellationToken cancellationToken = default)
    {
        var taskId = string.Empty;

        switch (update.Type)
        {
            case UpdateType.Message:
                taskId = update.Message.Text;
                break;
            case UpdateType.CallbackQuery:
                taskId = update.CallbackQuery.Data;
                break;
        }
        
        if (!Guid.TryParse(taskId, out Guid taskGuid))
        {
            throw new ArgumentException("Некорректный Id задачи");
        }
        
        await CompleteTask(taskGuid, user, botClient, update, cancellationToken);
    }
    
    private async Task CompleteTask(Guid taskId, ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var chatId = ToDoUtils.GetChatId(update);
        await _toDoService.MarkCompleted(taskId, cancellationToken);
        var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(chatId, $"Задача `{taskId}` выполнена", replyMarkup: markup, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
    }
}