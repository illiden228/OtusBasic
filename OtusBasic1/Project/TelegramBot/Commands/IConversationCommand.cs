using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OtusBasic1.Project.TelegramBot.Commands;

public interface IConversationCommand
{
    Task ContinueConversation(ToDoUser? user, ITelegramBotClient botClient, Update update, ConversationContext context, CancellationToken cancellationToken = default);
}