namespace Identity.Application.Features.SubjectRegistration;

public sealed record RegisterSubjectArgs(string Email, string Password, string Login, CancellationToken Ct);