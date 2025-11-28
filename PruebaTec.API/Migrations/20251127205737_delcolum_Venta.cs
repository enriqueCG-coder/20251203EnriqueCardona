using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTec.API.Migrations
{
    /// <inheritdoc />
    public partial class delcolum_Venta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreUsuario",
                table: "Venta");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pwd",
                value: "$2a$11$GXYXICmOBbheKvV8Qe6IzuOFBUhQqGcaVqddIexWNpHNcFfHIhJRm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreUsuario",
                table: "Venta",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pwd",
                value: "$2a$11$hDg1EwXxgI1MfRpxpAGx8eFOMD85Fz8sU1F9Ez6Efy0kO2KkfwfPa");
        }
    }
}
