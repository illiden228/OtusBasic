using System;
using System.Collections.Generic;

bool isWorking = true;
string? input = string.Empty;
string name = string.Empty;
string version = "0.0.2";
string dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");
List<string> tasks = new();

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
                      "/showtasks - посмотреть список задач\n" +
                      "/removetask - удалить задачу");

    if (!string.IsNullOrWhiteSpace(name))
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

void ShowTasks()
{
    for (int i = 0; i < tasks.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {tasks[i]}");
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
    name = enteredName!;
    Console.WriteLine($"Привет, {name}! Чем могу помочь?");
    Console.WriteLine();
}

void HandleHelp()
{
    if (!string.IsNullOrWhiteSpace(name))
        Console.Write($"{name}, ");
    Console.WriteLine("Здравствуйте! Вот ваша инструкция: \n");
    OutputHelp();
}

void HandleInfo()
{
    OutputInfo();
    if (!string.IsNullOrWhiteSpace(name))
        Console.WriteLine($"{name}, спасибо, что вы с нами!\n");
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

    if (tasks.Contains(normalized, StringComparer.OrdinalIgnoreCase))
        throw new DuplicateTaskException(normalized);

    tasks.Add(normalized);
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

void HandleExit()
{
    isWorking = false;
    if (!string.IsNullOrWhiteSpace(name))
        Console.Write($"{name}, до скорой встречи!");
}

void HandleEcho()
{
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Команда не доступна. Для доступа введите ваше имя с помощью команды /start");
        return;
    }

    Console.WriteLine(input!.Replace("/echo", "").TrimStart());
}