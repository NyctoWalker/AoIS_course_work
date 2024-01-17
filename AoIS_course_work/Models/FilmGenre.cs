using System;
using System.Collections.Generic;

#nullable disable

namespace AoIS_course_work.Models
{
    public partial class FilmGenre
    {
        public int IdFilm { get; set; }
        public int IdGenre { get; set; }

        public virtual Film IdFilmNavigation { get; set; }
        public virtual Genre IdGenreNavigation { get; set; }
    }
}
