using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OtusBasic1.Project.TelegramBot.Services;

public class CommandService : ICommandService
{
    private readonly IUserService _userService;
    private readonly IToDoService _toDoService;
    private readonly IToDoReportService _reportService;
    private readonly string _version = "0.0.5";
    private readonly string _dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");
    private readonly Dictionary<string, TelegramBotCommand> _commands = new();
    private HashSet<CommandConversation> _awaitCommands = new();

    public CommandService(IUserService userService, IToDoService toDoService, IToDoReportService reportService, string version, string dateOfCreation)
    {
        _userService = userService;
        _toDoService = toDoService;
        _reportService = reportService;
        _version = version;
        _dateOfCreation = dateOfCreation;
        
        _commands.Add("/start", new TelegramBotCommand("/start", "регистрация", new StartBaseCommand(this, _userService)));
        _commands.Add("/info", new TelegramBotCommand("/info", "информация о боте", new InfoCommand(this, _version, _dateOfCreation)));
        _commands.Add("/addtask", new TelegramBotCommand("/addtask", "добавить задачу в список", new AddTaskCommand(this, _toDoService)));
        _commands.Add("/showtasks", new TelegramBotCommand("/showtasks", "посмотреть список активных задач", new ShowTasksCommand(this, _toDoService)));
        _commands.Add("/showalltasks", new TelegramBotCommand("/showalltasks", "посмотреть список всех задач", new ShowAllTasksCommand(this, _toDoService)));
        _commands.Add("/removetask", new TelegramBotCommand("/removetask", "удалить задачу", new RemoveTaskCommand(this, _toDoService)));
        _commands.Add("/completetask", new TelegramBotCommand("/completetask", "выполнить задачу с указанным индексом", new CompleteTaskCommand(this, _toDoService)));
        _commands.Add("/find", new TelegramBotCommand("/find", "поиск по имени", new FindTaskCommand(this, _toDoService)));
        _commands.Add("/report", new TelegramBotCommand("/report", "составить отчет по задачам", new ReportCommand(this, _reportService)));
        _commands.Add("/cancel", new TelegramBotCommand("/cancel", "отменить выполняемую комманду", new CancelCommand(this)));
    }

    public IReadOnlyList<TelegramBotCommand> GetCommands() => _commands.Select(x => x.Value).ToList();

    public async Task<List<TelegramBotCommand>> GetExecutableCommands(ToDoUser? user, Update update, CancellationToken ct)
    {
        var commands = GetCommands();
        var executableCommands = new List<TelegramBotCommand>();
        
        foreach (var command in commands)
        {
            var canExecute = await command.Handler.CanExecute(user, update, ct);
            if(!canExecute)
                continue;
            
            executableCommands.Add(command);
        }
        
        return executableCommands;
    }

    public Task<bool> TryGet(string commandName, out TelegramBotCommand command, CancellationToken ct) => Task.FromResult(_commands.TryGetValue(commandName, out command));
    
    public Task AddAwaitConversation(CommandConversation context, CancellationToken ct)
    {
        if(_awaitCommands.Contains(context))
            _awaitCommands.Remove(context);
        
        _awaitCommands.Add(context);
        return Task.CompletedTask;
    }

    public Task RemoveAwaitConversation(ToDoUser? user, CancellationToken ct)
    {
        var command = _awaitCommands.FirstOrDefault(x => x.User == user);
        if (command != null)
        {
            _awaitCommands.Remove(command);
        }
        return Task.CompletedTask;
    }

    public async Task<bool> TryExecuteAwaitConversation(ToDoUser? user, ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var command = _awaitCommands.FirstOrDefault(x => x.User == user);
        bool canExecute = command != null;
        
        if (canExecute)
        {
            await command.Command.ContinueConversation(user, botClient, update, command.Context, ct);
            _awaitCommands.Remove(command);
        }
        
        return canExecute;
    }

    public Task<bool> ExistAwaitConversationByUser(ToDoUser? user, CancellationToken ct)
    {
        var contains = _awaitCommands.Any(x => x.User == user);
        return Task.FromResult(contains);
    }
}