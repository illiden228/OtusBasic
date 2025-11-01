using Otus.ToDoList.ConsoleBot;
using OtusBasic1;

ITelegramBotClient botClient = new ConsoleBotClient();
IUserRepository userRepository = new InMemoryUserRepository();
IUserService userService = new UserService(userRepository);
IToDoRepository toDoRepository = new InMemoryToDoRepository();
IToDoService toDoService = new ToDoService(toDoRepository);
IToDoReportService reportService = new ToDoReportService(toDoRepository);
IUpdateHandler updateHandler = new UpdateHandler(userService, toDoService, reportService);

try
{
    while (true)
    {
        botClient.StartReceiving(updateHandler);
    }
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Произошла непредвиденная ошибка: {ex.GetType().FullName}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException}");
}