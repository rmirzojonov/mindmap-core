using Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.ViewModels
{
    public class TreeModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        
        public TreeType TreeType { get; set; }
    }
}
