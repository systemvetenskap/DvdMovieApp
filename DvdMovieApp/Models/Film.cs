using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvdMovieApp.Models
{
    public class Film
    {
        /// <summary>
        /// primary key
        /// </summary>
        public int Film_id { get; set; }

        /// <summary>
        /// Movie title
        /// </summary>
        public string Title { get; set; }
    }
}
