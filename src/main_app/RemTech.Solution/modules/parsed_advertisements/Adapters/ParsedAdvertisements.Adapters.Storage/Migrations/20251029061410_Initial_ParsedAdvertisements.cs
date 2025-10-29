using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace ParsedAdvertisements.Adapters.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Initial_ParsedAdvertisements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "parsed_advertisements_module");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,")
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characteristics",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characteristics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "models",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_models", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    vehicle_id = table.Column<string>(type: "text", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    is_nds = table.Column<bool>(type: "boolean", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    domain = table.Column<string>(type: "text", nullable: false),
                    location_path = table.Column<string>(type: "ltree", nullable: false),
                    photos = table.Column<string>(type: "jsonb", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.vehicle_id);
                    table.ForeignKey(
                        name: "fk_vehicle_brands",
                        column: x => x.brand_id,
                        principalSchema: "parsed_advertisements_module",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicle_categories",
                        column: x => x.category_id,
                        principalSchema: "parsed_advertisements_module",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicle_locations",
                        column: x => x.location_id,
                        principalSchema: "parsed_advertisements_module",
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicle_models",
                        column: x => x.model_id,
                        principalSchema: "parsed_advertisements_module",
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_characteristics",
                schema: "parsed_advertisements_module",
                columns: table => new
                {
                    vehicle_id = table.Column<string>(type: "text", nullable: false),
                    characteristic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacteristicName = table.Column<string>(type: "text", nullable: false),
                    CharacteristicValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_characteristics", x => new { x.vehicle_id, x.characteristic_id });
                    table.ForeignKey(
                        name: "fk_vehicle_characteristics",
                        column: x => x.vehicle_id,
                        principalSchema: "parsed_advertisements_module",
                        principalTable: "vehicles",
                        principalColumn: "vehicle_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_brands_hnsw",
                schema: "parsed_advertisements_module",
                table: "brands",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_categories_hnsw",
                schema: "parsed_advertisements_module",
                table: "categories",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_characteristics_hnsw",
                schema: "parsed_advertisements_module",
                table: "characteristics",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_models_hnsw",
                schema: "parsed_advertisements_module",
                table: "models",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_regions_hnsw",
                schema: "parsed_advertisements_module",
                table: "regions",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_location_path",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "location_path")
                .Annotation("Npgsql:IndexMethod", "gist");

            migrationBuilder.CreateIndex(
                name: "idx_vehicles_hnsw",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_brand_id",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_category_id",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_location_id",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_model_id",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_price",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_vehicle_id_brand_id_category_id_model_id_location_~",
                schema: "parsed_advertisements_module",
                table: "vehicles",
                columns: new[] { "vehicle_id", "brand_id", "category_id", "model_id", "location_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characteristics",
                schema: "parsed_advertisements_module");

            migrationBuilder.DropTable(
                name: "vehicle_characteristics",
                schema: "parsed_advertisements_module");

            migrationBuilder.DropTable(
                name: "vehicles",
                schema: "parsed_advertisements_module");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "parsed_advertisements_module");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "parsed_advertisements_module");

            migrationBuilder.DropTable(
                name: "regions",
                schema: "parsed_advertisements_module");

            migrationBuilder.DropTable(
                name: "models",
                schema: "parsed_advertisements_module");
        }
    }
}
