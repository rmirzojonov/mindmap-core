using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace Project.Controllers
{
    /// <summary>
    /// Контроллер для работы со страницами пользователя
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// База данных
        /// </summary>
        private UserContext db = new UserContext();

        /// <summary>
        /// Конструктор для тестирование (комментировать во время выполнения приложения)
        /// </summary>
        public HomeController() { }

   
        

        /// <summary>
        /// Возвращает страницу пользователя с его данными
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult About()
        {
            try
            {
                User user = db.Users.FirstOrDefault(u => u.Email == @User.Identity.Name);
                return View(user);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Возвращает страницу добавления технологии
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult AddTech()
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

                ViewBag.Level = level;

                User user = db.Users.FirstOrDefault(u => u.Email == @User.Identity.Name);
                SelectList technologies = new SelectList(db.Technologies, "SId", "Name");
                technologies = AddFirstItem(technologies);
                ViewBag.Technologies = technologies;

                return View(user);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Принимает новую технологию, обновляет бд
        /// </summary>
        /// <param name="technologieid"></param>
        /// <param name="Id"></param>
        /// <param name="level"></param>
        /// <param name="tech"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTech(int technologieid,int Id, int level, string tech)
        {
            if (technologieid == -1)
            {
                if (tech == null)
                    return RedirectToAction("AddTech", "Home");
                db.Technologies.Add(new Technology { Name = tech });
                await db.SaveChangesAsync();
                Technology technology = db.Technologies.FirstOrDefault(t => t.Name == tech);
                technologieid = technology.SId;
            }
            db.UserTechnologies.Add(new UserTechnology { SId = technologieid, Id = Id, Level = level });
            await db.SaveChangesAsync();


            return RedirectToAction("About", "Home");
           
        }


        /// <summary>
        /// Возвращает страницу редактирования данных пользователя
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            try
            {
                User user = db.Users.FirstOrDefault(u => u.Email == @User.Identity.Name);
                return View(user);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }


        /// <summary>
        /// Принимает новые данные пользователя, обновляет бд
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="inf"></param>
        /// <param name="im"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(string name, string surname, string inf, IFormFile im)
        {
            User user = db.Users.FirstOrDefault(u => u.Email == @User.Identity.Name);
            if (name!=null)
                user.Name = name;
            if (surname != null)
                user.Surname = surname;
            user.Information = inf;
            if (im != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(im.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)im.Length);
                }
                // установка массива байтов
                user.Photo = imageData;
            }

            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("About", "Home");
        }


        /// <summary>
        /// Выход пользователя, удаление куки
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Добавляет первый элемент в лист
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public SelectList AddFirstItem(SelectList list)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            foreach (SelectListItem item in list)
            {
                _list.Add(new SelectListItem() { Value = item.Value, Text = item.Text });
            }
            _list.Insert(0, new SelectListItem() { Value = "-1", Text = "" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
        }

    }
}
