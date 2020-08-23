using Microsoft.EntityFrameworkCore;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.test
{
    /// <summary>
    /// Класс для доступа к объекту UserContext
    /// </summary>
    public class ContextFixture : IDisposable
    {
        /// <summary>
        /// Конструктор класса, где база инициализируется и заполняется тестовыми данными
        /// </summary>
        public ContextFixture()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
            var options = InMemoryDbContextOptionsExtensions.UseInMemoryDatabase<UserContext>(optionsBuilder, Guid.NewGuid().ToString()).Options;
            Context = new UserContext(options);

            var node1 = new Node()
            {
                Id = "node1",
                ParentId = null,
                TreeId = 1,
                Topic = "Node 1"
            };
            var node2 = new Node()
            {
                Id = "node2",
                ParentId = null,
                TreeId = 2,
                Topic = "Node 2"
            };
            var node3 = new Node()
            {
                Id = "node3",
                ParentId = null,
                TreeId = 3,
                Topic = "Node 3"
            };
            Context.Nodes.Add(node1);
            Context.Nodes.Add(node2);
            Context.Nodes.Add(node3);

            var tree1 = new Tree()
            {
                Id = 1,
                Name = "Test tree 1",
                TreeType = TreeType.Production,
                UserId = 1
            };
            var tree2 = new Tree()
            {
                Id = 2,
                Name = "Test tree 2",
                TreeType = TreeType.Communications,
                UserId = 1
            };
            var tree3 = new Tree()
            {
                Id = 3,
                Name = "Test tree 3",
                TreeType = TreeType.ProjectBlock,
                UserId = 1
            };
            Context.Trees.Add(tree1);
            Context.Trees.Add(tree2);
            Context.Trees.Add(tree3);

            Context.SaveChanges();
        }

        /// <summary>
        /// Избавляемся от базы данных после завершения тестов
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();
        }

        /// <summary>
        /// База данных
        /// </summary>
        public UserContext Context { get; private set; }
    }
}
