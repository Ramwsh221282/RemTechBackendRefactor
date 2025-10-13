using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.VehicleContext.ValueObjects;

#nullable disable

namespace Vehicles.Infrastructure.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "vehicles_module");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "vehicles_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    rating = table.Column<long>(type: "bigint", nullable: false),
                    vehicles_count = table.Column<long>(type: "bigint", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "vehicles_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<long>(type: "bigint", nullable: false),
                    vehicles_count = table.Column<long>(type: "bigint", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                schema: "vehicles_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<LocationAddress>(type: "jsonb", nullable: false),
                    rating = table.Column<long>(type: "bigint", nullable: false),
                    vehicles_count = table.Column<long>(type: "bigint", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "models",
                schema: "vehicles_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<long>(type: "bigint", nullable: false),
                    vehicles_count = table.Column<long>(type: "bigint", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_models", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                schema: "vehicles_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    characteristics = table.Column<VehicleCharacteristicsCollection>(type: "jsonb", nullable: false),
                    photos = table.Column<VehiclePhotosCollection>(type: "jsonb", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true),
                    is_nds = table.Column<bool>(type: "boolean", nullable: false),
                    price_value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.id);
                    table.ForeignKey(
                        name: "FK_vehicles_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "vehicles_module",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehicles_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "vehicles_module",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehicles_locations_location_id",
                        column: x => x.location_id,
                        principalSchema: "vehicles_module",
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehicles_models_model_id",
                        column: x => x.model_id,
                        principalSchema: "vehicles_module",
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "brands_unique_name_idx",
                schema: "vehicles_module",
                table: "brands",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_hnsw_brands",
                schema: "vehicles_module",
                table: "brands",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "category_unique_name_idx",
                schema: "vehicles_module",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_hnsw_categories",
                schema: "vehicles_module",
                table: "categories",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_hnsw_locations",
                schema: "vehicles_module",
                table: "locations",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_hnsw_models",
                schema: "vehicles_module",
                table: "models",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "unique_model_name_idx",
                schema: "vehicles_module",
                table: "models",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_hnsw_vehicles",
                schema: "vehicles_module",
                table: "vehicles",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_brand_id",
                schema: "vehicles_module",
                table: "vehicles",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_category_id",
                schema: "vehicles_module",
                table: "vehicles",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_id_category_id_location_id_brand_id_model_id",
                schema: "vehicles_module",
                table: "vehicles",
                columns: new[] { "id", "category_id", "location_id", "brand_id", "model_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_location_id",
                schema: "vehicles_module",
                table: "vehicles",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_model_id",
                schema: "vehicles_module",
                table: "vehicles",
                column: "model_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicles",
                schema: "vehicles_module");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "vehicles_module");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "vehicles_module");

            migrationBuilder.DropTable(
                name: "locations",
                schema: "vehicles_module");

            migrationBuilder.DropTable(
                name: "models",
                schema: "vehicles_module");
        }
    }
}
