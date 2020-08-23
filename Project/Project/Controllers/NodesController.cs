using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    /// <summary>
    /// API Контроллер для работы с узлами дерева
    /// </summary>
    [Produces("application/json")]
    [Route("api/Nodes")]
    public class NodesController : Controller
    {
        /// <summary>
        /// База данных
        /// </summary>
        private readonly UserContext db;

        /// <summary>
        /// Конструктор для контроллеров узлов 
        /// </summary>
        public NodesController(UserContext context)
        {
            db = context;
        }

        /// <summary>
        /// Возвращает все существующие узлы в json формате
        /// Пример запроса: GET: api/Nodes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Node> GetNodes()
        {
            return db.Nodes;
        }

        /// <summary>
        /// Возвращает все узлы принадлижащие данному дереву в json формате
        /// Пример запроса: GET: api/Nodes/1
        /// </summary>
        /// <returns></returns>
        [HttpGet("{tid}")]
        public async Task<IActionResult> GetNodes([FromRoute] int tid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nodes = await db.Nodes.Where(t => t.TreeId == tid).Select(nd => new NodeDTO()
            {
                id = nd.Id, parentid = nd.ParentId, treeid = nd.TreeId, topic=nd.Topic
            }).ToListAsync();

            if (nodes == null)
            {
                return NotFound();
            }

            return Ok(nodes);
        }

        /// <summary>
        /// Возвращает узел принадлижащий данному дереву в json формате
        /// Пример запроса: GET: api/Nodes/5/1
        /// </summary>
        /// <param name="tid">Идентификатор дерева</param>
        /// <param name="nid">Идентификатор узла</param>
        /// <returns></returns>
        [HttpGet("{tid}/{nid}")]
        public async Task<IActionResult> GetNode([FromRoute] int tid, [FromRoute] string nid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var node = await db.Nodes.SingleOrDefaultAsync(m => m.TreeId == tid && m.Id == nid);

            if (node == null)
            {
                return NotFound();
            }

            return Ok(node);
        }

        /// <summary>
        /// Обновляет узел
        /// Пример запроса: PUT: api/Nodes/5
        /// </summary>
        /// <param name="id">Идентификатор узла</param>
        /// <param name="node"></param>
        /// <returns></returns>\
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNode([FromRoute] string id, [FromBody] NodeDTO node)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != node.id)
            {
                return BadRequest();
            }
            var editedNode = await db.Nodes.SingleOrDefaultAsync(n => n.Id == node.id);
            editedNode.Topic = node.topic;
            db.Entry(editedNode).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Принимает новый узел, сохраняет её в бд
        /// Пример запроса: POST: api/Nodes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostNode([FromBody] NodeDTO node)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newNode = new Node()
            {
                Id = node.id,
                ParentId = node.parentid,
                TreeId = node.treeid,
                Topic = node.topic
            };
            db.Nodes.Add(newNode);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NodeExists(newNode.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        /// <summary>
        /// Удаляет узел с базы данных
        /// Пример запроса: DELETE: api/Nodes/5
        /// </summary>
        /// <param name="id">Идентификатор узла</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var node = await db.Nodes.SingleOrDefaultAsync(m => m.Id == id);
            if (node == null)
            {
                return NotFound();
            }
            await RemoveChildren(node.Id);
            db.Nodes.Remove(node);
            await db.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Каскадное удаление узлов
        /// </summary>
        /// <param name="id">Идентификатор узла</param>
        /// <returns></returns>
        async Task RemoveChildren(string id)
        {
            var children = await db.Nodes.Where(c => c.ParentId == id).ToListAsync();
            foreach (var child in children)
            {
                await RemoveChildren(child.Id);
                db.Nodes.Remove(child);
            }
        }

        /// <summary>
        /// Проверяет существует ли узел с данным идентификатором
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool NodeExists(string id)
        {
            return db.Nodes.Any(e => e.Id == id);
        }
    }
}