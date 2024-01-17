using System;
using System.Collections.Generic;

#nullable disable

namespace AoIS_course_work.Models
{
    public partial class Film
    {
        public Film()
        {
            FilmGenres = new HashSet<FilmGenre>();
        }

        public int FilmId { get; set; }
        public string OriginalName { get; set; }
        public string TranslatedName { get; set; }
        public short ReleaseYear { get; set; }
        public DateTime AddedDate { get; set; }
        public string Rating { get; set; }
        public string Duration { get; set; }
        public string Director { get; set; }

        public virtual ICollection<FilmGenre> FilmGenres { get; set; }
    }
}
