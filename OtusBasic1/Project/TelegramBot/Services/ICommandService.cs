using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OtusBasic1.Project.TelegramBot.Services;

public interface ICommandService
{
    IReadOnlyList<TelegramBotCommand> GetCommands();
    Task<List<TelegramBotCommand>> GetExecutableCommands(ToDoUser? user, Update update, CancellationToken ct);
    Task<bool> TryGet(string commandName, out TelegramBotCommand command, CancellationToken ct);
    Task AddAwaitConversation(CommandConversation context, CancellationToken ct);
    Task RemoveAwaitConversation(ToDoUser? user, CancellationToken ct);
    Task<bool> TryExecuteAwaitConversation(ToDoUser? user, ITelegramBotClient botClient, Update update, CancellationToken ct);
    Task<bool> ExistAwaitConversationByUser(ToDoUser? user, CancellationToken ct);
}