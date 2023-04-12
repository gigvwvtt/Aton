using project.Data;
using project.Models;

namespace project;

public static class Seed
{
    public static void SeedData(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                context.Users.AddRange(new List<User>()
                {
                    new User()
                    {
                        Guid = Guid.NewGuid(),
                        Login = "admin",
                        Password = "passwordAdmin",
                        Name = "администратор",
                        Gender = 2,
                        Birthday = DateTime.MinValue,
                        Admin = true,
                        CreatedOn = DateTime.Now.AddMonths(-2),
                        CreatedBy = "",
                        ModifiedOn = DateTime.Now.AddMonths(-2),
                        ModifiedBy = ""
                    },
                    new User()
                    {
                        Guid = Guid.NewGuid(),
                        Login = Guid.NewGuid().ToString().Skip(3).Take(7).ToString(),
                        Password = Guid.NewGuid().ToString().Skip(2).Take(5).ToString(),
                        Name = "юзер1",
                        Gender = Random.Shared.Next(0, 2),
                        Birthday = DateTime.Now.AddDays(-3555),
                        Admin = false,
                        CreatedOn = DateTime.Now.AddMonths(-1).AddDays(-2),
                        CreatedBy = "admin",
                        ModifiedOn = DateTime.Now.AddMonths(-2).AddDays(-2),
                        ModifiedBy = "admin",
                        RevokedOn = DateTime.Now.AddDays(-2),
                        RevokedBy = "admin"
                    },
                    new User()
                    {
                        Guid = Guid.NewGuid(),
                        Login = Guid.NewGuid().ToString().Skip(3).Take(7).ToString(),
                        Password = Guid.NewGuid().ToString().Skip(2).Take(5).ToString(),
                        Name = "юзер1",
                        Gender = Random.Shared.Next(0, 2),
                        Birthday = DateTime.Now.AddDays(-3269),
                        Admin = false,
                        CreatedOn = DateTime.Now.AddMonths(-1).AddDays(-2),
                        CreatedBy = "admin",
                        ModifiedOn = DateTime.Now.AddMonths(-2).AddDays(-2),
                        ModifiedBy = "admin"
                    },
                    new User()
                    {
                        Guid = Guid.NewGuid(),
                        Login = Guid.NewGuid().ToString().Skip(3).Take(7).ToString(),
                        Password = Guid.NewGuid().ToString().Skip(2).Take(5).ToString(),
                        Name = "юзер1",
                        Gender = Random.Shared.Next(0, 2),
                        Birthday = DateTime.Now.AddDays(-3245),
                        Admin = false,
                        CreatedOn = DateTime.Now.AddMonths(-1).AddDays(-2),
                        CreatedBy = "admin",
                        ModifiedOn = DateTime.Now.AddMonths(-2).AddDays(-2),
                        ModifiedBy = "admin"
                    },
                    new User()
                    {
                        Guid = Guid.NewGuid(),
                        Login = Guid.NewGuid().ToString().Skip(3).Take(7).ToString(),
                        Password = Guid.NewGuid().ToString().Skip(2).Take(5).ToString(),
                        Name = "юзер1",
                        Gender = Random.Shared.Next(0, 2),
                        Birthday = DateTime.Now.AddDays(-4165),
                        Admin = false,
                        CreatedOn = DateTime.Now.AddMonths(-1).AddDays(-2),
                        CreatedBy = "admin",
                        ModifiedOn = DateTime.Now.AddMonths(-2).AddDays(-2),
                        ModifiedBy = "admin",
                        RevokedOn = DateTime.Now.AddDays(-5),
                        RevokedBy = "admin"
                    },
                    new User()
                    {
                        Guid = Guid.NewGuid(),
                        Login = Guid.NewGuid().ToString().Skip(3).Take(7).ToString(),
                        Password = Guid.NewGuid().ToString().Skip(2).Take(5).ToString(),
                        Name = "юзер1",
                        Gender = Random.Shared.Next(0, 2),
                        Birthday = DateTime.Now.AddDays(-2395),
                        Admin = false,
                        CreatedOn = DateTime.Now.AddMonths(-1).AddDays(-2),
                        CreatedBy = "admin",
                        ModifiedOn = DateTime.Now.AddMonths(-2).AddDays(-2),
                        ModifiedBy = "admin"
                    }
                });
                
                context.SaveChanges();
            }
        }
    }
}