using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Telemetry.Core.ActionRecords.ValueObjects;

namespace Telemetry.Core.ActionRecords;

/// <summary>
/// Конструктор записей действий пользователя.
/// </summary>
public static class ActionRecordsConstruction
{
	extension(ActionRecord)
	{
		public static ActionRecord CreateNew(
			ActionRecordInvokerId? invokerId,
			ActionRecordName name,
			ActionRecordPayloadJson? payload,
			ActionRecordSeverity severity,
			ActionRecordError? error
		)
		{
			return new()
			{
				Error = error,
				InvokerId = invokerId,
				Name = name,
				PayloadJson = payload,
				Severity = severity,
				Id = ActionRecordId.New(),
				OccuredDateTime = ActionRecordOccuredDateTime.Now(),
			};
		}

		public static ActionRecord CreateNew(
			Optional<Guid> invokerId,
			ActionRecordName name,
			ActionRecordPayloadJson? payload,
			ActionRecordSeverity severity,
			ActionRecordError? error
		)
		{
			return new()
			{
				Error = error,
				InvokerId = invokerId.HasValue ? ActionRecordInvokerId.Create(invokerId.Value).Value : null,
				Name = name,
				PayloadJson = payload,
				Severity = severity,
				Id = ActionRecordId.New(),
				OccuredDateTime = ActionRecordOccuredDateTime.Now(),
			};
		}
	}
}
