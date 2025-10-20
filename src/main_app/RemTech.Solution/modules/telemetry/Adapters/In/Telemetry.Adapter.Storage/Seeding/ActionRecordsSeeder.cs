using Serilog;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;
using Telemetry.Domain.Ports.Storage;

namespace Telemetry.Adapter.Storage.Seeding;

public sealed class ActionRecordsSeeder(IActionRecordsStorage storage, ILogger logger)
    : IActionRecordsSeeder
{
    private const string Context = nameof(ActionRecordsSeeder);
    private readonly Random _random = new();

    public async Task Seed()
    {
        if (await storage.Count() > 0)
        {
            logger.Warning("{Context}. Data already persists.", Context);
            return;
        }

        logger.Information("{Context}. Seeding records...", Context);
        await storage.AddRange(GenerateDomainObjects());
        logger.Information("{Context}. Records seeded.", Context);

        long count = await storage.Count();
        logger.Information("{Context}. Records count: {Count}", Context, count);
    }

    private IEnumerable<ActionRecord> GenerateDomainObjects()
    {
        ActionInvokerId[] invokerIds = new ActionInvokerId[50];
        for (int i = 0; i < 50; i++)
        {
            invokerIds[i] = new ActionInvokerId();
        }

        ActionStatus[] statuses = new[]
        {
            ActionStatus.Success,
            ActionStatus.Failure,
            ActionStatus.Create("Pending").Value,
            ActionStatus.Create("Processing").Value,
            ActionStatus.Create("Cancelled").Value,
            ActionStatus.Create("Timeout").Value,
            ActionStatus.Create("Warning").Value,
        };

        string[] names =
        [
            "Login",
            "Logout",
            "Create User",
            "Delete User",
            "Update Profile",
            "View Report",
            "Upload File",
            "Download File",
            "Send Message",
            "Change Password",
            "Reset Password",
            "View Dashboard",
            "Export Data",
            "Import Data",
            "Delete Account",
            "Enable 2FA",
            "Disable 2FA",
            "Change Email",
            "Change Phone",
            "Request Support",
            "Add Role",
            "Remove Role",
            "Assign Permission",
            "Revoke Permission",
            "Update Settings",
            "Create Project",
            "Delete Project",
            "Update Project",
            "Clone Project",
            "Run Pipeline",
            "Deploy Service",
            "Rollback Service",
            "View Logs",
            "View Metrics",
            "View Alerts",
            "Create Task",
            "Complete Task",
            "Cancel Task",
            "Reassign Task",
            "Update Task",
            "Create Group",
            "Delete Group",
            "Invite User",
            "Remove User",
            "Join Group",
            "Leave Group",
            "Create Post",
            "Update Post",
            "Delete Post",
            "React to Post",
            "Share Resource",
            "Unshare Resource",
            "View Resource",
            "Edit Resource",
            "Lock Resource",
            "Unlock Resource",
            "Archive Resource",
            "Restore Resource",
            "View History",
            "Revert Change",
            "Configure Service",
            "Restart Service",
            "Stop Service",
            "Start Service",
            "Scale Service",
            "Backup Data",
            "Restore Data",
            "Schedule Task",
            "Cancel Scheduled Task",
            "View Task Queue",
            "Submit Form",
            "Approve Request",
            "Reject Request",
            "Forward Request",
            "Escalate Issue",
            "Close Issue",
            "Reopen Issue",
            "Transfer Ownership",
            "Change Ownership",
            "Set Priority",
            "Clear Cache",
            "Update Cache",
            "Flush Logs",
            "Analyze Logs",
            "Export Logs",
            "Generate Report",
            "Schedule Report",
            "Cancel Report",
            "View Report",
            "Save Report",
            "Send Email",
            "Receive Email",
            "Configure SMTP",
            "Configure API",
            "Update API Key",
            "Revoke API Key",
            "Rotate API Key",
            "Test Connection",
            "Check Health",
            "View Metrics",
            "Set Alert",
            "Clear Alert",
            "Acknowledge Alert",
            "Trigger Alert",
            "Silence Alert",
            "Add Tag",
            "Remove Tag",
            "Filter Records",
            "Sort Records",
            "Group Records",
            "Export Records",
            "Import Records",
            "Validate Records",
            "Process Records",
            "Archive Records",
        ];

        for (int i = 0; i < 100; i++)
        {
            ActionInvokerId randomInvokerId = invokerIds[_random.Next(invokerIds.Length)];
            ActionStatus randomStatus = statuses[_random.Next(statuses.Length)];
            ActionName randomName = ActionName.Create(names[_random.Next(names.Length)]).Value;
            ActionId randomId = new ActionId();
            DateTime randomDateTime = DateTime
                .UtcNow.AddDays(-_random.Next(1, 365 * 2)) // за последние 2 года
                .AddHours(_random.Next(0, 24))
                .AddMinutes(_random.Next(0, 60))
                .AddSeconds(_random.Next(0, 60));

            ActionDate randomOccuredAt = ActionDate.Create(randomDateTime).Value;
            List<ActionComment> comments = GenerateRandomComments();

            yield return new ActionRecord
            {
                Id = randomId,
                InvokerId = randomInvokerId,
                Name = randomName,
                Status = randomStatus,
                OccuredAt = randomOccuredAt,
                Comments = comments,
            };
        }
    }

    private List<ActionComment> GenerateRandomComments()
    {
        string[] possibleComments =
        [
            "Комментарий 1",
            "Комментарий 2",
            "Комментарий 3",
            "Успешное выполнение",
            "Ошибка выполнения",
            "Проблема с доступом",
            "Данные не найдены",
            "Повторная попытка",
            "Запрос отклонён",
            "Выполнено автоматически",
            "Выполнено вручную",
            "Ожидание подтверждения",
            "Подтверждение получено",
            "Подтверждение отсутствует",
            "Требуется повтор",
            "Требуется модерация",
            "Операция отменена",
            "Операция отложена",
            "Завершено успешно",
            "Завершено с ошибками",
            "Частичное выполнение",
            "Время ожидания истекло",
            "Превышено ограничение",
            "Превышено количество попыток",
            "Недостаточно прав",
            "Недостаточно ресурсов",
            "Проблема с сетью",
            "Проблема с БД",
            "Проблема с файлом",
            "Файл не найден",
            "Файл повреждён",
            "Файл загружен",
            "Файл обработан",
            "Файл сохранён",
            "Файл удалён",
            "Файл зашифрован",
            "Файл расшифрован",
            "Файл защищён",
            "Файл заблокирован",
            "Файл разблокирован",
            "Файл отправлен",
            "Файл получен",
            "Файл передан",
            "Файл отменён",
            "Файл просрочен",
            "Файл устарел",
            "Файл обновлён",
            "Файл создан",
            "Файл изменён",
            "Файл откат",
            "Файл восстановлен",
            "Файл заархивирован",
            "Файл разархивирован",
            "Файл сжат",
            "Файл распакован",
            "Файл проверен",
            "Файл отсканирован",
            "Файл подписан",
            "Файл подтверждён",
            "Файл отозван",
            "Файл передан",
            "Файл обновлён",
            "Файл опубликован",
            "Файл отозван",
            "Файл отменён",
            "Файл заблокирован",
            "Файл разблокирован",
            "Файл зашифрован",
            "Файл расшифрован",
            "Файл защищён",
            "Файл передан",
            "Файл отменён",
            "Файл просрочен",
            "Файл устарел",
            "Файл обновлён",
            "Файл создан",
            "Файл изменён",
            "Файл откат",
            "Файл восстановлен",
            "Файл заархивирован",
            "Файл разархивирован",
            "Файл сжат",
            "Файл распакован",
            "Файл проверен",
            "Файл отсканирован",
            "Файл подписан",
            "Файл подтверждён",
            "Файл отозван",
        ];

        int count = _random.Next(0, 4); // от 0 до 3 комментариев
        HashSet<string> selected = new HashSet<string>();

        for (int i = 0; i < count; i++)
        {
            string commentText = possibleComments[_random.Next(possibleComments.Length)];
            selected.Add(commentText);
        }

        List<ActionComment> result = ActionComment.Create(selected).Value.ToList();
        return result;
    }
}
