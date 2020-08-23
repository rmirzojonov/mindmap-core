using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса GradeName
    /// Поля:  int Id,  String Name
    /// </summary>
    public class GradeName
    {
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
    }
}
