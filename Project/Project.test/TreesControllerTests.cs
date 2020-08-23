using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Controllers;
using Project.Models;
using Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Project.test
{
    /// <summary>
    /// Класс тестов контроллера деревьев
    /// </summary>
    public class TreesControllerTests : IClassFixture<ContextFixture>
    {
        /// <summary>
        /// База данных
        /// </summary>
        ContextFixture fixture;
        
        /// <summary>
        /// Контструктор класса тестов
        /// </summary>
        /// <param name="fixture"></param>
        public TreesControllerTests(ContextFixture fixture)
        {
            this.fixture = fixture;
        }

        /// <summary>
        /// Тест метода AddTree, который возвращает ViewResult для добавления дерева
        /// </summary>
        [Fact]
        public void TreeViewResultNotNull()
        {
            var controller = new TreesController(fixture.Context);
            var result = controller.AddTree();
            
            Assert.IsAssignableFrom<ViewResult>(result);
        }

        /// <summary>
        /// Тест успешности метода TreeEdit, который обновляет информацию о дереве
        /// </summary>
        [Fact]
        public async void TreeEditActionSuccess()
        {
            var controller = new TreesController(fixture.Context);
            var treeModel = new TreeModel()
            {
                Name = "test 1",
                TreeType = TreeType.Production
            };
            var result = await controller.TreeEdit(1, treeModel);
            var receivedResult = await fixture.Context.Trees.SingleOrDefaultAsync(t => t.Id == 1);

            Assert.NotNull(receivedResult);
            Assert.Equal(receivedResult.TreeType, treeModel.TreeType);
            Assert.IsType<RedirectToActionResult>(result);
        }

        /// <summary>
        /// Тест метода AddTree, который возвращает ViewResult для добавления дерева
        /// </summary>
        [Fact]
        public async void ListOfTreesViewResultNotNull()
        {
            var controller = new TreesController(fixture.Context);
            var result = await controller.ListOfTrees();

            Assert.NotNull(result);
        }

        /// <summary>
        /// Тест метода TreeEdit, который возвращает ViewResult для изменения дерева
        /// </summary>
        [Fact]
        public async void TreeEditViewResultFound()
        {
            var controller = new TreesController(fixture.Context);
            var result = await controller.TreeEdit(1);

            Assert.IsNotType<NotFoundResult>(result);
        }

        /// <summary>
        /// Тест успешности метода Delete, который возвращает ViewResult для удаления дерева
        /// </summary>
        [Fact]
        public async void DeleteViewResultSuccess()
        {
            var controller = new TreesController(fixture.Context);
            var result = await controller.Delete(1);

            Assert.IsNotType<NotFoundResult>(result);
        }

        /// <summary>
        /// Тест успешности метода DeleteConfirmed, который удаляет дерево
        /// </summary>
        [Fact]
        public async void DeleteConfirmedResultSuccess()
        {
            var controller = new TreesController(fixture.Context);
            var result = await controller.DeleteConfirmed(2);
            
            var tree = await fixture.Context.Trees.SingleOrDefaultAsync(t => t.Id == 2);

            Assert.Null(tree);
        }
    }
}
