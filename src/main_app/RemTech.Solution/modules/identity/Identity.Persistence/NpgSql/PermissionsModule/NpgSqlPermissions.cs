using System.Data;
using System.Reflection;
using Dapper;
using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;
using RemTech.Primitives.Extensions;

namespace Identity.Persistence.NpgSql.PermissionsModule;

public static class NpgSqlPermissions
{
    private static readonly Assembly Assembly = typeof(NpgSqlPermissions).Assembly;
    
    public static Insert Insert(NpgSqlSession session) =>
        async (permission, ct) =>
        {
            const string sql =
                """
                INSERT INTO identity_module.permissions(id, name)
                VALUES (@id, @name)
                """;
            CommandDefinition command = permission.CreateCommand(sql, session, ct);
            await session.Execute(command);
            return Unit.Value;
        };

    public static Delete Delete(NpgSqlSession session) =>
        async (permission, ct) =>
        {
            const string sql =
                """
                DELETE FROM identity_module.permissions
                WHERE id = @id
                """;
            CommandDefinition command = permission.CreateCommand(sql, session, ct);
            await session.Execute(command);
            return Unit.Value;
        };

    public static Update Update(NpgSqlSession session) =>
        async (permission, ct) =>
        {
            string updateClause = permission.UpdateClause();
            string sql = 
                $"""
                 UPDATE identity_module.permissions
                 {updateClause}
                 WHERE id = @id
                 """;
            CommandDefinition command = permission.CreateCommand(sql, session, ct);
            await session.Execute(command);
            return Unit.Value;
        };

    public static Find Find(NpgSqlSession session) =>
        async (args, ct) =>
        {
            string whereClause = args.WhereClause();
            
            if (string.IsNullOrWhiteSpace(whereClause)) return Optional.None<Permission>();
            
            string sql =
                $"""
                SELECT * FROM identity_module.permissions
                {whereClause}
                """;
            
            CommandDefinition command = args.CreateCommand(sql, session, ct);
            TablePermission? permission = await session.QueryMaybeRow<TablePermission>(command);
            return permission == null
                ? Optional.None<Permission>()
                : Optional.Some(permission.ToPermission());
        };

    public static FindMany FindMany(NpgSqlSession session) =>
        async (args, ct) =>
        {
            string whereClause = args.WhereClause();
            string sql =
                $"""
                 SELECT * FROM identity_module.permissions
                 {whereClause}
                 """;
            
            CommandDefinition command = args.CreateCommand(sql, session, ct);
            
            IEnumerable<TablePermission> permissions =
                await session.QueryMultipleRows<TablePermission>(command);
            
            return permissions.Map(p => p.ToPermission());
        };

    extension(Permission permission)
    {
        private DynamicParameters FillParameters()
        {
            PermissionSnapshot snapshot = permission.Snapshot();
            return NpgSqlParametersStorage.New()
                .With("@id", snapshot.Id, DbType.Guid)
                .With("@name", snapshot.Name, DbType.String)
                .GetParameters();
        }

        private CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = permission.FillParameters();
            return new CommandDefinition(
                sql, 
                parameters, 
                cancellationToken: ct, 
                transaction: session.Transaction);
        }

        private string UpdateClause()
        {
            string[] clauses = permission.UpdateClauses();
            return "SET " + Strings.Joined(clauses, ", ");
        }
        
        private string[] UpdateClauses()
        {
            string[] clauses =
            [
                "id = @id",
                "name = @name"
            ];
            return clauses;
        }
    }

    extension(PermissionsQueryArgs args)
    {
        private CommandDefinition CreateCommand(string sql, NpgSqlSession session, CancellationToken ct)
        {
            DynamicParameters parameters = args.FillParameters();
            return new CommandDefinition(
                sql, 
                parameters, 
                cancellationToken: ct, 
                transaction: session.Transaction);
        }
        
        private DynamicParameters FillParameters()
        {
            return ScopedParametersStorage<PermissionsQueryArgs>.New(args)
                .WithIfNotNull("@id", a => a.Id, id => id.Value, DbType.Guid)
                .WithIfNotNull("@name", a => a.Name, n => n, DbType.String)
                .GetParameters();
        }
        
        private string WhereClause()
        {
            string[] clauses = args.WhereClauses();
            if (clauses.Length == 0) return string.Empty;
            return "WHERE " + Strings.Joined(clauses, " AND ");
        }
        
        private string[] WhereClauses()
        {
            List<string> clauses = [];
            if (args.Id.HasValue) clauses.Add("id = @id");
            if (!string.IsNullOrWhiteSpace(args.Name)) clauses.Add("name = @name");
            return clauses.ToArray();
        }
    }

    extension(TablePermission permission)
    {
        private Permission ToPermission()
        {
            return Permission.Create(permission.Name, permission.Id);
        }
    }

    extension(IServiceCollection services)
    {
        public void AddPermissionsStorage()
        {
            services.AddScoped<Insert>(sp => Insert(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<Delete>(sp => Delete(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<Update>(sp => Update(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<Find>(sp => Find(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<FindMany>(sp => FindMany(sp.Resolve<NpgSqlSession>()));
            services.AddScoped<PermissionsStorage>();
        }
    }
}