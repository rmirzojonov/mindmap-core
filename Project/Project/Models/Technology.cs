using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Technology
    /// Поля:   int SId, string Name
    /// </summary>
    public class Technology
    {
        [Key]
        public int SId { get; set; }
        public string Name { get; set; }

        public List<UserTechnology> UserTechnologies { get; set; }
        public List<TechnologyCharacteristic> TechnologyCharacteristics { get; set; }
        public List<Curator> Curators { get; set; }
        public List<DataLink> DataLinks { get; set; }
        public List<TestLink> TestLinks { get; set; }
        public List<Grade> Grades { get; set; }
    }
}
