using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class StartBaseCommand : BaseCommand
{
    private readonly IUserService _userService;

    public StartBaseCommand(ICommandService commandService, IUserService userService) : base(commandService)
    {
        _userService = userService;
    }

    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        if (user == null)
        {
            await _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username ?? "User", cancellationToken);
            user = await _userService.GetUser(update.Message.From.Id, cancellationToken);
            
            var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
            await botClient.SendMessage(update.Message.Chat,
                $"Регистрация пройдена! Добро пожаловать, {user.TelegramUserName}!", 
                replyMarkup: markup,
                cancellationToken: cancellationToken);
        }
    }

    public override async Task<bool> CanExecuteHandle(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var canExecute = await CanExecute(user, update, cancellationToken);
        if (!canExecute)
        {
            await botClient.SendMessage(update.Message.Chat, $"Регистрация уже пройдена, {user.TelegramUserName}!", cancellationToken: cancellationToken);
        }
        return canExecute;
    }

    public override Task<bool> CanExecute(ToDoUser? user, Update update, CancellationToken cancellationToken = default)
    {
        var canExecute = user == null;
        return Task.FromResult(canExecute);
    }
}