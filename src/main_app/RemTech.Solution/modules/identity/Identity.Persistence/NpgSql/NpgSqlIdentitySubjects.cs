using System.Data;
using Dapper;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Primitives.Extensions;

namespace Identity.Persistence.NpgSql;

internal sealed record TableSubject(
    Guid SId,
    string SLogin,
    string SEmail,
    string SPassword,
    string permissions,
    bool SActivationDate);

internal sealed record TableSubjectPermission(Guid PId, string PName);

public sealed record NpgSqlIdentitySubjectCommands(
    NpgSqlSession Session,
    InsertSubject Insert,
    DeleteSubject Delete,
    UpdateSubject Update,
    IsSubjectEmailUnique IsEmailUnique,
    IsSubjectLoginUnique IsLoginUnique) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Session.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Session.DisposeAsync();
    }
}

public static class NpgSqlIdentitySubjects
{
    public static InsertSubject InsertSubject(NpgSqlSession session) => async (subject, ct) =>
    {
        string sql = 
            """
            INSERT INTO identity_module.subjects
            (id, email, login, password, activation_date)
            VALUES
            (@id, @email, @login, @password, @activation_date)
            """;
        CommandDefinition command = subject.ToQuery(sql, session, ct);
        await session.Execute(command);
        return Unit.Value;
    };

    public static DeleteSubject DeleteSubject(NpgSqlSession session) => async (subject, ct) =>
        await Unit.UnitOf(session).Executed(async s =>
            await s.Execute(subject.ToQuery(
                """
                DELETE FROM identity_module.subjects WHERE id = @id
                """,
                session, 
                ct
                )));

    public static UpdateSubject UpdateSubject(NpgSqlSession session) => async (subject, ct) =>
        await Unit.UnitOf(session).Executed(async s =>
            await s.Execute(subject.ToQuery(
                $"""
                UPDATE identity_module.subjects {subject.UpdateClause()}
                WHERE id = @id
                """,
                session, 
                ct
            )));
    
    public static IsSubjectEmailUnique IsSubjectEmailUnique(NpgSqlSession session)
    {
        return (email, ct) =>
        {
            SubjectQueryArgs args = new(Email: email);
            string whereClause = args.WhereClause();
            return session.QuerySingleRow<bool>(args.TransformToCommand(
                $"SELECT EXISTS(SELECT 1 FROM identity_module.subjects {whereClause})",
                session, 
                ct)
            );
        };
    }

    public static IsSubjectLoginUnique IsSubjectLoginUnique(NpgSqlSession session)
    {
        return (login, ct) =>
        {
            SubjectQueryArgs args = new(Login: login);
            string whereClause = args.WhereClause();
            return session.QuerySingleRow<bool>(args.TransformToCommand(
                    $"SELECT EXISTS(SELECT 1 FROM identity_module.subjects {whereClause})",
                    session, 
                    ct)
                );
        };
    }
    
    
    // TODO COMPLETE FIND QUERY
//     public static FindSubject FindSubject(NpgSqlSession session)
//     {
//         return (args, ct) =>
//         {
//             string whereClause = args.WhereClause();
//             string lockClause = args.LockClause();
//             string sql =
//                 $"""
//                 SELECT
//                 s.id as s_id,
//                 s.email as s_login,
//                 s.login as s_email,
//                 s.password as s_password,
//                 s.activated as s_activated
//                 FROM identity_module.subjects s
//                 LEFT JOIN identity_module.subject_permissions p ON p
//                 {whereClause}
//                 {lockClause}
//                 """;
//             
//         };
//     }
//
//     extension(TableSubject subject)
//     {
//         internal IdentitySubject ToIdentitySubject()
//         {
//             
//         }
//     }
//     
//     extension(TableSubject? subject)
//     {
//         internal Optional<IdentitySubject> ToIdentitySubject()
//         {
//             
//         }
//     }

    extension(Subject subject)
    {
        private CommandDefinition ToQuery(string sql, NpgSqlSession session, CancellationToken ct)
        {
            return new CommandDefinition(sql, subject.TransformToParameters(), cancellationToken: ct, transaction: session.Transaction);
        }
        
        private DynamicParameters TransformToParameters()
        {
            IdentitySubjectSnapshot snapshot = subject.Snapshotted();
            return NpgSqlParametersStorage.New()
                .With("@id", snapshot.Id, DbType.Guid)
                .With("@login", snapshot.Login, DbType.String)
                .With("@email", snapshot.Email, DbType.String)
                .With("@password", snapshot.Password, DbType.String)
                .WithValueOrNull("@activation_date", snapshot.ActivationDate, d => d.HasValue, d => d!.Value, DbType.DateTime)
                .GetParameters();
        }

        private string UpdateClause()
        {
            List<string> clauses =
            [
                "id = @id",
                "login = @login",
                "email = @email",
                "password = @password",
                "activation_date = @activation_date"
            ];
            return "SET " + clauses.ToArray().Join(", ");
        }
    }

    extension(SubjectQueryArgs args)
    {
        private string LockClause()
        {
            return args.WithLock ? " FOR UPDATE " : string.Empty;
        }

        private string WhereClause()
        {
            string[] clauses = args.QueryClauses();
            return clauses.Length == 0 ? string.Empty : "WHERE " + clauses.Join(" AND ");
        }

        private string[] QueryClauses()
        {
            List<string> clauses = [];
            if (Guids.NotEmpty(args.Id)) clauses.Add("@id = id");
            if (Strings.NotEmptyOrWhiteSpace(args.Email)) clauses.Add("@email = @email");
            if (Strings.NotEmptyOrWhiteSpace(args.Login)) clauses.Add("@login = @login");
            return clauses.ToArray();
        }

        private DynamicParameters TransformToParameters()
        {
            return NpgSqlParametersStorage.New()
                .With("@id", args.Id, Guids.NotEmpty, id => id!.Value, DbType.Guid)
                .With("@login", args.Login, Strings.NotEmptyOrWhiteSpace, s => s, DbType.String)
                .With("@email", args.Email, Strings.NotEmptyOrWhiteSpace, s => s, DbType.String)
                .GetParameters();
        }

        private CommandDefinition TransformToCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            return new CommandDefinition(
                sql, 
                args.TransformToParameters(), 
                cancellationToken: ct,
                transaction: session.Transaction);
        }
    }
}