using System.Text;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Exceptions;
using OtusBasic1.Project.Core.Services;

namespace OtusBasic1.Project.TelegramBot;

public class UpdateHandler : IUpdateHandler
{
    public delegate void MessageEventHandler(string message);
    
    private readonly IUserService _userService;
    private readonly IToDoService _toDoService;
    private readonly IToDoReportService _reportService;
    private string version = "0.0.4";
    private string dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");
    
    public event MessageEventHandler? OnHandleUpdateStarted;
    public event MessageEventHandler? OnHandleUpdateCompleted;

    public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService reportService)
    {
        _userService = userService;
        _toDoService = toDoService;
        _reportService = reportService;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        try
        {
            string input = update.Message.Text;
            var user = await _userService.GetUser(update.Message.From.Id, ct);

            OnHandleUpdateStarted?.Invoke(input);
            if (input.StartsWith("/start"))
            {
                if (user == null)
                {
                    user = await _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username ?? "User", ct);
                    await botClient.SendMessage(update.Message.Chat,
                        $"Регистрация пройдена! Добро пожаловать, {user.TelegramUserName}!", ct);
                }
                else
                {
                    await botClient.SendMessage(update.Message.Chat, $"Регистрация уже пройдена, {user.TelegramUserName}!", ct);
                }

                await botClient.SendMessage(update.Message.Chat, $"Список команд: {GetHelp()}", ct);
                OnHandleUpdateCompleted?.Invoke(input);
                return;
            }

            if (user == null)
            {
                await botClient.SendMessage(update.Message.Chat, $"Здравствуйте! Введите /start, чтобы начать", ct);
                OnHandleUpdateCompleted?.Invoke(input);
                return;
            }

            if (input.StartsWith("/help"))
            {
                await botClient.SendMessage(update.Message.Chat, GetHelp(), ct);
            }
            else if (input.StartsWith("/info"))
            {
                await botClient.SendMessage(update.Message.Chat, $"Программа создана {dateOfCreation}\nТекущая версия: v.{version}\n", ct);
            }
            else if (input.StartsWith("/addtask"))
            {
                var taskName = input.Replace("/addtask", "").Trim();
                var task = await _toDoService.Add(user, taskName, ct);
                await botClient.SendMessage(update.Message.Chat, $"Создана новая задача \"{task.Name}\"", ct);
            }
            else if (input.StartsWith("/showtasks"))
            {
                var activeTasks = await _toDoService.GetActiveByUserId(user.UserId, ct);
                var tasks = LineTasks(activeTasks, false);
                if (string.IsNullOrWhiteSpace(tasks))
                    tasks = "У вас нет задач";
                await botClient.SendMessage(update.Message.Chat, tasks, ct);
            }
            else if (input.StartsWith("/showalltasks"))
            {
                var allTasks = await _toDoService.GetAllByUserId(user.UserId, ct);
                var tasks = LineTasks(allTasks);
                if (string.IsNullOrWhiteSpace(tasks))
                    tasks = "У вас нет задач";
                await botClient.SendMessage(update.Message.Chat, tasks, ct);
            }
            else if (input.StartsWith("/removetask"))
            {
                var taskId = input.Replace("/removetask", "").Trim();
                if (!Guid.TryParse(taskId, out Guid taskGuid))
                {
                    throw new ArgumentException("Некорректный Id задачи");
                }
                await _toDoService.Delete(taskGuid, ct);
                await botClient.SendMessage(update.Message.Chat, $"Удалена задача \"{taskId}\"", ct);
            }
            else if (input.StartsWith("/completetask"))
            {
                var taskId = input.Replace("/completetask", "").Trim();
                if (!Guid.TryParse(taskId, out Guid taskGuid))
                {
                    throw new ArgumentException("Некорректный Id задачи");
                }
                await _toDoService.MarkCompleted(taskGuid, ct);
                await botClient.SendMessage(update.Message.Chat, $"Задача \"{taskId}\" выполнена", ct);
            }
            else if (input.StartsWith("/report"))
            {
                var report = await _reportService.GetUserStats(user.UserId, ct);
                await botClient.SendMessage(update.Message.Chat, $"Статистика по задачам на {report.generatedAt}. Всего: {report.total}; Звершенных: {report.completed}; Активных: {report.active}", ct);
            }
            else if (input.StartsWith("/find"))
            {
                var namePrefix = input.Replace("/find", "").Trim();
                var tasks = await _toDoService.Find(user, namePrefix, ct);
                var lineTasks = LineTasks(tasks);
                if (string.IsNullOrWhiteSpace(lineTasks))
                    lineTasks = $"У вас нет задач, начинающихся на {namePrefix}";
                await botClient.SendMessage(update.Message.Chat, lineTasks, ct);
            }
            else
            {
                await botClient.SendMessage(update.Message.Chat, $"У меня нет такой команды. Вот какие есть: {GetHelp()}", ct);
            }
            
            OnHandleUpdateCompleted?.Invoke(input);
        }
        catch (TaskCountLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
        catch (TaskLengthLimitException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
        catch (DuplicateTaskException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
        catch (ArgumentException ex)
        {
            await botClient.SendMessage(update.Message.Chat, ex.Message, ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken ct)
    {
        Console.WriteLine(
            $"Произошла непредвиденная ошибка: {ex.GetType().FullName}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException}");
        return Task.CompletedTask;
    }

    string GetHelp()
    {
        return "\n/help - инструкция\n" +
               "/info - информация о программе\n" +
               "/addtask - добавить задачу в список\n" +
               "/showtasks - посмотреть список активных задач\n" +
               "/showalltasks - посмотреть список всех задач\n" +
               "/completetask - выполнить задачу с указанным индексом\n" +
               "/removetask - удалить задачу\n" +
               "/find - поиск по имени\n" +
               "/report - составить отчет по задачам\n";
    }

    string LineTasks(IReadOnlyList<ToDoItem> tasks, bool showState = true)
    {
        StringBuilder stringBuilder = new();
        for (int i = 0; i < tasks.Count; i++)
        {
            stringBuilder.Append($"{i + 1}. ");
            if (showState)
            {
                stringBuilder.Append($"({tasks[i].State.ToString()}) ");
            }

            var task = tasks[i];

            stringBuilder.Append($"{task.Name} - {task.CreatedAt} - {task.Id}\n");
        }

        return stringBuilder.ToString();
    }
}