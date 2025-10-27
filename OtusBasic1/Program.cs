using System.Text;
using OtusBasic1;

bool isWorking = true;
string? input = string.Empty;
ToDoUser user = null;
string version = "0.0.2";
string dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");
List<ToDoItem> tasks = new();

int taskCountLimit = 0;
int taskLengthLimit = 0;

Console.WriteLine("Добро пожаловать!");

try
{
    taskCountLimit = AskForIntWithRetry("Введите максимально допустимое количество задач (1..100): ", 1, 100);
    taskLengthLimit = AskForIntWithRetry("Введите максимально допустимую длину задачи (1..100): ", 1, 100);
    Console.WriteLine();

    OutputHelp();

    while (isWorking)
    {
        try
        {
            input = GetInput("Введите команду: \n");

            switch (input)
            {
                case "/start":
                    HandleStart();
                    break;

                case "/help":
                    HandleHelp();
                    break;

                case "/info":
                    HandleInfo();
                    break;

                case "/addtask":
                    HandleAddTask();
                    break;

                case "/showtasks":
                    HandleShowTasks();
                    break;

                case "/showalltasks":
                    HandleShowAllTasks();
                    break;

                case "/removetask":
                    HandleRemoveTask();
                    break;

                case "/exit":
                    HandleExit();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(input) && input.Contains("/echo"))
            {
                HandleEcho();
            }
            
            if (!string.IsNullOrWhiteSpace(input) && input.Contains("/completetask"))
            {
                HandleCompleteTask(input.Replace("/completetask ", ""));
            }
        }
        catch (TaskCountLimitException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (TaskLengthLimitException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (DuplicateTaskException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.GetType().FullName}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException}");
}

string? GetInput(string output)
{
    if (!string.IsNullOrWhiteSpace(output))
        Console.Write(output);

    return Console.ReadLine();
}

void OutputHelp()
{
    Console.WriteLine("/start - ввести имя\n" +
                      "/help - инструкция\n" +
                      "/info - информация о программе\n" +
                      "/exit - выход\n" +
                      "/addtask - добавить задачу в список\n" +
                      "/showtasks - посмотреть список активных задач\n" +
                      "/showalltasks - посмотреть список всех задач\n" +
                      "/completetask - выполнить задачу с указанным индексом\n" +
                      "/removetask - удалить задачу");

    if (user != null && !string.IsNullOrWhiteSpace(user.TelegramUserName))
        Console.WriteLine("/echo - вывод написанного аргумента");

    Console.WriteLine();
}

void OutputInfo()
{
    Console.WriteLine($"Программа создана {dateOfCreation}\nТекущая версия: v.{version}\n");
}

bool HasTasks()
{
    return tasks.Count != 0;
}

void ShowTasks(bool ignoreCompleted = true)
{
    for (int i = 0; i < tasks.Count; i++)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"{i + 1}. ");
        if (ignoreCompleted)
        {
            if(tasks[i].State == ToDoItemState.Completed)
                continue;
        }
        else
        {
            stringBuilder.Append($"({tasks[i].State.ToString()}) ");
        }
        
        var task = tasks[i];
        
        stringBuilder.Append($"{task.Name} - {task.CreatedAt} - {task.Id}");
        
        Console.WriteLine(stringBuilder.ToString());
    }

    Console.WriteLine();
}

void RemoveTask(int number)
{
    if (number < 0 || number >= tasks.Count)
    {
        Console.WriteLine("Такого номера не существует!");
        return;
    }

    tasks.RemoveAt(number);
    Console.WriteLine("Задача удалена!\n");
}

int ParseAndValidateInt(string? str, int min, int max)
{
    if (!int.TryParse(str, out int value))
        throw new ArgumentException("Ожидалось целое число.");

    if (value < min || value > max)
        throw new ArgumentException($"Число должно быть в диапазоне от {min} до {max}.");

    return value;
}

void ValidateString(string? str)
{
    if (str is null)
        throw new ArgumentException("Строка не должна быть null.");

    if (str.Length == 0)
        throw new ArgumentException("Строка не должна быть пустой.");

    if (string.IsNullOrWhiteSpace(str))
        throw new ArgumentException("Строка должна содержать символы, отличные от пробелов.");
}

int AskForIntWithRetry(string prompt, int min, int max)
{
    while (true)
    {
        try
        {
            string? entered = GetInput(prompt);
            return ParseAndValidateInt(entered, min, max);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

void HandleStart()
{
    string? enteredName = GetInput("Пожалуйста, введите ваше имя: ");
    ValidateString(enteredName);
    user = new ToDoUser(enteredName);
    Console.WriteLine($"Привет, {user.TelegramUserName}! Чем могу помочь?");
    Console.WriteLine();
}

void HandleHelp()
{
    if (user != null && !string.IsNullOrWhiteSpace(user.TelegramUserName))
        Console.Write($"{user.TelegramUserName}, ");
    Console.WriteLine("Здравствуйте! Вот ваша инструкция: \n");
    OutputHelp();
}

void HandleInfo()
{
    OutputInfo();
    if (user != null && !string.IsNullOrWhiteSpace(user.TelegramUserName))
        Console.WriteLine($"{user.TelegramUserName}, спасибо, что вы с нами!\n");
}

void HandleAddTask()
{
    if (tasks.Count >= taskCountLimit)
        throw new TaskCountLimitException(taskCountLimit);

    string newTask = GetInput("Введите описание задачи: ") ?? string.Empty;
    ValidateString(newTask);

    string normalized = newTask.Trim();

    if (normalized.Length > taskLengthLimit)
        throw new TaskLengthLimitException(normalized.Length, taskLengthLimit);

    if (tasks.Any(x => x.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase)))
        throw new DuplicateTaskException(normalized);

    if (user == null)
        throw new ArgumentException("Сначала введите имя пользователя, выполнив команду /start!\n");

    tasks.Add(new ToDoItem(user, normalized));
    Console.WriteLine($"Задача \"{normalized}\" добавлена.\n");
}

void HandleShowTasks()
{
    if (!HasTasks())
    {
        Console.WriteLine("В вашем списке нет задач!\n");
        return;
    }
    ShowTasks();
}

void HandleShowAllTasks()
{
    if (!HasTasks())
    {
        Console.WriteLine("В вашем списке нет задач!\n");
        return;
    }
    ShowTasks(false);
}

void HandleRemoveTask()
{
    if (!HasTasks())
    {
        Console.WriteLine("В вашем списке нет задач!\n");
        return;
    }

    Console.WriteLine("Вот ваши задачи: ");
    ShowTasks();

    string numberString = GetInput("Введите номер задачи для удаления: ") ?? string.Empty;
    int indexBased = ParseAndValidateInt(numberString, 1, tasks.Count);
    RemoveTask(indexBased - 1);
}

void HandleCompleteTask(string id)
{
    var completeTask = tasks.FirstOrDefault(x => x.Id.ToString() == id);
    if(completeTask == null)
        throw new ArgumentException($"Не существует задачи с Id {id}");

    completeTask.Complete();
    Console.WriteLine($"Задача {completeTask.Name} выполнена");
}

void HandleExit()
{
    isWorking = false;
    if (user != null && !string.IsNullOrWhiteSpace(user.TelegramUserName))
        Console.Write($"{user.TelegramUserName}, до скорой встречи!");
}

void HandleEcho()
{
    if (user == null || string.IsNullOrWhiteSpace(user.TelegramUserName))
    {
        Console.WriteLine("Команда не доступна. Для доступа введите ваше имя с помощью команды /start");
        return;
    }

    Console.WriteLine(input.Replace("/echo", "").TrimStart());
}