using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CompShop.Models
{
    public partial class Component
    {
        [NotMapped] // Указывает EF Core игнорировать это свойство при работе с БД
        public string StockStatus { get; set; } = "Загрузка";
    }
}
