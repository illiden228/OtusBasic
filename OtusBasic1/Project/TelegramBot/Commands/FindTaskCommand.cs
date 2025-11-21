using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class FindTaskCommand : BaseCommand, IConversationCommand
{
    private readonly IToDoService _toDoService;

    public FindTaskCommand(ICommandService commandService, IToDoService toDoService) : base(commandService)
    {
        _toDoService = toDoService;
    }
    
    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var input = update.Message.Text;
        var namePrefix = input.Replace("/find", "").Trim();
       
        if (string.IsNullOrWhiteSpace(namePrefix))
        {
            await CommandService.AddAwaitConversation(new CommandConversation(user, this, null), cancellationToken);
            await botClient.SendMessage(update.Message.Chat, $"Введите название задачи", replyMarkup: ReplyMarkup.RemoveKeyboard, cancellationToken:cancellationToken);
            return;
        }
        
        await FindTask(namePrefix, user, botClient, update, cancellationToken);
    }
    
    public async Task ContinueConversation(ToDoUser? user, ITelegramBotClient botClient, Update update, ConversationContext context,
        CancellationToken cancellationToken = default)
    {
        var namePrefix = update.Message.Text;
        await FindTask(namePrefix, user, botClient, update, cancellationToken);
    }
    
    private async Task FindTask(string namePrefix, ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _toDoService.Find(user, namePrefix, cancellationToken);
        var lineTasks = ToDoUtils.LineTasks(tasks);
        if (string.IsNullOrWhiteSpace(lineTasks))
            lineTasks = $"У вас нет задач, начинающихся на {namePrefix}";
        var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(update.Message.Chat, lineTasks, replyMarkup: markup, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);
    }
}