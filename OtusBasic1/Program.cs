using OtusBasic1.Project.Core.DataAccess;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.Infrastructure.DataAccess;
using OtusBasic1.Project.TelegramBot;
using OtusBasic1.Project.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

string version = "0.0.5";
string dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");
string token = "8344142896:AAGARJ3TW5pHIqVdlGslqhOcKTr-ne51TUw";
using var cts = new CancellationTokenSource();
ITelegramBotClient botClient = new TelegramBotClient(token);
IUserRepository userRepository = new InMemoryUserRepository();
IUserService userService = new UserService(userRepository);
IToDoRepository toDoRepository = new InMemoryToDoRepository();
IToDoService toDoService = new ToDoService(toDoRepository);
IToDoReportService reportService = new ToDoReportService(toDoRepository);
ICommandService commandService = new CommandService(userService, toDoService, reportService, version, dateOfCreation);
UpdateHandler updateHandler = new UpdateHandler(userService, commandService);

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>(),
    DropPendingUpdates = true
};

try
{
    updateHandler.OnHandleUpdateStarted += OnHandleUpdateStartedHandle;
    updateHandler.OnHandleUpdateCompleted += OnHandleUpdateCompletedHandle;
    
    await botClient.SetMyCommands(commandService.GetCommands().Select(x => new BotCommand(x.Name, x.Description)));
    botClient.StartReceiving(updateHandler, receiverOptions, cts.Token);
    
    Console.WriteLine($"Для выхода из приложения нажмите клавишу A");

    while (Console.ReadKey().Key != ConsoleKey.A) { }
    cts.Cancel();
}
finally
{
    updateHandler.OnHandleUpdateStarted -= OnHandleUpdateStartedHandle;
    updateHandler.OnHandleUpdateCompleted -= OnHandleUpdateCompletedHandle;
} 

void OnHandleUpdateStartedHandle(string message)
{
    Console.WriteLine($"Началась обработка сообщения '{message}'");
}

void OnHandleUpdateCompletedHandle(string message)
{
    Console.WriteLine($"Закончилась обработка сообщения '{message}'");
}