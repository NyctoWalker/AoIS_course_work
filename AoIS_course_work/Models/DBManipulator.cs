using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoIS_course_work.Models
{
    public class DBManipulator
    {
        public List<Film> GetDBValues()
        {
            List<Film> output = new();
            using (aoisContext db = new())
            {
                var films = db.Films.Include(f => f.FilmGenres).ThenInclude(fg => fg.IdGenreNavigation);
                foreach (Film f in films)
                {
                    output.Add(f);
                }
            }

            return output;
        }

        public void DeleteDBRecord(Film f)
        {
            using (aoisContext db = new())
            {
                db.Remove(f);
                db.SaveChanges();
            }
        }

        public void AddDBRecord(Film f, bool rewriteIfExists)
        {
            using (aoisContext db = new())
            {
                List<FilmGenre> filmGenres = new();

                // Добавление жанров в БД, если не существуют
                foreach (Genre g in f.FilmGenres.Select(fg => fg.IdGenreNavigation))
                {
                    FilmGenre filmGenre = new();
                    Genre existingGenre = db.Genres.FirstOrDefault(x => x.GenreName == g.GenreName);
                    if (existingGenre == null)
                    {
                        Genre newGenre = new Genre { GenreName = g.GenreName };
                        db.Genres.Add(newGenre);
                        db.SaveChanges();

                        newGenre = db.Genres.FirstOrDefault(x => x.GenreName == g.GenreName);
                        filmGenre = new FilmGenre { IdGenre = newGenre.GenreId };
                    }
                    else
                    {
                        filmGenre = new FilmGenre { IdGenre = existingGenre.GenreId };
                    }

                    filmGenres.Add(filmGenre);
                }

                f.FilmGenres.Clear();
                foreach (FilmGenre fg in filmGenres)
                { f.FilmGenres.Add(fg); }

                // Запись/перезапись фильмов
                var existingFilm = db.Films.FirstOrDefault(x => x.OriginalName == f.OriginalName);

                if (existingFilm != null)
                {
                    // Запись с таким же оригинальным названием существует и есть команда на перезапись
                    // Если команды на перезапись нет, ничего не происходит
                    if (rewriteIfExists)
                    {
                        existingFilm.TranslatedName = f.TranslatedName;
                        existingFilm.Director = f.Director;
                        existingFilm.Rating = f.Rating;
                        existingFilm.ReleaseYear = f.ReleaseYear;
                        existingFilm.Duration = f.Duration;
                        existingFilm.AddedDate = DateTime.Now;
                        existingFilm.FilmGenres = f.FilmGenres;
                        db.SaveChanges();
                    }
                }
                else
                {
                    // Схожей записи не существует
                    Film newFilm = new Film
                    {
                        OriginalName = f.OriginalName,
                        TranslatedName = f.TranslatedName,
                        Director = f.Director,
                        Rating = f.Rating,
                        ReleaseYear = f.ReleaseYear,
                        Duration = f.Duration,
                        FilmGenres = f.FilmGenres,
                    };
                    db.Add(newFilm);
                    db.SaveChanges();
                }
            }
        }

    }
}
