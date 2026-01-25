namespace Notifications.Core.PendingEmails.Contracts;

public sealed class PendingEmailNotificationsSpecification
{
	public bool? SentOnly { get; private set; }
	public bool? NotSentOnly { get; private set; }
	public bool? LockRequired { get; private set; }
	public int? Limit { get; private set; }

	public PendingEmailNotificationsSpecification OfSentOnly()
	{
		if (SentOnly.HasValue)
			return this;
		SentOnly = true;
		return this;
	}

	public PendingEmailNotificationsSpecification OfNotSentOnly()
	{
		if (NotSentOnly.HasValue)
			return this;
		NotSentOnly = true;
		return this;
	}

	public PendingEmailNotificationsSpecification WithLock()
	{
		if (LockRequired.HasValue)
			return this;
		LockRequired = true;
		return this;
	}

	public PendingEmailNotificationsSpecification WithLimit(int limit)
	{
		if (Limit.HasValue)
			return this;
		Limit = limit;
		return this;
	}
}
