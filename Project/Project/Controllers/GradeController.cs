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
    /// Контроллер для работы со страницами должностей
    /// </summary>
    public class GradeController : Controller
    {
        /// <summary>
        /// База данных
        /// </summary>
        private UserContext db = new UserContext();

        /// <summary>
        /// Конструктор для тестирование 
        /// </summary>
        public GradeController() { }



        /// <summary>
        /// Возвращает все должности в представление
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ListOfGrades()
        {
            return View(await db.GradeNames.ToListAsync());
        }

        /// <summary>
        /// Возвращает представление страницы должности
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public IActionResult Grade(GradeName grade)
        {
            try
            {
                return View(grade);
            }
            catch
            {
                return RedirectToAction("Login","Account");
            }
        }

        /// <summary>
        /// Возвращает представление страницы редактирования должности(может только админ)
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GradeEdit(GradeName grade)
        {
            try
            {
                List<SelectListItem> level = new List<SelectListItem>
            {
                new SelectListItem { Text = "Новичок", Value = "1" },
                new SelectListItem { Text = "Средний", Value = "2" },
                new SelectListItem { Text = "Опытный", Value = "3" },
                new SelectListItem { Text = "Мастер", Value = "4" }
            };
                SelectList levels = new SelectList(level, "Value", "Text");
                ViewBag.Level = levels;

                SelectList tech = new SelectList(db.Technologies, "SId", "Name");
                tech = AddFirstItem(tech);
                ViewBag.Tech = tech;

                return View(grade);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Принимает данные с редактирования страницы, обновляет бд
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="Id"></param>
        /// <param name="level"></param>
        /// <param name="tech"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GradeEdit(int techId, int Id, int level, string tech)
        {
            GradeName GN = db.GradeNames.FirstOrDefault(gn => gn.Id == Id);
            if (techId == -1)
            {
                if (tech == null)
                    return RedirectToAction("Grade", "Grade", GN);
                db.Technologies.Add(new Technology { Name = tech });
                await db.SaveChangesAsync();
                Technology technology = db.Technologies.FirstOrDefault(t => t.Name == tech);
                techId = technology.SId;
            }
            db.Grades.Add(new Grade { SId = techId, Id = Id, Level = level, NId = 2 });
            await db.SaveChangesAsync();
            return RedirectToAction("Grade", "Grade", GN);

        }

        /// <summary>
        /// Возвращает представление добавления должности
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddGrade()
        {
            return View(await db.GradeNames.ToListAsync());
        }

        /// <summary>
        /// Принимает новую должность, сохраняет её в бд
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddGrade(string grade)
        {
            if (grade != null)
            {
                db.GradeNames.Add(new GradeName { Name = grade });
                await db.SaveChangesAsync();
            }
            return RedirectToAction("ListOfGrades", "Grade");
        }

        /// <summary>
        /// Добавляет первый элемент в лист
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            foreach (SelectListItem item in list) {
                _list.Add(new SelectListItem() { Value = item.Value, Text = item.Text });
            }
            _list.Insert(0, new SelectListItem() { Value = "-1", Text = "" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
        }
    }

}