

using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Praeclarum_Tech.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcFormAuthentication_Demo.Controllers
{

    public class HomeController : Controller
    {
        private readonly TaskEntities _dbContext = new TaskEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public static string GenerateRandomKey(int keyLength)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomBytes = new byte[keyLength];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            var chars = new char[keyLength];
            var charLength = validChars.Length;
            for (int i = 0; i < keyLength; i++)
            {
                chars[i] = validChars[randomBytes[i] % charLength];
            }
            return new string(chars);
        }


        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var dbUser = _dbContext.Users.FirstOrDefault(u => u.Username.ToLower() == user.Username.ToLower());
                    var tokencheck = _dbContext.Users.Where(a => a.Token == dbUser.Token).FirstOrDefault();
                    if (dbUser != null && tokencheck!=null)
                    {


                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid Username or Password");
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Register(User registerUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var keyLength = 32;
                    var secretKey = GenerateRandomKey(keyLength);
                    var key = Encoding.ASCII.GetBytes(secretKey); // Replace with your secret key
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.Name, registerUser.Username),

                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);


                    string encryptedPassword = EncryptPassword(registerUser.Password);
                    registerUser.Password = encryptedPassword;

                    registerUser.Token = tokenString;
                    _dbContext.Users.Add(registerUser);
                    _dbContext.SaveChanges();
                    return RedirectToAction("Login");
                }
                return View();
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", "Home");
            }
        }

        private string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private string DecryptPassword(string encryptedPassword)
        {
            // Replace "your_secret_key_here" with your actual secret key
            var keyLength = 32;
            var secretKeys = GenerateRandomKey(keyLength);
            string secretKey = secretKeys;
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(secretKey);
                aesAlg.IV = new byte[16]; // Example IV, should be unique per encrypted value

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and return as string.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }
    }
}