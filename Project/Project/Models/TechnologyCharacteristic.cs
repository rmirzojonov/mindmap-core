using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса TechnologyCharacteristic
    /// Поля:  int Id, String Description, int SId
    /// </summary>
    public class TechnologyCharacteristic
    {
        public int Id { get; set; }
        public Characteristic Characteristic { get; set; }

        public string Description { get; set; }

        public int SId { get; set; }
        public Technology Technology { get; set; }
    }
}
