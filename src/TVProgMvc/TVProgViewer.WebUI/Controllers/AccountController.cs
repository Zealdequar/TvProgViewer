﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System.Threading.Tasks;
using LogicUsers = TVProgViewer.BusinessLogic.Users;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.WebUI.Concrete;
using TVProgViewer.CryptoService;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.BusinessLogic.Users;

namespace TVProgViewer.WebUI.Controllers
{
    /// <summary>
    /// Контроллер для операций с личным кабинетом
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IUsersRepository _repository;

        public AccountController()  {}
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="usersRepository">Пользовательский репозиторий</param>
        public AccountController(IUsersRepository usersRepository)
        {
            _repository = usersRepository;
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        [AllowAnonymous]
        public ActionResult Register()
        {
            LogicUsers.User model = new LogicUsers.User();

            return View("Registration", model);
        }

        /// <summary>
        /// Регистрация POST-обработка
        /// </summary>
        /// <param name="user">Пользователь</param>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(LogicUsers.User user)
        {
            if (ModelState.IsValidField("UserName") && ModelState.IsValidField("Pass") &&
                ModelState.IsValidField("PassRepeate") && user.Pass != user.PassRepeate)
            {
                ModelState.AddModelError("", "Указанные строки пароля и подтверждения пароля должны совпадать");
            }
            if (ModelState.IsValidField("BirthDate") && user.BirthDate > DateTime.Now)
            {
                ModelState.AddModelError("", "Укажите дату рождения в прошлом");
            }
            if (ModelState.IsValid)
            {
                PBKDF2 pbkdf2 = new PBKDF2();
                string passHash = pbkdf2.Compute(user.PassRepeate);
                _repository.UserRegister(user.UserName, passHash, pbkdf2.Salt, user.LastName, user.FirstName, user.MiddleName, user.BirthDate,
                                         user.Gender, user.Email, user.MobilePhone, user.OtherPhone1, user.OtherPhone2, user.Address, user.GmtZone);
                TempData["message"] = $"{user.UserName} успешно зарегистрирован.";
                return RedirectToAction("List", "Programme", null);
            }
            else
            {
                return View("Registration", user);
            }
        }
       
        /// <summary>
        /// Обработка страницы входа
        /// </summary>
        /// <param name="returnUrl">Возвращаемый URL</param>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Вход POST-обработка
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="returnUrl">Возвращаемый URL</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LogicUsers.LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            LogicUsers.User user = new LogicUsers.User() { UserName = model.Login, Pass = model.Password };

            SecureData secureData = await _repository.GetHashes(user.UserName);
            PBKDF2 pbkdf2 = new PBKDF2();
            if (secureData == null)
            {
                ModelState.AddModelError("", "Логин и Пароль введены неверно! Укажите правильные Логин и Пароль.");
                return View(model);
            }

            string hash = pbkdf2.Compute(user.Pass, secureData.PassExtend);
            if (pbkdf2.Compare(hash, secureData.PassHash))
            {
                int errCode = 0;
                User userAuth = await _repository.GetUser(secureData.UID, out errCode);
                if (userAuth != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, true);

                    var authTicket = new FormsAuthenticationTicket(1, userAuth.UserName, DateTime.Now, DateTime.Now.AddDays(7), false, $"User,{secureData.UID}");
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    HttpContext.Response.Cookies.Add(authCookie);
                    Session["ClientCode"] = secureData.UID;
                    
                    return RedirectToAction("List", "Programme");
                }
                else
                {
                    ModelState.AddModelError("", "Логин и Пароль введены неверно! Укажите правильные Логин и Пароль.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Логин и Пароль введены неверно! Укажите правильные Логин и Пароль.");
                return View(model);
            }
        }

        /// <summary>
        /// Выход
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session["ClientCode"] = null;
            return RedirectToAction("List", "Programme");
        }
    }
}