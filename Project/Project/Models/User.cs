using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Project.Models
{
    /// <summary>
    /// Модель класса User
    /// Поля: int Id, string Name, string Surname, string Email,  byte[] Photo, string Information
    /// </summary>
    public class User
    {
        [Key]
        [ForeignKey("Authorization")]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; } 
        public string Email { get; set; }
        public byte[] Photo { get; set; }
        public string Information { get; set; }

        public List<UserTechnology> UserTechnologies { get; set; }
        public List<Curator> Curators { get; set; }
        public Authorization Authorization { get; set; }
        public virtual ICollection<Tree> Trees { get; set; }
    }

}