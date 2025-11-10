using Otus.ToDoList.ConsoleBot;
using OtusBasic1.Project.Core.DataAccess;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.Infrastructure.DataAccess;
using OtusBasic1.Project.TelegramBot;

using var cts = new CancellationTokenSource();
ITelegramBotClient botClient = new ConsoleBotClient();
IUserRepository userRepository = new InMemoryUserRepository();
IUserService userService = new UserService(userRepository);
IToDoRepository toDoRepository = new InMemoryToDoRepository();
IToDoService toDoService = new ToDoService(toDoRepository);
IToDoReportService reportService = new ToDoReportService(toDoRepository);
UpdateHandler updateHandler = new UpdateHandler(userService, toDoService, reportService);

try
{
    updateHandler.OnHandleUpdateStarted += OnHandleUpdateStartedHandle;
    updateHandler.OnHandleUpdateCompleted += OnHandleUpdateCompletedHandle;

    botClient.StartReceiving(updateHandler, cts.Token);
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