using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    IdCategory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameCategory = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.IdCategory);
                });

            migrationBuilder.CreateTable(
                name: "OrdersStatus",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersStatus", x => x.IdStatus);
                });

            migrationBuilder.CreateTable(
                name: "PaymnetMethods",
                columns: table => new
                {
                    IdMethod = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymnetMethods", x => x.IdMethod);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRole = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    IdItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    createdat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCategory = table.Column<int>(type: "int", nullable: false),
                    IdCategoryItem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.IdItem);
                    table.ForeignKey(
                        name: "FK_Items_Categories_IdCategoryItem",
                        column: x => x.IdCategoryItem,
                        principalTable: "Categories",
                        principalColumn: "IdCategory",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_Users_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    IdBasket = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdUserBasket = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.IdBasket);
                    table.ForeignKey(
                        name: "FK_Baskets_Users_IdUserBasket",
                        column: x => x.IdUserBasket,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    IdLogin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.IdLogin);
                    table.ForeignKey(
                        name: "FK_Logins_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    IdSession = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.IdSession);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BasketItems",
                columns: table => new
                {
                    IdBasketItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBasket = table.Column<int>(type: "int", nullable: false),
                    IdBasketBasketItems = table.Column<int>(type: "int", nullable: false),
                    IdItem = table.Column<int>(type: "int", nullable: false),
                    IdItemBasketItems = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItems", x => x.IdBasketItem);
                    table.ForeignKey(
                        name: "FK_BasketItems_Baskets_IdBasketBasketItems",
                        column: x => x.IdBasketBasketItems,
                        principalTable: "Baskets",
                        principalColumn: "IdBasket",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItems_Items_IdItemBasketItems",
                        column: x => x.IdItemBasketItems,
                        principalTable: "Items",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    IdOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMethod = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    IdBasket = table.Column<int>(type: "int", nullable: false),
                    IdBasketOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.IdOrder);
                    table.ForeignKey(
                        name: "FK_Orders_Baskets_IdBasketOrder",
                        column: x => x.IdBasketOrder,
                        principalTable: "Baskets",
                        principalColumn: "IdBasket",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_OrdersStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "OrdersStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_PaymnetMethods_IdMethod",
                        column: x => x.IdMethod,
                        principalTable: "PaymnetMethods",
                        principalColumn: "IdMethod",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_IdBasketBasketItems",
                table: "BasketItems",
                column: "IdBasketBasketItems");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_IdItemBasketItems",
                table: "BasketItems",
                column: "IdItemBasketItems");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_IdUser",
                table: "Baskets",
                column: "IdUser",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_IdUserBasket",
                table: "Baskets",
                column: "IdUserBasket");

            migrationBuilder.CreateIndex(
                name: "IX_Items_IdCategoryItem",
                table: "Items",
                column: "IdCategoryItem");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_IdUser",
                table: "Logins",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_Login1",
                table: "Logins",
                column: "Login1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdBasketOrder",
                table: "Orders",
                column: "IdBasketOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdMethod",
                table: "Orders",
                column: "IdMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IdStatus",
                table: "Orders",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IdUser",
                table: "Sessions",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdRole",
                table: "Users",
                column: "IdRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketItems");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "OrdersStatus");

            migrationBuilder.DropTable(
                name: "PaymnetMethods");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
