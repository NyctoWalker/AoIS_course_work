using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoIS_course_work.Models
{
    //dotnet ef dbcontext scaffold "server=localhost;port=3306;user=root;password=<password>;database=<dbname>" Pomelo.EntityFrameworkCore.MySql -o Models
    public class Parser
    {
        const string siteUrl = "https://www.imdb.com/title/tt";
        ChromeOptions options;

        public Parser()
        {
            options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            //options.AddArgument("log-level=3");
            options.AddArgument("--ignore-certificate-errors-spki-list");

        }

        public List<Film> Parse(List<string> relationalUrls)
        {
            List<Film> output = new();
            var driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();

            foreach (string url in relationalUrls)
            {
                driver.Navigate().GoToUrl(siteUrl + url);
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //js.ExecuteScript("window.scrollBy(0,document.body.scrollHeight)");
                //js.ExecuteScript("window.scrollBy(0,4700)", "");
                var storyline = driver.FindElement(By.XPath("//*[text()='Storyline']"));
                js.ExecuteScript("arguments[0].scrollIntoView();", storyline);
                System.Threading.Thread.Sleep(2000);

                var originalName = driver.FindElement(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[1]/div"));
                var translatedName = driver.FindElement(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[1]/h1/span"));
                var releaseYear = driver.FindElement(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[1]/ul/li[1]/a"));
                var rating = driver.FindElement(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[1]/ul/li[2]/a"));
                //*[@id="__next"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[1]/ul/li[2]/a
                var duration = driver.FindElement(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[1]/ul/li[3]"));
                var director = driver.FindElement(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[1]/section/div[2]/div/ul/li[1]/div/ul/li/a"));
                    
                short _releaseYear;
                if (!short.TryParse(releaseYear.Text, out _releaseYear))
                    continue;

                //проверить, есть ли жанры из перечня. если нет - добавить в БД

                var film = new Film
                {
                    OriginalName = originalName.Text.Replace("Original title: ", ""),
                    TranslatedName = translatedName.Text,
                    ReleaseYear = _releaseYear,
                    Rating = rating.Text,
                    Duration = duration.Text,
                    Director = director.Text,
                    FilmGenres = new List<FilmGenre>()
                };

                //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                //var genresLabel = driver.FindElement(By.XPath("//*[text()='Genres' or text()='Genre']"));
                //var parentOfGenresLabel = genresLabel.FindElement(By.XPath(".."));
                //*[@id="__next"]/main/div/section[1]/div/section/div/div[1]/section[7]/div[2]/ul[2]/li[2]/span
                var genres = driver.FindElements(By.XPath("//*[text()='Genres' or text()='Genre']//following-sibling::div/ul/li/a"));
                //var genres = driver.FindElements(By.XPath("//*[@id=\"__next\"]/main/div/section[1]/div/section/div/div[1]/section[7]/div[2]/ul[2]/li[2]/div/ul/li/a"));
                //*[@id="__next"]/main/div/section[1]/div/section/div/div[1]/section[6]/div[2]/ul[2]/li[2]/div/ul/li/a
                //*[@id="__next"]/main/div/section[1]/div/section/div/div[1]/section[7]/div[2]/ul[2]/li[2]/div/ul/li/a

                foreach (var genreElement in genres)
                {
                    try
                    {
                        var genreName = genreElement.Text;
                        var genre = new Genre { GenreName = genreName };
                        var filmGenre = new FilmGenre { IdFilmNavigation = film, IdGenreNavigation = genre };
                        if (filmGenre.IdGenreNavigation.GenreName is not null && filmGenre.IdGenreNavigation.GenreName !="")
                            film.FilmGenres.Add(filmGenre);
                    }
                    catch { continue; }

                }

                output.Add(film);
            }

            driver.Quit();
            return output;
        }

        /*var title = driver.FindElement(By.XPath("/html/body/div[2]/main/div[2]/div/form/div[2]/h1"));
                var stats = driver.FindElements(By.XPath("/html/body/div[2]/main/div[2]/div/form/div[8]/div[2]/div[2]/div")).Take(10);
                var price = driver.FindElement(By.ClassName("product__price-cur"));

                string priceNumber = Regex.Replace(price.Text, @"\D", "");

                foreach (var stat in stats)
                {
                    var name = stat.FindElement(By.ClassName("product__property-name")).Text;
                    var value = stat.FindElement(By.ClassName("product__property-value")).Text;

                }*/
    }
}
