using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Controllers;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Project.test
{
    /// <summary>
    /// Класс тестов контроллера узлов
    /// </summary>
    public class NodesControllerTests : IClassFixture<ContextFixture>
    {
        /// <summary>
        /// База данных
        /// </summary>
        ContextFixture fixture;

        /// <summary>
        /// Контструктор класса тестов
        /// </summary>
        /// <param name="fixture"></param>
        public NodesControllerTests(ContextFixture fixture)
        {
            this.fixture = fixture;
        }

        /// <summary>
        /// Тест метода GetNodes
        /// </summary>
        [Fact]
        public void GetNodesNotNull()
        {
            var controller = new NodesController(fixture.Context);
            var result = controller.GetNodes();

            Assert.NotNull(result);
        }

        /// <summary>
        /// Тест метода GetNodes определенного дерева
        /// </summary>
        [Fact]
        public async void GetNodesActionSuccess()
        {
            var controller = new NodesController(fixture.Context);
            var result = await controller.GetNodes(1);
            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Тест успешности метода GetNode, который возвращает единственный узел опеределенного дерева
        /// </summary>
        [Fact]
        public async void GetNodeActionSuccess()
        {
            var controller = new NodesController(fixture.Context);
            var result = await controller.GetNode(1, "node1");
            Assert.IsType<OkObjectResult>(result);
        }

        /// <summary>
        /// Тест успешности метода PutNode, который обновляет узел
        /// </summary>
        [Fact]
        public async void PutNodeActionSuccess()
        {
            var controller = new NodesController(fixture.Context);
            var node = new NodeDTO()
            {
                id = "node3",
                parentid = null,
                treeid = 3,
                topic = "Bingo"
            };
            var result = await controller.PutNode(node.id, node);
            var receivedNode = await fixture.Context.Nodes.SingleOrDefaultAsync(n => n.Id == node.id);

            Assert.NotNull(receivedNode);
            Assert.Equal(node.topic, receivedNode.Topic);
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Тест успешности метода PostNode, который добавляет новый узел в бд
        /// </summary>
        [Fact]
        public async void PostNodeActionSuccess()
        {
            var controller = new NodesController(fixture.Context);
            var node = new NodeDTO()
            {
                id = "node4",
                parentid = null,
                treeid = 4,
                topic = "Node 4"
            };
            var result = await controller.PostNode(node);

            var receivedNode = await fixture.Context.Nodes.SingleOrDefaultAsync(n => n.Id == node.id);
            Assert.NotNull(receivedNode);
            Assert.IsType<OkResult>(result);
        }

        /// <summary>
        /// Тест успешности метода DeleteNode, который удаляет выбранный узел
        /// </summary>
        [Fact]
        public async void DeleteNodeActionSuccess()
        {
            string nodeId = "node2";
            var controller = new NodesController(fixture.Context);
            var result = await controller.DeleteNode(nodeId);

            var receivedNode = await fixture.Context.Nodes.SingleOrDefaultAsync(n => n.Id == nodeId);
            Assert.Null(receivedNode);
            Assert.IsType<OkResult>(result);
        }
    }
}
