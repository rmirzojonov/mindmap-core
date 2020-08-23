using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Curator
    /// Поля: int Id, int SId
    /// </summary>
    public class Curator
    {
        public int Id { get; set; }
        public User User { get; set; }

        public int SId { get; set; }
        public Technology Technology { get; set; }

    }
}
