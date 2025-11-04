using System.Data;
using Dapper;
using Mailing.Domain.Postmans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.Postmans;

internal sealed class DbPostman(IDbConnection connection, IPostman postman) : PostmanEnvelope(postman)
{
    internal Guid Id => Data.Id;
    internal string Email => Data.Email;
    internal string SmtpPassword => Data.SmtpPassword;

    internal static void Configure(EntityTypeBuilder<DbPostman> builder)
    {
        builder.ToTable("postmans");
        builder.HasKey(x => x.Id).HasName("pk_postmans");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
        builder.Property(x => x.SmtpPassword).HasColumnName("password").IsRequired().HasMaxLength(512);
    }

    public async Task Delete(CancellationToken ct) =>
        await connection.ExecuteAsync(new CommandDefinition(
            """
            DELETE FROM mailing_module.postmans 
            WHERE id = @id
            """,
            new
            {
                id = Id
            },
            cancellationToken: ct));

    public async Task Update(CancellationToken ct) =>
        await connection.ExecuteAsync(new CommandDefinition(
            """
            UPDATE mailing_module.postmans
            SET email = @email,
                smtp_password = @password
                WHERE id = @id
            """,
            new
            {
                email = Email,
                password = SmtpPassword,
                id = Id
            },
            cancellationToken: ct
        ));

    public async Task Save(CancellationToken ct) =>
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO mailing_module.postmans
            (id, email, password)
            VALUES
            (@id, @email, @password)
            """,
            new
            {
                @id = Id,
                @email = Email,
                @password = SmtpPassword
            },
            cancellationToken: ct));

    public async Task<bool> HasUniqueEmail(CancellationToken ct) =>
        !(await connection.QuerySingleAsync<bool>(
            new CommandDefinition(
                """
                SELECT 
                    EXISTS(SELECT 1 FROM mailing_module.postmans 
                WHERE 
                    email = @email);
                """,
                new
                {
                    @email = Email
                },
                cancellationToken: ct)));

    private DbPostman() : this(null!, null!)
    {
        // ef core
    }
}