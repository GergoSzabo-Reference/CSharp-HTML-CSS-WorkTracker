using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Autoszerelo.Shared
{
    public class Ugyfel
    {
        public int UgyfelId { get; set; }

        [Required(ErrorMessage = "Az ügyfél neve kötelező.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "A névnek legalább 2 és legfeljebb 100 karakter hosszúnak kell lennie.")]
        public string Nev { get; set; }

        [Required(ErrorMessage = "A lakcím kötelező.")]
        [StringLength(200, ErrorMessage = "A lakcím legfeljebb 200 karakter hosszú lehet.")]
        public string Lakcim { get; set; }

        [Required(ErrorMessage = "Az email cím kötelező.")]
        [EmailAddress(ErrorMessage = "Érvénytelen email cím formátum.")]
        [StringLength(100, ErrorMessage = "Az email cím legfeljebb 100 karakter hosszú lehet.")]
        public string Email { get; set; }
    }
}
