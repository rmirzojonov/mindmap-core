using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса UserTechnology
    /// Поля: int Id, int SId, int Level
    /// </summary>
    public class UserTechnology
    {


        public int Id { get; set; }
        public User User { get; set; }

        public int Level { get; set; }

        public int SId { get; set; }
        public Technology Technology { get; set; }






    }
}
