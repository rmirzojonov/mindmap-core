using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Project.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using Project.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

namespace Project.Controllers
{
    /// <summary>
    /// Контроллер для работы со страницами создания аккаунта и регистрации
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// База данных
        /// </summary>
        private UserContext db;

        /// <summary>
        /// Конструктор контроллера аккаунт
        /// </summary>
        /// <param name="context"></param>
        public AccountController(UserContext context)
        {
            db = context;
            DatabaseInitializeAsync();
        }

        /// <summary>
        /// Создание аккаунта админа
        /// </summary>
        private async void DatabaseInitializeAsync()
        {
            Authorization authorization = await db.Authorizations.FirstOrDefaultAsync(u => u.Email == "admin@mail.ru");
            if (authorization == null)
            {
                string adminRoleName = "admin";
                string adminEmail = "admin@mail.ru";
                string adminPassword = "admin";
                adminPassword = Md5(adminPassword);

                db.Authorizations.Add(new Authorization { Email = adminEmail, Password = adminPassword, Role = adminRoleName });
                await db.SaveChangesAsync();

                authorization = await db.Authorizations.FirstOrDefaultAsync(u => u.Email == adminEmail);
                db.Users.Add(new User { Email = adminEmail, Id = authorization.Id });
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Get метод для логина, возвращающий представление страницы логина
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Post метод логин, принимающий модель логина и проверяющий данные в бд
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string result = Md5(model.Password);

                Authorization authorization = await db.Authorizations.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == result);
                if (authorization != null)
                {
                    await Authenticate(authorization); // аутентификация

                    return RedirectToAction("About", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        /// <summary>
        /// Get метод для регистрации, возвращающий представление страницы регистрации
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Post метод регистрации, принимающий модель логина и сохраняющий данные в бд
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Authorization authorization = await db.Authorizations.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (authorization == null)
                {
                    string result = Md5(model.Password);
                    // добавляем пользователя в бд
                    db.Authorizations.Add(new Authorization { Email = model.Email, Password = result, Role = "user" });

                    await db.SaveChangesAsync();

                    authorization = await db.Authorizations.FirstOrDefaultAsync(u => u.Email == model.Email);
                    db.Users.Add(new User { Email = model.Email, Name = model.Name, Surname = model.Surname, Id = authorization.Id });
                    await db.SaveChangesAsync();

                    await Authenticate(authorization); // аутентификация

                    return RedirectToAction("About", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }



       
        /// <summary>
        /// Работа с куки, сохраняющий E-mail и Роль пользователя
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        private async Task Authenticate(Authorization authorization)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, authorization.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, authorization.Role)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        /// <summary>
        /// Выход пользователя, удаление куки
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Шифрование пароля
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public string Md5(string pass)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
            return BitConverter.ToString(checkSum).Replace("-", String.Empty);
        }
    }
}