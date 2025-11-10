using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusBasic1.Project.TelegramBot.Commands;

public abstract class BaseCommand
{
    protected readonly ICommandService CommandService;

    protected BaseCommand(ICommandService commandService)
    {
        CommandService = commandService;
    }

    public abstract Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default);

    public async virtual Task<bool> CanExecuteHandle(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var canHandle = await CanExecute(user, update, cancellationToken);
        if (!canHandle)
        {
            var markup  = await GetCurrentTasksKeyboard(user, update, cancellationToken);
            await botClient.SendMessage(update.Message.Chat, $"Здравствуйте! Введите /start, чтобы начать", replyMarkup: markup, cancellationToken: cancellationToken);
        }
        return canHandle;
    }

    public virtual Task<bool> CanExecute(ToDoUser? user, Update update, CancellationToken cancellationToken = default)
    {
        var canHandle = user != null;
        return Task.FromResult(canHandle);
    }

    protected async Task<ReplyKeyboardMarkup> GetCurrentTasksKeyboard(ToDoUser? user, Update update, CancellationToken cancellationToken = default)
    {
        var executableCommands = await CommandService.GetExecutableCommands(user, update, cancellationToken);
        var markup = ToDoUtils.ConvertListToReplyKeyboard(executableCommands.Select(x => x.Name).ToList(), 1);
        return markup;
    }
}