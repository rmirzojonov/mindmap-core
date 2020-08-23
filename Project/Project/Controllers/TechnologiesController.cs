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

namespace Project.Controllers
{
    /// <summary>
    /// Контроллер для работы со страницами технологий
    /// </summary>
    public class TechnologiesController : Controller
    {
        /// <summary>
        /// База данных
        /// </summary>
        private UserContext db;


        /// <summary>
        /// Конструктор контроллера технологий
        /// </summary>
        /// <param name="context"></param>
        public TechnologiesController(UserContext context)
        {
            db = context;
        }

        /// <summary>
        /// Возвращает все технологии в представление
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ListOfTechnologies()
        {
            return View(await db.Technologies.ToListAsync());
        }

        /// <summary>
        /// Возвращает представление страницы технологии
        /// </summary>
        /// <param name="technology"></param>
        /// <returns></returns>
        public IActionResult Technology(Technology technology)
        {
            return View(technology);
        }

        /// <summary>
        /// Возвращает представление страницы редактирования технологии(может только админ и кураторы)
        /// </summary>
        /// <param name="technology"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin, curator")]
        public IActionResult TechnologyEdit(Technology technology)
        {
            List<SelectListItem> level = new List<SelectListItem>
            {
                new SelectListItem { Text = "Новичок", Value = "1" },
                new SelectListItem { Text = "Средний", Value = "2" },
                new SelectListItem { Text = "Опытный", Value = "3" },
                new SelectListItem { Text = "Мастер", Value = "4" }
            };
            SelectList levels = new SelectList(level,"Value", "Text");
            ViewBag.Level = levels;

            SelectList chars = new SelectList(db.Characteristics, "Id", "Name");
            chars = AddFirstItem(chars);
            ViewBag.Char = chars;


            var users = db.Users.ToList();
            users.Insert(0, new Models.User { Id = -1, Name = "", Surname = "" });
            IEnumerable<SelectListItem> selectList = from s in users where s.Email != "admin@mail.ru"
                                                     select new SelectListItem
                                                     {

                                                         Value = s.Id.ToString(),
                                                         Text = s.Name + " " + s.Surname
                                                     };
            ViewBag.Users = new SelectList(selectList, "Value", "Text");



            return View(technology);
        }

        /// <summary>
        /// Принимает данные с редактирования страницы, обновляет бд
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="charName"></param>
        /// <param name="charD"></param>
        /// <param name="test"></param>
        /// <param name="levelTest"></param>
        /// <param name="data"></param>
        /// <param name="levelData"></param>
        /// <param name="users"></param>
        /// <param name="SId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, curator")]
        public async Task<IActionResult> TechnologyEdit(int charId, string charName, string charD, 
                                                                    string test, int levelTest, 
                                                                    string data, int levelData,
                                                                    int users,
                                                                    int SId)
        {
            if (charId == -1)
            {
                if (charName != null)
                {
                    db.Characteristics.Add(new Characteristic { Name = charName });
                    await db.SaveChangesAsync();
                    Characteristic characteristic = db.Characteristics.FirstOrDefault(c => c.Name == charName);
                    db.TechnologyCharacteristics.Add(new TechnologyCharacteristic { Id = characteristic.Id, SId = SId, Description = charD });
                    await db.SaveChangesAsync();
                }
            }
            else
            {
                db.TechnologyCharacteristics.Add(new TechnologyCharacteristic { Id = charId, SId = SId, Description = charD });
                await db.SaveChangesAsync();
            }

            if (test != null)
            {
                db.TestLinks.Add(new TestLink { SId = SId, Level = levelTest, Link = test });
                await db.SaveChangesAsync();
            }

            if (data != null)
            {
                db.DataLinks.Add(new DataLink { SId = SId, Level = levelData, Link = data });
                await db.SaveChangesAsync();
            }

            if (users != -1)
            {
                db.Curators.Add(new Curator { SId = SId, Id = users });
                Authorization authorization = db.Authorizations.FirstOrDefault(a => a.Id == users);
                if (authorization.Role != "admin")
                {
                    authorization.Role = "curator";
                    db.Authorizations.Update(authorization);
                    await db.SaveChangesAsync();
                }
            }
            Technology technology = db.Technologies.FirstOrDefault(t => t.SId == SId);
            return RedirectToAction("Technology", "Technologies", technology);

        }


        /// <summary>
        /// Добавляет первый элемент в лист
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "-1", Text = "" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
        }

        

    }
}