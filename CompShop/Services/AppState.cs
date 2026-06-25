using CompShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompShop.Services
{
    public static class AppState
    {
        public static Employee? CurrentUser { get; set; }

        public static bool IsAdmin => CurrentUser?.RoleId == 1;   // Администратор
        public static bool IsManager => CurrentUser?.RoleId == 2; // Менеджер
        public static bool IsMaster => CurrentUser?.RoleId == 3;  // Мастер
    }
}
