using System.Data;
using System.Reflection;
using System.Text.Json;
using Dapper;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.ActivationStatus;
using Identity.Core.SubjectsModule.Domain.Credentials;
using Identity.Core.SubjectsModule.Domain.Metadata;
using Identity.Core.SubjectsModule.Domain.Permissions;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Primitives.Extensions;

namespace Identity.Persistence.NpgSql.SubjectsModule;

public static class NpgSqlIdentitySubjects
{
    private static readonly Assembly Assembly = typeof(NpgSqlIdentitySubjects).Assembly;
    
    public static Insert InsertSubject(NpgSqlSession session) => async (subject, ct) =>
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

    public static Delete DeleteSubject(NpgSqlSession session) => async (subject, ct) =>
        await Unit.UnitOf(session).Executed(async s =>
            await s.Execute(subject.ToQuery(
                """
                DELETE FROM identity_module.subjects WHERE id = @id
                """,
                session, 
                ct
                )));

    public static Update UpdateSubject(NpgSqlSession session) => async (subject, ct) =>
        await Unit.UnitOf(session).Executed(async s =>
            await s.Execute(subject.ToQuery(
                $"""
                UPDATE identity_module.subjects {subject.UpdateClause()}
                WHERE id = @id
                """,
                session, 
                ct
            )));
    
    public static IsEmailUnique IsSubjectEmailUnique(NpgSqlSession session)
    {
        return async (email, ct) =>
        {
            SubjectQueryArgs args = new(Email: email);
            string whereClause = args.WhereClause();
            bool exists = await session.QuerySingleRow<bool>(args.TransformToCommand(
                $"SELECT EXISTS(SELECT 1 FROM identity_module.subjects {whereClause})",
                session,
                ct));
            return !exists;
        };
    }

    public static IsLoginUnique IsSubjectLoginUnique(NpgSqlSession session)
    {
        return async (login, ct) =>
        {
            SubjectQueryArgs args = new(Login: login);
            string whereClause = args.WhereClause();
            bool exists = await session.QuerySingleRow<bool>(args.TransformToCommand(
                $"SELECT EXISTS(SELECT 1 FROM identity_module.subjects {whereClause})",
                session,
                ct));
            return !exists;
        };
    }
    
    public static Find Find(NpgSqlSession session) => async (args, ct) =>
    {
        string whereClause = args.WhereClause();
        if (string.IsNullOrWhiteSpace(whereClause)) return Optional.None<Subject>();
        string lockClause = args.LockClauseWithAlias("s");
            
        string sql =
            $"""
             SELECT
                 s.id as s_id,
                 s.email as s_email,
                 s.login as s_login,
                 s.password as s_password,
                 s.activation_date as s_activation_date,
                 (SELECT COALESCE((SELECT jsonb_build_array(jsonb_build_object(
                     'id', p.id,
                     'name', p.name)) FROM identity_module.permissions p WHERE p.id = sp.permission_id), '[]'::jsonb)) as s_permissions_json_array
             FROM identity_module.subjects s
             LEFT JOIN identity_module.subject_permissions sp ON sp.subject_id = s.id
             {whereClause}
             {lockClause}
             """;
            
        CommandDefinition command = args.TransformToCommand(sql, session, ct);
        TableSubject? subject = await session.QueryMaybeRow<TableSubject>(command);
        return subject.MaybeToIdentitySubject();
    };

    public static InsertPermission InsertPermission(NpgSqlSession session) => async (subject, permission, ct) =>
    {
        DynamicParameters parameters = SubjectPermissionParameters(subject, permission);
        string sql =
            """
            INSERT INTO identity_module.subject_permissions(subject_id, permission_id)
            VALUES (@subject_id, @permission_id)
            """;
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
        return Unit.Value;
    };

    public static DeletePermission DeletePermission(NpgSqlSession session) => async (subject, permission, ct) =>
    {
        DynamicParameters parameters = SubjectPermissionParameters(subject, permission);
        string sql =
            """
            DELETE FROM identity_module.subject_permissions 
            WHERE subject_id = @subject_id AND permission_id = @permission_id     
            """;
        CommandDefinition command = new(sql, parameters, cancellationToken: ct, transaction: session.Transaction);
        await session.Execute(command);
        return Unit.Value;
    };
    
