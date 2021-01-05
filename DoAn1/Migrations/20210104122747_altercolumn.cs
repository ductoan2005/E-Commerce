using Microsoft.EntityFrameworkCore.Migrations;

namespace DoAn1.Migrations
{
    public partial class altercolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "255586ff-bd32-479d-ab17-2220244c809b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3d17f7e-bf0a-4d6b-9e97-4c5ac8290086");

            migrationBuilder.AddColumn<double>(
                name: "TongTien",
                table: "HoaDon",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "dfb46570-c773-4d38-9fd4-6a2b1059665f", "44fbcd8d-76c6-4b66-87a1-4068a139ca38", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b044ea0b-0a22-4150-86f3-b7222ae15576", "2d813126-27f8-4199-ba71-c7a076455ed1", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b044ea0b-0a22-4150-86f3-b7222ae15576");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dfb46570-c773-4d38-9fd4-6a2b1059665f");

            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "HoaDon");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f3d17f7e-bf0a-4d6b-9e97-4c5ac8290086", "9fe9205b-7941-4eb4-aa5f-a8a7820d45f1", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "255586ff-bd32-479d-ab17-2220244c809b", "88cc2aba-228d-4554-94f0-cd1863c36f2d", "Admin", "ADMIN" });
        }
    }
}
