using OtusBasic1.Project.Core.Entities;
using OtusBasic1.Project.Core.Services;
using OtusBasic1.Project.TelegramBot.Services;
using OtusBasic1.Project.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace OtusBasic1.Project.TelegramBot.Commands;

public class ReportCommand : BaseCommand
{
    private readonly IToDoReportService _reportService;

    public ReportCommand(ICommandService commandService, IToDoReportService reportService) : base(commandService)
    {
        _reportService = reportService;
    }
    
    public override async Task Execute(ToDoUser? user, ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken = default)
    {
        var report = await _reportService.GetUserStats(user.UserId, cancellationToken);
        var markup = await GetCurrentTasksKeyboard(user, update, cancellationToken);
        await botClient.SendMessage(update.Message.Chat, 
            $"Статистика по задачам на {report.generatedAt}. Всего: {report.total}; Звершенных: {report.completed}; Активных: {report.active}", 
            replyMarkup: markup,
            cancellationToken: cancellationToken);
    }
}