using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Info.Models;

namespace Info.Data
{
    public class InfoSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))

                if (dbContext.Database.CanConnect())
                {
                    SeedRoles(dbContext);
                    SeedUsers(dbContext);
                    SeedCategoris(dbContext);
                    SeedTexts(dbContext);
                    SeedOpinions(dbContext);
                }
        }

        //zakładanie ról w apliakcji, o ile nie istnieją
        private static void SeedRoles(ApplicationDbContext dbContext)
        {
            var roleStore = new RoleStore<IdentityRole>(dbContext);

            if (!dbContext.Roles.Any(r => r.Name == "admin"))
            {
                roleStore.CreateAsync(new IdentityRole
                {
                    Name = "admin",
                    NormalizedName = "admin"
                }).Wait();
            }

            if (!dbContext.Roles.Any(r => r.Name == "author"))
            {
                roleStore.CreateAsync(new IdentityRole
                {
                    Name = "author",
                    NormalizedName = "author"
                }).Wait();
            }
        }  // koniec ról

        //zakładanie kont uzytkowników w apliakcji, o ile nie istnieją
        private static void SeedUsers(ApplicationDbContext dbContext)
        {
            if (!dbContext.Users.Any(u => u.UserName == "autor1@portal.pl"))
            {
                var user = new AppUser
                {
                    UserName = "autor1@portal.pl",
                    NormalizedUserName = "autor1@portal.pl",
                    Email = "autor1@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Piotr",
                    LastName = "Pisarski",
                    Photo = "autor1.jpg",
                    Information = "Wszechstronny programista aplikacji sieciowych i internetowych. W portfolio ma kilka ciekawych projektów zrealizowanych dla firm z branży finansowej. Współpracuje z innowacyjnymi startupami."
                };
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Portalik1!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(dbContext);
                userStore.CreateAsync(user).Wait();
                userStore.AddToRoleAsync(user, "author").Wait();

                dbContext.SaveChanges();
            }

            if (!dbContext.Users.Any(u => u.UserName == "autor2@portal.pl"))
            {
                var user = new AppUser
                {
                    UserName = "autor2@portal.pl",
                    NormalizedUserName = "autor2@portal.pl",
                    Email = "autor2@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Anna",
                    LastName = "Autorska",
                    Photo = "autor2.jpg",
                    Information = "Doświadczona programistka i projektantka stron internetowych oraz uznana blogierka. Specjalizuje się w HTML5, CSS3, JavaScript, jQuery i Bootstrap. Obecnie pracuje nad nowymi rozwiązaniami dla graczy."
                };
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Portalik1!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(dbContext);
                userStore.CreateAsync(user).Wait();
                userStore.AddToRoleAsync(user, "author").Wait();
                dbContext.SaveChanges();
            }

            if (!dbContext.Users.Any(u => u.UserName == "admin@portal.pl"))
            {
                var user = new AppUser
                {
                    UserName = "admin@portal.pl",
                    NormalizedUserName = "admin@portal.pl",
                    Email = "admin@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Ewa",
                    LastName = "Ważna",
                    Photo = "woman.png",
                    Information = ""
                };
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "Portalik1!");
                user.PasswordHash = hashed;

                var userStore = new UserStore<AppUser>(dbContext);
                userStore.CreateAsync(user).Wait();
                userStore.AddToRoleAsync(user, "admin").Wait();
                dbContext.SaveChanges();
            }
        } // koniec użytkowników

        //dodawanie danych kategorii
        private static void SeedCategoris(ApplicationDbContext dbContext)
        {
            if (!dbContext.Categories.Any())
            {
                var kat = new List<Category>
                {
                    new Category { Name = "Wiadomości", Active = true, Display=true, Icon="chat-left-text", Description="Najświeższe wiadomości i informacje z dziedziny informatyki. Coś dla programistów i zwykłych użytkowników komputerów, tabletów oraz smartfonów." },
                    new Category { Name = "Artykuły", Active = true, Display=true, Icon="journal-richtext", Description="Artykuły w naszym serwisie pisane są przez wybitnych znawców tematu, którzy z olbrzymią przenikliwością zgłębiają każdy temat."},
                    new Category { Name = "Testy", Active = true, Display=true, Icon="speedometer", Description="Nasze laboratorium testuje dla Was najnowszy sprzęt, poddając go elektronicznym torturom i wyciskając siódme poty elektronów."},
                    new Category { Name = "Porady", Active = true, Display=true, Icon="life-preserver", Description="Jeżeli wciąż masz problemy z obsługą komputerów lub chcesz pracowaæ efektywnie zajrzyj do sekcji z poradami dla adminów i laików." },
                    new Category { Name = "Tutoriale", Active = true, Display=true, Icon="display", Description="W tutorialach opisujemy krok po kroku, w jaki sposób rozwiązać zadania programistyczne praktycznie z każdej dziedziny." },
                    new Category { Name = "Recenzje", Active = true, Display=true, Icon="controller", Description="Czytamy najciekawsze książki informatyczne i gramy dla Was w najnowsze gry, aby później opisać je dokładnie w tym dziale." }
                };
                dbContext.AddRange(kat);
                dbContext.SaveChanges();
            }
        } //koniec danych kategorii

        //dodawanie danych tekstów, o ile nie istnieją
        private static void SeedTexts(ApplicationDbContext dbContext)
        {
            if (!dbContext.Texts.Any())
            {
                for (int i = 1; i <= 6; i++) //sześć kategorii
                {
                    var idUzytkownika1 = dbContext.AppUsers
                    .Where(u => u.UserName == "autor1@portal.pl")
                    .FirstOrDefault()
                    .Id;

                    for (int j = 0; j <= 4; j++) //teksty autora1
                    {
                        var tekst = new Text()
                        {
                            Title = "Tytuł" + i.ToString() + j.ToString(),
                            Summary = "Streszczenie tekstu o tytule Tytuł" + i.ToString() + j.ToString(),
                            Keywords = "tag" + j.ToString() + ", tag" + (i + j).ToString(),
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                            AddedDate = DateTime.Now.AddDays(-i * j),
                            CategoryId = i,
                            Id = idUzytkownika1,
                            Active = true
                        };
                        dbContext.Set<Text>().Add(tekst);
                    }
                    dbContext.SaveChanges();

                    var idUzytkownika2 = dbContext.AppUsers
                    .Where(u => u.UserName == "autor2@portal.pl")
                    .FirstOrDefault()
                    .Id;

                    for (int j = 5; j <= 9; j++) //teksty autora2
                    {
                        var tekst = new Text()
                        {
                            Title = "Tytuł" + i.ToString() + j.ToString(),
                            Summary = "Streszczenie tekstu o tytule Tytuł" + i.ToString() + j.ToString(),
                            Keywords = "tag" + j.ToString() + ", tag" + (i + j).ToString(),
                            Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
                            AddedDate = DateTime.Now.AddDays(-i * j),
                            CategoryId = i,
                            Id = idUzytkownika2,
                            Active = true
                        };
                        dbContext.Set<Text>().Add(tekst);
                    }
                    dbContext.SaveChanges();
                }
            }
        } // koniec danych tekstów

        //dodawanie treści opinii, o ile nie istnieją
        private static void SeedOpinions(ApplicationDbContext dbContext)
        {
            if (!dbContext.Opinions.Any())
            {
                var idUzytkownika1 = dbContext.AppUsers
                .Where(u => u.UserName == "autor2@portal.pl").FirstOrDefault()
                .Id;

                for (int i = 1; i <= 60; i++) //sześćdziesiąt tekstów
                {
                    var komentarz = new Opinion()
                    {
                        Comment = "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                        AddedDate = DateTime.Now.AddDays(-i),
                        Id = idUzytkownika1,
                        TextId = i,
                        Rating = (TypeOfGrade?)5
                    };
                    dbContext.Set<Opinion>().Add(komentarz);
                }
                dbContext.SaveChanges();

                var idUzytkownika2 = dbContext.AppUsers
                .Where(u => u.UserName == "autor1@portal.pl").FirstOrDefault()
                .Id;

                for (int i = 1; i <= 60; i++)
                {
                    var komentarz = new Opinion()
                    {
                        Comment = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident.",
                        AddedDate = DateTime.Now.AddDays(-i),
                        Id = idUzytkownika2,
                        TextId = i,
                        Rating = (TypeOfGrade?)4
                    };
                    dbContext.Set<Opinion>().Add(komentarz);
                }
                dbContext.SaveChanges();
            }
        } //koniec treści opinii
    }
}
