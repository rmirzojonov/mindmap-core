using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    /// <summary>
    /// Модель класса Node
    /// Поля: int Id, int TreeId, int ParentId, string Topic
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Идентификатор узла
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор дерева которому принадлежит этот узел
        /// </summary>
        public int TreeId { get; set; }

        /// <summary>
        /// Идентификатор родительского узла
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Текст узла
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Дерево которому принадлежит этот узел
        /// </summary>
        public virtual Tree Tree { get; set; }

        /// <summary>
        /// Родительский узел
        /// </summary>
        public virtual Node ParentNode { get; set; }

        /// <summary>
        /// Список дочерних узлов этого узла
        /// </summary>
        public virtual ICollection<Node> ChildNodes { get; set; }
    }

    /// <summary>
    /// Модель класса Node для работы с api контроллером
    /// Поля: int id, int treeid, int parentid, string topic, isroot
    /// Note: Не нужно добавляеть в бд
    /// </summary>
    [NotMapped]
    public class NodeDTO
    {
        public NodeDTO()
        {
            id = null;
            parentid = null;
            treeid = -1;
        }

        /// <summary>
        /// Идентификатор узла
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Идентификатор дерева которому принадлежит этот узел
        /// </summary>
        public int treeid { get; set; }

        /// <summary>
        /// Идентификатор родительского узла
        /// </summary>
        public string parentid { get; set; }

        /// <summary>
        /// Текст узла
        /// </summary>
        public string topic { get; set; }

        /// <summary>
        /// Является ли данный узел корнем дерева
        /// </summary>
        public bool isroot { get { return string.IsNullOrEmpty(parentid); } }
    }

}
