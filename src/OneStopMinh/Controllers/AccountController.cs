using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using OneStopMinh.Models;
using OneStopMinh.ViewModels;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading;

namespace OneStopMinh.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var thisTourist = _db.Tourists.Include(tourists => tourists.Attractions).FirstOrDefault(item => item.UserName == User.Identity.Name);
                return View(thisTourist);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.UserName };
            user.Email = model.Email;
            user.ConfirmedEmail = false;
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var emailMessage = new MimeMessage();
        

                emailMessage.From.Add(new MailboxAddress("Obviously It's Minh", "Hallo@example.com"));
                emailMessage.To.Add(new MailboxAddress(model.UserName, user.Email));
                emailMessage.Subject = "Email confirmation";
                emailMessage.Body = new TextPart("html")
                {
                    Text = string.Format("Dear {0} <br/> Thank you for your registration, please click on the below link to complete your registration: <a href='http://localhost:50365/{1}'>Click here</a>",
                model.UserName, Url.Action("ConfirmEmail", "Account",new { Token = user.Id, Email = user.Email }))
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    client.Authenticate("mikejung107@gmail.com", "Cupcake12#");
                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                };
                return RedirectToAction("Confirm", "Account", new { Email = user.Email });
            }
            else
            {
               ViewBag.Error = "Please re-enter your credentials";
                return View();
            }
        }


        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.ConfirmedEmail = true;
                    await _userManager.UpdateAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home", new { ConfirmedEmail = user.Email });
                }
                else
                {
                    return RedirectToAction("Confirm", "Account", new { Email = user.Email });
                }
            }
            else
            {
                return RedirectToAction("Confirm", "Account", new { Email = "" });
            }
        }

        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            ViewBag.Email = Email; return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                if (user.ConfirmedEmail == true)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = "No match for username and password";
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Confirm Email Address.");
                }
            }
                return View(model);
            }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tourist tourist, IFormFile avatar)
        {
            tourist.UserName = User.Identity.Name;
            byte[] profilePic = ConvertToBytes(avatar);
            tourist.Pic = profilePic;
            _db.Tourists.Add(tourist);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var editedTourist = _db.Tourists.FirstOrDefault(tourists => tourists.TouristId == id);
            return View(editedTourist);
        }

        [HttpPost]
        public IActionResult Edit(Tourist tourist, IFormFile avatar)
        {
            var editedTourist = _db.Tourists.FirstOrDefault(tourists => tourists.TouristId == tourist.TouristId);
            _db.Tourists.Attach(editedTourist);
            byte[] profilePic = ConvertToBytes(avatar);
            editedTourist.Name = tourist.Name;
            editedTourist.Pic = profilePic;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var deleteTourist = _db.Tourists.FirstOrDefault(tourists => tourists.TouristId == id);
            return View(deleteTourist);
        }

        [HttpPost, ActionName ("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var deleteTourist = _db.Tourists.FirstOrDefault(tourists => tourists.TouristId == id);
            var user = await _userManager.FindByNameAsync(deleteTourist.UserName);

                var results = await _userManager.DeleteAsync(user);

                if (results.Succeeded)
                    {
                    await _signInManager.SignOutAsync();
                        _db.Entry(deleteTourist.Attractions).State = EntityState.Deleted;
                     _db.Tourists.Remove(deleteTourist);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                  {
                return View(deleteTourist);
            }
        }

        private byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)image.Length);
            return CoverImageBytes;
        }

        
    }
}
