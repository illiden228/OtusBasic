using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.TelegramBot.Commands;

namespace OtusBasic1.Project.TelegramBot.Services;

public class CommandConversation
{
    public ToDoUser? User { get; private set; }
    public IConversationCommand Command { get; private set; }
    public ConversationContext Context { get; private set; }

    public CommandConversation(ToDoUser? user, IConversationCommand command, ConversationContext context)
    {
        User = user;
        Command = command;
        Context = context;
    }
}

public class ConversationContext
{
}