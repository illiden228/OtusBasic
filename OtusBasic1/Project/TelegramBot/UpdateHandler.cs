using OtusBasic1.Project.Core.Exceptions;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace OtusBasic1.Project.TelegramBot;

public class UpdateHandler : IUpdateHandler
{
    public delegate void MessageEventHandler(string message);
    
    private readonly IUserService _userService;
    private readonly ICommandService _commandService;
    
    public event MessageEventHandler? OnHandleUpdateStarted;
    public event MessageEventHandler? OnHandleUpdateCompleted;

    public UpdateHandler(IUserService userService, ICommandService commandService)
    {
        _userService = userService;
        _commandService = commandService;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        try
        {
            string input = string.Empty;
            long fromId = 0;
            ChatId? chatId = null;
            
            switch (update.Type)
            {
                case UpdateType.Message:
                    input = update.Message!.Text;
                    fromId = update.Message!.From.Id;
                    chatId = update.Message.Chat.Id;
                    break;
                case UpdateType.CallbackQuery:
                    input = update.CallbackQuery.Data;
                    fromId = update.CallbackQuery.From.Id;
                    chatId = update.CallbackQuery.Message!.Chat.Id;
                    break;
            }
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var user = await _userService.GetUser(fromId, ct);

            OnHandleUpdateStarted?.Invoke(input);
            if (await _commandService.TryGet(parts[0], out var command, ct))
            {
                var canHandle = await command.Handler.CanExecuteHandle(user, botClient, update, ct);
                if (canHandle)
                {
                    await command.Handler.Execute(user, botClient, update, ct);
                }
            }
            else
            {
                bool isExecute = await _commandService.TryExecuteAwaitConversation(user, botClient, update, ct);
                if (!isExecute)
                {
                    var executableCommands = await _commandService.GetExecutableCommands(user, update, ct);
                    var markup = ToDoUtils.ConvertListToReplyKeyboard(executableCommands.Select(x => x.Name).ToList(), 1);
                    await botClient.SendMessage(chatId, $"У меня нет такой команды!", replyMarkup: markup, cancellationToken: ct);
                }
            }
            if(update.Type == UpdateType.CallbackQuery)
                await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            OnHandleUpdateCompleted?.Invoke(input);
        }
        catch (TaskCountLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
        }
        catch (TaskLengthLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
        }
        catch (DuplicateTaskException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
        }
        catch (ArgumentException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"Произошла непредвиденная ошибка: {ex.GetType().FullName}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException}");
        return Task.CompletedTask;
    }
}