    public static FindMany FindMany(NpgSqlSession session) => async (args, ct) =>
    {
        string whereClause = args.WhereClause();
        string lockClause = args.LockClause();
            
        string sql =
            $"""
             SELECT
                 s.id as s_id,
                 s.email as s_email,
                 s.login as s_login,
                 s.password as s_password,
                 s.activation_date as s_activation_date,
                 (SELECT COALESCE((SELECT jsonb_build_array(jsonb_build_object(
                     'id', p.id,
                     'name', p.name)) FROM identity_module.permissions p), '[]'::jsonb)) as s_permissions_json_array
             FROM identity_module.subjects s
             LEFT JOIN identity_module.subject_permissions sp ON sp.subject_id = s.id
             {whereClause}
             {lockClause}
             """;
            
        CommandDefinition command = args.TransformToCommand(sql, session, ct);
        IEnumerable<TableSubject> subjects = await session.QueryMultipleRows<TableSubject>(command);
        return subjects.ToIdentitySubjects();
    };
    
    private static DynamicParameters SubjectPermissionParameters(Subject subject, SubjectPermission permission)
    {
        SubjectSnapshot subjectSnap = subject.Snapshot();
        return NpgSqlParametersStorage.New()
            .With("@subject_id", subjectSnap.Id, dbType: DbType.Guid)
            .With("@permission_id", permission.Id, dbType: DbType.Guid)
            .GetParameters();
    }
    
    extension(TableSubject subject)
    {
        internal Subject ToIdentitySubject()
        {
            SubjectMetadata metadata = SubjectMetadata.Create(subject.SLogin, subject.SId);
            SubjectCredentials credentials = SubjectCredentials.Create(subject.SEmail, subject.SPassword);
            SubjectActivationStatus activation = SubjectActivationStatus.Create(subject.SActivationDate);
            SubjectPermissions permissions = SubjectPermissions.Create(subject.GetPermissionsFromJsonArray());
            return Subject.Create(metadata, credentials, activation, permissions);
        }
        
        internal SubjectPermission[] GetPermissionsFromJsonArray()
        {
            string array = subject.SPermissionsJsonArray;
            using JsonDocument document = JsonDocument.Parse(array);
            int length = document.RootElement.GetArrayLength();
            SubjectPermission[] permissions = new SubjectPermission[length];
            int index = 0;
            
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                SubjectPermission permission = CreateSubjectPermissionFromJson(element);
                permissions[index] = permission;
                index++;
            }
            
            return permissions;
        }
    }

    extension(IEnumerable<TableSubject> subjects)
    {
        internal IEnumerable<Subject> ToIdentitySubjects()
        {
            return subjects.Select(s => s.ToIdentitySubject());
        }
    }
    
    private static SubjectPermission CreateSubjectPermissionFromJson(JsonElement json)
    {
        Guid id = json.GetProperty("id").GetGuid();
        string? name = json.GetProperty("name").GetString();
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException(
                $"{nameof(NpgSqlIdentitySubjects)} Invalid permission name jsonb property in database mapping function.");
        return new SubjectPermission(id, name);
    }
    
    extension(TableSubject? subject)
    {
        internal Optional<Subject> MaybeToIdentitySubject()
        {
            return subject is null ? Optional<Subject>.None() : Optional.Some(subject.ToIdentitySubject());
        }
    }

    extension(Subject subject)
    {
        private CommandDefinition ToQuery(string sql, NpgSqlSession session, CancellationToken ct)
        {
            return new CommandDefinition(sql, subject.TransformToParameters(), cancellationToken: ct, transaction: session.Transaction);
        }
        
        private DynamicParameters TransformToParameters()
        {
            SubjectSnapshot snapshot = subject.Snapshot();
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
        
        private string LockClauseWithAlias(string alias)
        {
            return args.WithLock ? $" FOR UPDATE OF {alias}" : string.Empty;
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

    extension(IServiceCollection services)
    {
        public void AddSubjectsStorage()
        {
            services.AddScopedDelegate<Insert>(Assembly);
            services.AddScopedDelegate<Delete>(Assembly);
            services.AddScopedDelegate<Update>(Assembly);
            services.AddScopedDelegate<IsEmailUnique>(Assembly);
            services.AddScopedDelegate<IsLoginUnique>(Assembly);
            services.AddScopedDelegate<Find>(Assembly);
            services.AddScopedDelegate<FindMany>(Assembly);
            services.AddScopedDelegate<InsertPermission>(Assembly);
            services.AddScopedDelegate<DeletePermission>(Assembly);
            services.AddScoped<SubjectsStorage>();
        }
    }
}