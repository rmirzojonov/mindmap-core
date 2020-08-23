using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Authorization
    /// Поля: int Id, string Email, string Password, string Role
    /// </summary>
    public class Authorization
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User User { get; set; }
    }
}
