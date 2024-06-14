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
                    Name = table.Column<string>(type: "text", nullable: false),
                    ChiefId = table.Column<int>(type: "integer", nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Tree = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Employee_ChiefId",
                table: "Department",
                column: "ChiefId",
                principalTable: "Employee",
                principalColumn: "Id");

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
            migrationBuilder.Sql("create or replace function public.\"trg_fn_department_before_ins_upd\" ()" +
                "                   returns trigger as " +
                "                   $$" +
                "                   begin " +
                "                       if not new.\"ParentId\" is null then " +
                "                           new.\"Tree\" = (select prn.\"Tree\" from public.\"Department\" prn where prn.\"Id\" = new.\"ParentId\" limit 1) || '.' || new.\"Name\"; " +
                "                       else" +
                "                           new.\"Tree\" = new.\"Name\";" +
                "                       end if; " +
                "                       update public.\"Employee\" " +
                "                       set \"Tree\" = new.\"Tree\" " +
                "                       where \"DepartmentId\" = new.\"Id\"; " +
                "                       return new; " +
                "                   end;" +
                "                   $$ language plpgsql;", true);
            migrationBuilder.Sql("create or replace function public.\"trg_fn_employee_before_ins_upd\" ()" +
                "                   returns trigger as" +
                "                   $$" +
                "                   begin" +
                "                   new.\"Tree\" = (select dep.\"Tree\" from public.\"Department\" dep where dep.\"Id\" = new.\"DepartmentId\" limit 1);" +
                "                   return new;" +
                "                   end;" +
                "                   $$ language plpgsql;", true);

            migrationBuilder.Sql("  create trigger \"trg_fn_department_before_ins_upd\"" +
                "                   before insert or update" +
                "                   on public.\"Department\"" +
                "                   for each row execute function public.\"trg_fn_department_before_ins_upd\"();", true);
            migrationBuilder.Sql("  create trigger \"trg_fn_employee_before_ins_upd\"" +
                "                   before insert or update" +
                "                   on public.\"Employee\"" +
                "                   for each row execute function public.\"trg_fn_employee_before_ins_upd\"();", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
