using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SA51_CA_Project_Team10.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.DBs
{
    public class DbSeeder
    {
        private readonly DbT10Software _db;

        public DbSeeder(DbT10Software db)
        {
            _db = db;
        }
        public void Seed()
        {
            CreateUsers();
            CreateProducts();
            CreateCarts();
            CreateOrders(10);
            CreateRatings(50);
            CreateCleanUser("cherwah");
        }

        private void CreateUsers()
        {
            // Every user has the same login and password information for seeding and testing
            // Example - Username: angelia  Password: angelia
            string[] users = { "angelia", "derek", "jianyuan", "site", "yitong", "yubo", "zifeng" };
            foreach (string user in users)
            {
                byte[] salt = GenerateSalt();
                _db.Add(new User
                {
                    Username = user,
                    Salt = Convert.ToBase64String(salt),
                    Password = GenerateHashString(user, salt)
                });
            }
            _db.SaveChanges();
        }

        private void CreateCleanUser(string user)
        {
            // Clean user seeding to simulate fresh account
            byte[] salt = GenerateSalt();
            _db.Add(new User
            {
                Username = user,
                Salt = Convert.ToBase64String(salt),
                Password = GenerateHashString(user, salt)
            });
            _db.SaveChanges();
        }


        private void CreateProducts()
        {
            
            string[] names = { "کتانی ورزشی سبک", "کتانی ورزشی", "کتانی برای وزن بالا", "کتانی ورزش های هوازی", "کتانی ورزش های سنگین", "کتانی پیاده روی", 
                               "کتانی ویژه بانوان", "کتانی خوش قیمت", "کتانی سبک", "کتانی فوق سبک", "کتانی زیره چرم", "کتانی پلیمری",
                               "کتانی ویژه"};
            string[] descriptions =
            {
                "مناسب برای ورزشهای سبک روزانه",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن",
                "مناسب ورزش های مختلف با هر وزن و سن"
            };
            double[] prices = { 99, 69, 299, 299, 49, 199, 65, 15, 35, 55, 25, 45, 35.5 };
            string[] imagelinks =
            {
                "shoe (1).jpg",
                "shoe (2).jpg",
                "shoe (3).jpg",
                "shoe (4).jpg",
                "shoe (5).jpg",
                "shoe (6).jpg",
                "shoe (7).jpg",
                "shoe (8).jpg",
                "shoe (1).jpg",
                "shoe (2).jpg",
                "shoe (3).jpg",
                "shoe (4).jpg",
                "shoe (1).jpg"
            };

            for (int i = 0; i < names.Length; i++)
            {
                _db.Add(new Product
                {
                    Name = names[i],
                    Description = descriptions[i],
                    Price = prices[i],
                    ImageLink = "~/images/software/" + imagelinks[i]
                });
            }
            _db.SaveChanges();
        }

        private void CreateCarts()
        {
            // Randomly seeds and generates carts for all initial users (non-clean)
            List<User> users = _db.Users.ToList();
            Random r = new Random();
            List<Product> products = _db.Products.ToList();

            foreach (User user in users)
            {
                List<int> used = new List<int>();
                for (int i = 0; i < r.Next(1, 5); i++)
                {
                    var randomNumber = r.Next(products.Count);
                    while (used.Contains(randomNumber))
                    {
                        randomNumber = r.Next(products.Count);
                    }
                    used.Add(randomNumber);
                    var randomProduct = products[randomNumber];

                    randomNumber = r.Next(1, 6);
                    var randomCart = new Cart { ProductId = randomProduct.Id, UserId = user.Id, Quantity = randomNumber };
                    _db.Carts.Add(randomCart);
                }
                _db.SaveChanges();
            }
        }

        private void CreateOrders(int orders)
        {
            // Randomly seeds and generates orders for all initial users (non-clean)
            List<User> users = _db.Users.ToList();
            List<Product> products = _db.Products.ToList();

            Random r = new Random();

            for (int i = 0; i < orders; i++)
            {
                List<int> used = new List<int>();
                List<string> usedKeys = new List<string>();
                var startDate = new DateTime(2010, 1, 1);
                var randomUser = users[r.Next(users.Count)];
                var randomDate = startDate.AddDays(r.Next((DateTime.Today - startDate).Days));
                var randomOrder = new Order { UserId = randomUser.Id, DateTime = randomDate };
                _db.Orders.Add(randomOrder);
                _db.SaveChanges();

                for (int j = 0; j < r.Next(1, 6); j++)
                {
                    var randomNumber = r.Next(products.Count);
                    while (used.Contains(randomNumber))
                    {
                        randomNumber = r.Next(products.Count);
                    }
                    used.Add(randomNumber);
                    var randomProduct = products[randomNumber];
                    var randomQuantity = r.Next(1, 6);
                    for (int k = 0; k <= randomQuantity; k++)
                    {
                        var randomActivation = GenerateActivationKey(r);
                        while (usedKeys.Contains(randomActivation))
                        {
                            randomActivation = GenerateActivationKey(r);
                        }
                        usedKeys.Add(randomActivation);
                        var randomItem = new OrderDetail { Id = randomActivation, OrderId = randomOrder.Id, ProductId = randomProduct.Id };
                        _db.OrderDetails.Add(randomItem);
                    }
                    
                    _db.SaveChanges();
                }
            }
        }

        private string GenerateActivationKey(Random r)
        {
            const string chars = "abcdefghiijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 14; i++)
            {
                if (i == 4 || i == 9) { sb.Append("-"); }
                else { sb.Append(chars[r.Next(chars.Length)]); }
            }
            return sb.ToString();
        }

        public static string GenerateHashString(string password, byte[] salt)
        {
            byte[] encrypted = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 32);
            return Convert.ToBase64String(encrypted);
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        
        public void CreateRatings(int numOfRatingsToGenerate)
        {
            int numOfProducts = _db.Products.ToList().Count();
            int numOfUsers = _db.Users.ToList().Count();
            Random r = new Random();
            
            for (int i = 0; i < numOfRatingsToGenerate; i++)
            {
                Rating rating = null;
                // Limit one rating per account per product to simulate actual usage which does not allow more than one rating on a product
                while (rating == null || _db.Ratings.FirstOrDefault(x => x.UserId == rating.UserId && x.ProductId == rating.ProductId) != null)
                {
                    rating = new Rating
                    {
                        Score = r.Next(1, 6),
                        UserId = r.Next(1, numOfUsers + 1),
                        ProductId = r.Next(1, numOfProducts + 1)
                    };
                };

                _db.Ratings.Add(rating);
                _db.SaveChanges();
            }
        }
    }
}
