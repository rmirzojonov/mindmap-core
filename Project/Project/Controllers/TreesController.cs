using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.ViewModels;

namespace Project.Controllers
{
    /// <summary>
    /// Контроллер для работы со страницами деревьев
    /// </summary>
    public class TreesController : Controller
    {
        /// <summary>
        /// База данных
        /// </summary>
        private readonly UserContext db;

        /// <summary>
        /// Конструктор для контроллера деревьев 
        /// </summary>
        public TreesController(UserContext context)
        {
            db = context;
        }

        /// <summary>
        /// Возвращает представление страницы дерева
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public IActionResult Tree(Tree tree)
        {
            return View(tree);
        }

        /// <summary>
        /// Возвращает все деревья в представление
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ListOfTrees()
        {
            return View(await db.Trees.Include(t => t.User).ToListAsync());
        }

        /// <summary>
        /// Возвращает представление страницы просмотра дерева
        /// Пример запроса: GET: Trees/Details/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var tree = await db.Trees.SingleOrDefaultAsync(t => t.Id == id);
            if (tree == null)
            {
                return NotFound();
            }
            
            return View(tree);
        }

        /// <summary>
        /// Возвращает представление добавления дерева
        /// Пример запроса: GET: Trees/Create
        /// </summary>
        /// <returns></returns>
        public IActionResult AddTree()
        {
            return View();
        }

        /// <summary>
        /// Принимает новое дерево, сохраняет её в бд
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTree(TreeModel model)
        {
            User user = db.Users.FirstOrDefault(u => u.Email == @User.Identity.Name);
            if (model != null)
            {
                var tree = new Tree { UserId = user.Id, Name = model.Name, TreeType = model.TreeType };
                db.Trees.Add(tree);
                var root = new Node()
                {
                    Id = Guid.NewGuid().ToString(),
                    TreeId = tree.Id,
                    ParentId = null,
                    Topic = tree.Name
                };
                db.Nodes.Add(root);

                await db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ListOfTrees));
        }

        /// <summary>
        /// Возвращает представление страницы редактирования дерева
        /// Пример запроса: GET: Trees/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> TreeEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tree = await db.Trees.SingleOrDefaultAsync(m => m.Id == id);
            if (tree == null)
            {
                return NotFound();
            }
            return View(tree);
        }

        /// <summary>
        /// Принимает данные с редактирования страницы, обновляет бд
        /// </summary>
        /// <param name="id">Идентификатор дерева</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TreeEdit(int id, TreeModel model)
        {
            var tree = await db.Trees.SingleOrDefaultAsync(t => t.Id == id);
            if (model != null)
            {
                try
                {
                    tree.Name = model.Name;
                    tree.TreeType = model.TreeType;
                    db.Update(tree);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreeExists(tree.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListOfTrees));
            }
            return View(tree);
        }

        /// <summary>
        /// Возвращает представление страницы удаления дерева
        /// Пример запроса: GET: Trees/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tree = await db.Trees
                .SingleOrDefaultAsync(m => m.Id == id);
            
                //.Include(t => t.User)
            if (tree == null)
            {
                return NotFound();
            }

            return View(tree);
        }

        /// <summary>
        /// Удаляет дерево с базы данных
        /// Пример запроса: POST: Trees/Delete/5
        /// </summary>
        /// <param name="id">Идентификатор дерева</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tree = await db.Trees.SingleOrDefaultAsync(m => m.Id == id);
            db.Trees.Remove(tree);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(ListOfTrees));
        }

        /// <summary>
        /// Проверяет существует ли дерево с данным идентификатором
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TreeExists(int id)
        {
            return db.Trees.Any(e => e.Id == id);
        }
    }
}
