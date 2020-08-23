using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
/// <summary>
/// Класс для html helper
/// </summary>
namespace Project.App_Code
{
    public static class TechnologiesList
    {
        /// <summary>
        /// Создание листа технологий пользователя
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static HtmlString CreateList(this IHtmlHelper html, int Id)
        {
            using (UserContext db = new UserContext())
            {
                //<tr><td class="active">.NET core</td><td>Новичок</td></tr>
                User user = db.Users.FirstOrDefault(u => u.Id == Id);
                var ListOfTechnologies = db.UserTechnologies.Where(ut => ut.Id == Id);
                string result = "";
                foreach (UserTechnology item in ListOfTechnologies)
                {
                    Technology technology = db.Technologies.FirstOrDefault(t => t.SId == item.SId);
                    String s = "";
                    if (item.Level == 1)
                        s = "Новичок";
                    else if (item.Level == 2)
                        s = "Средний";
                    else if (item.Level == 3)
                        s = "Опытный";
                    else if (item.Level == 4)
                        s = "Мастер";
                    result += $"<tr><td style=\"white-space: nowrap;  overflow: hidden; text-overflow: ellipsis; \" class=\"active\">" + technology.Name + "</td><td style=\"width: 30%; white-space: nowrap;  overflow: hidden; text-overflow: ellipsis; \">" + s + "</td></tr>";
                }
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Добавление кнопки в нужный момент
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static HtmlString AddButton(this IHtmlHelper html, int Id)
        {
            using (UserContext db = new UserContext())
            {
                string result = $"<span class=\"glyphicon glyphicon-plus-sign\" style=\"color:black\"></span>";
                return new HtmlString(result);
                //return new HtmlString("class=\"glyphicon glyphicon - plus - sign\" style=\"color: black\"");
            }
        }

        /// <summary>
        /// Список лейблов для графика
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static HtmlString GraphicLabels(this IHtmlHelper html)
        {
            using (UserContext db = new UserContext())
            {
                string result = "[";
                foreach (var item in db.Technologies)
                {
                    int l = Math.Min(15, item.Name.Length);
                    result = result + "\"" + item.Name.Substring(0,l) + "\"" + ',';
                }
                result = result.Substring(0, result.Length - 1);
                result += ']';
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Список данных для графика
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static HtmlString GraphicData(this IHtmlHelper html)
        {
            using (UserContext db = new UserContext())
            {
                string result = "[";
                int[] a = new int[db.Technologies.Count()];
                Array.Clear(a, 0, a.Length);
                foreach (var item in db.UserTechnologies)
                {
                    a[item.SId - 1]++;
                }
                for (int i = 0; i < a.Length; i++)
                {
                    result = result + a[i].ToString() + ',';
                }
                result = result.Substring(0, result.Length - 1);
                result += ']';
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Цвета графика 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static HtmlString GraphicBackground(this IHtmlHelper html)
        {
            using (UserContext db = new UserContext())
            {
                string result = "[";
                for (int i = 0; i < db.Technologies.Count(); i = i + 6)
                {
                    result = result + "'rgba(255, 99, 132, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(255, 206, 86, 0.2)', 'rgba(75, 192, 192, 0.2)', 'rgba(153, 102, 255, 0.2)', 'rgba(255, 159, 64, 0.2)',";
                }
                result = result.Substring(0, result.Length - 1);
                result += ']';
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Список характеристик технологии
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SId"></param>
        /// <returns></returns>
        public static HtmlString CreateListCharacteristics(this IHtmlHelper html, int SId)
        {
            using (UserContext db = new UserContext())
            {
                var ListOfCharacteristics = db.TechnologyCharacteristics.Where(tc => tc.SId == SId);
                string result = "";
                foreach (TechnologyCharacteristic item in ListOfCharacteristics)
                {
                    Characteristic characteristic = db.Characteristics.FirstOrDefault(c => c.Id == item.Id);
                    
                    result += $"<tr><td style=\"white-space: nowrap;  overflow: hidden; text-overflow: ellipsis; \" class=\"active\">" + characteristic.Name + "</td><td style=\"white-space: nowrap;  overflow: hidden; text-overflow: ellipsis; \" class=\"active\">" + item.Description + "</td></tr>";
                }
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Список тестов
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SId"></param>
        /// <returns></returns>
        public static HtmlString CreateListTests(this IHtmlHelper html, int SId)
        {
            using (UserContext db = new UserContext())
            {
                var ListOfTests = db.TestLinks.Where(tl => tl.SId == SId);
                string result = "";
                int i = 1;
                foreach (TestLink item in ListOfTests)
                {
                    String s = "";
                    if (item.Level == 1)
                        s = "Новичок";
                    else if (item.Level == 2)
                        s = "Средний";
                    else if (item.Level == 3)
                        s = "Опытный";
                    else if (item.Level == 4)
                        s = "Мастер";
                    result += $"<tr><td class=\"active\">" + "<a href =" + item.Link + ">" + i.ToString() + "</a>" + "</td><td>" + s + "</td></tr>";
                    i++;
                }
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Список ссылок на материалы
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SId"></param>
        /// <returns></returns>
        public static HtmlString CreateListData(this IHtmlHelper html, int SId)
        {
            using (UserContext db = new UserContext())
            {
                var ListOfData = db.DataLinks.Where(tl => tl.SId == SId);
                string result = "";
                int i = 1;
                foreach (DataLink item in ListOfData)
                {
                    String s = "";
                    if (item.Level == 1)
                        s = "Новичок";
                    else if (item.Level == 2)
                        s = "Средний";
                    else if (item.Level == 3)
                        s = "Опытный";
                    else if (item.Level == 4)
                        s = "Мастер";
                    result += $"<tr><td style=\"width:50%\" class=\"active\">" + "<a href =" + item.Link + ">" + i.ToString() + "</a>" + "</td><td>" + s + "</td></tr>";
                    i++;
                }
                return new HtmlString(result);
            }
        }

        /// <summary>
        /// Список кураторов
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SId"></param>
        /// <returns></returns>
        public static HtmlString CreateListCurators(this IHtmlHelper html, int SId)
        {
            using (UserContext db = new UserContext())
            {
                var ListOfCurators = db.Curators.Where(tl => tl.SId == SId);
                string result = "";
                foreach (Curator item in ListOfCurators)
                {
                    User user = db.Users.FirstOrDefault(u => u.Id == item.Id);
                    result += $"<tr><td style=\"white-space: nowrap;  overflow: hidden; text-overflow: ellipsis; \" class=\"active\">" + user.Name + " " + user.Surname + "</td>" + "</tr>";
                }
                return new HtmlString(result);
            }
        }

        
        /// <summary>
        /// Проверяет роль 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String Admin(this IHtmlHelper html, String s)
        {
            using (UserContext db = new UserContext())

            {
                
                Authorization authorization = db.Authorizations.FirstOrDefault(a => a.Email == s);
                if (authorization != null)
                    return (authorization.Role);
                else
                    return ("");

            }
        }

        /// <summary>
        /// Список технологий в должности
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static HtmlString CreateListTechnologies(this IHtmlHelper html, int Id)
        {
            using (UserContext db = new UserContext())
            {
                //<tr><td class="active">.NET core</td><td>Новичок</td></tr>
                User user = db.Users.FirstOrDefault(u => u.Id == Id);
                var ListOfTechnologies = db.Grades.Where(g => g.Id == Id);
                string result = "";
                foreach (Grade item in ListOfTechnologies)
                {
                    Technology technology = db.Technologies.FirstOrDefault(t => t.SId == item.SId);
                    String s = "";
                    if (item.Level == 1)
                        s = "Новичок";
                    else if (item.Level == 2)
                        s = "Средний";
                    else if (item.Level == 3)
                        s = "Опытный";
                    else if (item.Level == 4)
                        s = "Мастер";
                    result += $"<tr><td style=\"white - space: nowrap; overflow: hidden; text - overflow: ellipsis; \"class=\"active\">" + technology.Name + "</td><td>" + s + "</td></tr>";
                }
                return new HtmlString(result);
            }
        }

    }

    

}
