using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Tree
    /// Поля: int Id, int UserId, string Name, TreeType TreeType
    /// </summary>
    public class Tree
    {
        /// <summary>
        /// Идентификатор дерева
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор владельца дерева
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Название дерева
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип дерева
        /// </summary>
        [EnumDataType(typeof(TreeType))]
        public TreeType TreeType { get; set; }

        /// <summary>
        /// Объект владелеца дерева
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Список узлов принадлижащих этому дереву
        /// </summary>
        public virtual ICollection<Node> Nodes { get; set; }
    }

    /// <summary>
    /// Модель enum TreeType
    /// Виды: Production, ProjectBlock, Communications
    /// </summary>
    public enum TreeType
    {
        [Display(Name = "Производство")]
        Production,
        [Display(Name = "Проектный блок")]
        ProjectBlock,
        [Display(Name = "Коммуникации")]
        Communications
    }
}
