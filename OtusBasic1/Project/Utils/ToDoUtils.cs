using System.Text;
using OtusBasic1.Project.Core.Entities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace OtusBasic1.Project.Utils;

public class ToDoUtils
{
    public static string LineTasks(IReadOnlyList<ToDoItem> tasks, bool showState = true)
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

            stringBuilder.Append($"{task.Name} - {task.CreatedAt} - `{task.Id}`\n");
        }

        return stringBuilder.ToString();
    }

    public static ReplyKeyboardMarkup ConvertListToReplyKeyboard(List<string> list, int column)
    {
        int index = 0;
        int rows = list.Count / column;
        if (list.Count % column > 0)
            rows++;
        var commandsArray = new KeyboardButton[rows][];
        for (var i = 0; i < commandsArray.Length; i++)
        {
            commandsArray[i] = new KeyboardButton[column];
            for (int j = 0; j < column; j++)
            {
                commandsArray[i][j] = new KeyboardButton(list[index]);
                index++;
                if(index >= list.Count)
                    break;
            }
            
            if(index >= list.Count)
                break;
        }
        
        return new ReplyKeyboardMarkup(commandsArray)
        {
            ResizeKeyboard = true
        };
    }
    
    public static InlineKeyboardMarkup ConvertListToInlineKeyboard(List<(string, string)> list, int column)
    {
        int index = 0;
        int rows = list.Count / column;
        if (list.Count % column > 0)
            rows++;
        var commandsArray = new InlineKeyboardButton[rows][];
        for (var i = 0; i < commandsArray.Length; i++)
        {
            commandsArray[i] = new InlineKeyboardButton[column];
            for (int j = 0; j < column; j++)
            {
                commandsArray[i][j] = InlineKeyboardButton.WithCallbackData(list[index].Item1, list[index].Item2);
                index++;
                if(index >= list.Count)
                    break;
            }
            
            if(index >= list.Count)
                break;
        }
        
        return new InlineKeyboardMarkup(commandsArray);
    }

    public static string GetInput(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return update.Message.Text;
            case UpdateType.CallbackQuery:
                return update.CallbackQuery.Data;
        }
        
        return string.Empty;
    }
    
    public static long GetUserFrom(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return update.Message.From.Id;
            case UpdateType.CallbackQuery:
                return update.CallbackQuery.From.Id;
        }
        
        return 0;
    }
    
    public static ChatId? GetChatId(Update update)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                return update.Message.Chat.Id;
            case UpdateType.CallbackQuery:
                return update.CallbackQuery.Message!.Chat.Id;
        }
        
        return null;
    }
}