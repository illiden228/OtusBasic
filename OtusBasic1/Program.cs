using Otus.ToDoList.ConsoleBot;
using OtusBasic1;

ITelegramBotClient botClient = new ConsoleBotClient();
IUserService userService = new UserService();
IToDoService toDoService = new ToDoService();
IUpdateHandler updateHandler = new UpdateHandler(userService, toDoService);

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