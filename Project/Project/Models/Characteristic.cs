using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Characteristic
    /// Поля: int Id, string Name
    /// </summary>
    public class Characteristic
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<TechnologyCharacteristic> TechnologyCharacteristics { get; set; }
    }
}
