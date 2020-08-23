using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса TestLink
    /// Поля: int NId,  String Link, int Level, int SId
    /// </summary>
    public class TestLink
    {
        [Key]
        public int NId { get; set; }
        public String Link { get; set; }
        public int Level { get; set; }
        public int SId { get; set; }
        public Technology Technology { get; set; }
    }
}
