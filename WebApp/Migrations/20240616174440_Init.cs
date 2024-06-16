using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ChiefId = table.Column<int>(type: "integer", nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Tree = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Department_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ChiefId = table.Column<int>(type: "integer", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    Tree = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_ChiefId",
                        column: x => x.ChiefId,
                        principalTable: "Employee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Department_ChiefId",
                table: "Department",
                column: "ChiefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_ParentId",
                table: "Department",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ChiefId",
                table: "Employee",
                column: "ChiefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentId",
                table: "Employee",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Employee_ChiefId",
                table: "Department",
                column: "ChiefId",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.Sql("create or replace function public.\"trg_fn_department_before_ins_upd\" ()\n" +
                "                   returns trigger as \n" +
                "                   $$\n" +
                "                   begin \n" +
                "                       if lower(TG_OP) = 'insert' or (new.\"ParentId\" != old.\"ParentId\") or (old.\"Name\" != new.\"Name\") then\n" +
                "                           if not new.\"ParentId\" is null then \n" +
                "                               new.\"Tree\" = (select prn.\"Tree\" from public.\"Department\" prn where prn.\"Id\" = new.\"ParentId\" limit 1) || '.' || new.\"Name\"; \n" +
                "                           else\n" +
                "                               new.\"Tree\" = new.\"Name\";\n" +
                "                           end if; \n" +
                "                       elsif new.\"Tree\" is null then\n" +
                "                           new.\"Tree\" = old.\"Tree\";\n" +
                "                       end if;\n" +
                "                                                  \n" +
                "                       update public.\"Employee\" \n" +
                "                       set \"Tree\" = new.\"Tree\" \n" +
                "                       where \"DepartmentId\" = new.\"Id\"; \n" +
                "                       \n" +
                "                       if lower(TG_OP) = 'update' and ((old.\"ParentId\" != new.\"ParentId\") or (old.\"Name\" != new.\"Name\")) then\n" +
                "                           update public.\"Department\" dep\n" +
                "                           set \"Tree\" = replace(dep.\"Tree\", old.\"Tree\", new.\"Tree\")\n" +
                "                           where dep.\"Tree\" like (old.\"Tree\" || '.%');\n" +
                "                       end if;\n" +
                "                                                                               \n" +
                "                       return new; \n" +
                "                   end;\n" +
                "                   $$ language plpgsql;", true);
            migrationBuilder.Sql("create or replace function public.\"trg_fn_employee_before_ins_upd\" ()\n" +
                "                   returns trigger as\n" +
                "                   $$\n" +
                "                   begin\n" +
                "                   new.\"Tree\" = (select dep.\"Tree\" from public.\"Department\" dep where dep.\"Id\" = new.\"DepartmentId\" limit 1);\n" +
                "                   return new;\n" +
                "                   end;\n" +
                "                   $$ language plpgsql;", true);

            migrationBuilder.Sql("  create trigger \"trg_fn_department_before_ins_upd\"\n" +
                "                   before insert or update\n" +
                "                   on public.\"Department\"\n" +
                "                   for each row execute function public.\"trg_fn_department_before_ins_upd\"();", true);
            migrationBuilder.Sql("  create trigger \"trg_fn_employee_before_ins_upd\"\n" +
                "                   before insert or update\n" +
                "                   on public.\"Employee\"\n" +
                "                   for each row execute function public.\"trg_fn_employee_before_ins_upd\"();", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Department_Employee_ChiefId",
                table: "Department");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
