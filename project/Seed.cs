using Bogus;
using Bogus.DataSets;
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
                context.Users.Add(
                    new User
                    {
                        Guid = Guid.NewGuid(),
                        Login = "admin",
                        Password = "admin",
                        Name = "администратор",
                        Gender = 2,
                        Birthday = DateTime.MinValue,
                        Admin = true,
                        CreatedOn = DateTime.Now.AddMonths(-2),
                        CreatedBy = "",
                        ModifiedOn = DateTime.Now.AddMonths(-2),
                        ModifiedBy = ""
                    }
                );

                context.Users.AddRange(GenerateUsers(1000));
                context.SaveChanges();
            }
        }
    }

    private static IEnumerable<User> GenerateUsers(int amount)
    {
        var users = new UserFaker().Generate(amount);
        return users;
    }
}

public sealed class UserFaker : Faker<User>
{
    internal UserFaker()
    {
        Date.SystemClock = () => DateTime.Parse("2-2-2023 21:10");

        UseSeed(654191)
            .RuleFor(u => u.Guid, _ => Guid.NewGuid())
            .RuleFor(u => u.Gender, f => f.PickRandom(0, 1, 2))
            .RuleFor(u => u.Name, (f, u) => f.Name.FullName())
            .RuleFor(u => u.Login, (f, u) => f.Internet.UserName(u.Name))
            .RuleFor(u => u.Password, (f, u) => f.Internet.Password())
            .RuleFor(u => u.Birthday, (f, u) => f.Person.DateOfBirth)
            .RuleFor(u => u.Admin, (f, u) => false)
            .RuleFor(u => u.CreatedOn, (f, u) => f.Date.Past())
            .RuleFor(u => u.CreatedBy, (f, u) => f.PickRandom(u.Login, "admin"))
            .RuleFor(u => u.ModifiedOn, (f, u) => f.Date.Between(u.CreatedOn, DateTime.Now))
            .RuleFor(u => u.ModifiedBy, (f, u) => f.PickRandom(u.Login, "admin"))
            .RuleFor(u => u.RevokedOn,
                (f, u) => f.PickRandom(f.Date.Between(u.CreatedOn, DateTime.Now)).OrNull(f, 0.7f))
            .RuleFor(u => u.RevokedBy,
                (f, u) => u.RevokedOn == null ? default : f.PickRandom(u.Login, "admin"));
    }
}