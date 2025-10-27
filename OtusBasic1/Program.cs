bool isWorking = true;
string? input = string.Empty;
string name = string.Empty;
string version = "0.0.2";
string dateOfCreation = DateTime.Today.ToString("dd.MM.yyyy");
List<string> tasks = new();

Console.WriteLine("Добро пожаловать!");
OutputHelp();
while (isWorking)
{
    input = GetInput("Введите команду: \n");    
    switch (input)
    {
        case "/start":
            name = GetInput("Пожалуйста, введите ваше имя: ");
            Console.WriteLine($"Привет, {name}! Чем могу помочь?");
            Console.WriteLine();
            break;
        case "/help":
            if(!string.IsNullOrWhiteSpace(name))
                Console.Write($"{name}, ");
            Console.WriteLine("Здравствуйте! Вот ваша инструкция: \n");
            OutputHelp();
            break;
        case "/info":
            OutputInfo();
            if(!string.IsNullOrWhiteSpace(name))
                Console.WriteLine($"{name}, спасибо, что вы с нами!\n");
            break;
        case "/addtask":
            string newTask = GetInput("Введите описание задачи: ");
            if (!string.IsNullOrWhiteSpace(newTask))
            {
                tasks.Add(newTask);
                Console.WriteLine($"Задача \"{newTask}\" добавлена.\n");
            }
            break;
        case "/showtasks":
            if (!HasTasks())
            {
                Console.WriteLine($"В вашем списке нет задач!\n");
                break;
            }
            ShowTasks();
            break;
        case "/removetask":
            if (!HasTasks())
            {
                Console.WriteLine($"В вашем списке нет задач!\n");
                break;
            }
            Console.WriteLine($"Вот ваши задачи: ");
            ShowTasks();
            string number = GetInput("Введите номер задачи для удаления: ");
            if (!int.TryParse(number, out int result))
            {
                Console.WriteLine("Вы ввели невалидный номер!");
                break;
            }

            RemoveTask(result - 1);
            break;
        case "/exit":
            isWorking = false;
            if(!string.IsNullOrWhiteSpace(name))
                Console.Write($"{name}, до скорой встречи!");
            break;
    }

    if (!string.IsNullOrWhiteSpace(input) && input.Contains("/echo"))
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Команда не доступна. Для доступа введите ваше имя с помощью команды /start");
            continue;
        }
        
        Console.WriteLine(input.Replace("/echo ", ""));
    }
}


string? GetInput(string output)
{
    if(!string.IsNullOrWhiteSpace(output))
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
    
    if(!string.IsNullOrWhiteSpace(name))
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