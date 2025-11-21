using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class CancelCommand : BaseCommand
{
    public CancelCommand(ICommandService commandService) : base(commandService)
    {
    }

    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var chatId = ToDoUtils.GetChatId(update);
        await CommandService.RemoveAwaitConversation(user, cancellationToken);
        var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(chatId, $"Команда успешно отменена", replyMarkup: markup, cancellationToken: cancellationToken);
    }

    public override async Task<bool> CanExecuteHandle(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var canHandle = await CanExecute(user, update, cancellationToken);
        if (!canHandle)
        {
            var chatId = ToDoUtils.GetChatId(update);
            await botClient.SendMessage(chatId, $"У вас сейчас нечего отменять", cancellationToken: cancellationToken);
        }
        return canHandle;
    }

    public override Task<bool> CanExecute(ToDoUser? user, Update update, CancellationToken cancellationToken = default)
    {
        return CommandService.ExistAwaitConversationByUser(user, cancellationToken);
    }
}