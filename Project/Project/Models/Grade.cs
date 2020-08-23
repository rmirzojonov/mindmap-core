using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Grade
    /// Поля: int NId,  int Id, int Level, int SId
    /// </summary>
    public class Grade
    {
        public int NId { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public int SId { get; set; }
        public Technology Technology { get; set; }
    }
}
