using System.Text;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace OtusBasic1;

public class UpdateHandler : IUpdateHandler
{
    private readonly IUserService _userService;
    private readonly IToDoService _toDoService;
    private string version = "0.0.3";
    private string dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");

    public UpdateHandler(IUserService userService, IToDoService toDoService)
    {
        _userService = userService;
        _toDoService = toDoService;
    }

    public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
    {
        try
        {
            string input = update.Message.Text;
            var user = _userService.GetUser(update.Message.From.Id);

            if (input.StartsWith("/start"))
            {
                if (user == null)
                {
                    user = _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username ?? "User");
                    botClient.SendMessage(update.Message.Chat,
                        $"Регистрация пройдена! Добро пожаловать, {user.TelegramUserName}!");
                }
                else
                {
                    botClient.SendMessage(update.Message.Chat, $"Регистрация уже пройдена, {user.TelegramUserName}!");
                }

                botClient.SendMessage(update.Message.Chat, $"Список команд: {GetHelp()}");
                return;
            }

            if (user == null)
            {
                botClient.SendMessage(update.Message.Chat, $"Здравствуйте! Введите /start, чтобы начать");
                return;
            }

            if (input.StartsWith("/help"))
            {
                botClient.SendMessage(update.Message.Chat, GetHelp());
                return;
            }

            if (input.StartsWith("/info"))
            {
                botClient.SendMessage(update.Message.Chat, $"Программа создана {dateOfCreation}\nТекущая версия: v.{version}\n");
                return;
            }

            if (input.StartsWith("/addtask"))
            {
                var taskName = input.Replace("/addtask", "").Trim();
                var task = _toDoService.Add(user, taskName);
                botClient.SendMessage(update.Message.Chat, $"Создана новая задача \"{task.Name}\"");
                return;
            }

            if (input.StartsWith("/showtasks"))
            {
                var activeTasks = _toDoService.GetActiveByUserId(user.UserId);
                var tasks = LineTasks(activeTasks, false);
                if (string.IsNullOrWhiteSpace(tasks))
                    tasks = "У вас нет задач";
                botClient.SendMessage(update.Message.Chat, tasks);
                return;
            }

            if (input.StartsWith("/showalltasks"))
            {
                var allTasks = _toDoService.GetAllByUserId(user.UserId);
                var tasks = LineTasks(allTasks);
                if (string.IsNullOrWhiteSpace(tasks))
                    tasks = "У вас нет задач";
                botClient.SendMessage(update.Message.Chat, tasks);
                return;
            }

            if (input.StartsWith("/removetask"))
            {
                var taskId = input.Replace("/removetask", "").Trim();
                if (!Guid.TryParse(taskId, out Guid taskGuid))
                {
                    throw new ArgumentException("Некорректный Id задачи");
                }
                _toDoService.Delete(taskGuid);
                botClient.SendMessage(update.Message.Chat, $"Удалена задача \"{taskId}\"");
                return;
            }

            if (input.StartsWith("/completetask"))
            {
                var taskId = input.Replace("/completetask", "").Trim();
                if (!Guid.TryParse(taskId, out Guid taskGuid))
                {
                    throw new ArgumentException("Некорректный Id задачи");
                }
                _toDoService.MarkCompleted(taskGuid);
                botClient.SendMessage(update.Message.Chat, $"Задача \"{taskId}\" выполнена");
                return;
            }
            
            botClient.SendMessage(update.Message.Chat, $"У меня нет такой команды. Вот какие есть: {GetHelp()}");
        }
        catch (TaskCountLimitException ex)
        {
            botClient.SendMessage(update.Message.Chat, ex.Message);
        }
        catch (TaskLengthLimitException ex)
        {
            botClient.SendMessage(update.Message.Chat, ex.Message);
        }
        catch (DuplicateTaskException ex)
        {
            botClient.SendMessage(update.Message.Chat, ex.Message);
        }
        catch (ArgumentException ex)
        {
            botClient.SendMessage(update.Message.Chat, ex.Message);
        }
    }

    string GetHelp()
    {
        return "\n/help - инструкция\n" +
               "/info - информация о программе\n" +
               "/addtask - добавить задачу в список\n" +
               "/showtasks - посмотреть список активных задач\n" +
               "/showalltasks - посмотреть список всех задач\n" +
               "/completetask - выполнить задачу с указанным индексом\n" +
               "/removetask - удалить задачу\n";
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