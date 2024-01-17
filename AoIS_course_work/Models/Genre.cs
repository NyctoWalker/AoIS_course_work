using System;
using System.Collections.Generic;

#nullable disable

namespace AoIS_course_work.Models
{
    public partial class Genre
    {
        public Genre()
        {
            FilmGenres = new HashSet<FilmGenre>();
        }

        public int GenreId { get; set; }
        public string GenreName { get; set; }

        public virtual ICollection<FilmGenre> FilmGenres { get; set; }
    }
}
