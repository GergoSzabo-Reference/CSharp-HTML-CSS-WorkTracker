using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Autoszerelo_Shared
{
    public class Munka
    {
        public int MunkaId { get; set; }

        public int UgyfelId { get; set; }

        [Required(ErrorMessage = "A rendszám kötelező.")]
        [RegularExpression(@"[A-Z][A-Z][A-Z]-\d\d\d", ErrorMessage = "A rendszám formátuma érvénytelen (pl. ABC-123).")]
        public string Rendszam { get; set; }

        [Required(ErrorMessage = "A gyártási év kötelező.")]
        [Range(1900, 2200, ErrorMessage = "A gyártási év 1900 és 2200 között kell, hogy legyen.")]
        public int GyartasiEv { get; set; }

        [Required(ErrorMessage = "A munka kategóriája kötelező.")]
        public MunkaKategoria Kategoria { get; set; }

        [Required(ErrorMessage = "A hiba leírása kötelező.")]
        [StringLength(1000, ErrorMessage = "A hiba leírása legfeljebb 1000 karakter hosszú lehet.")]
        public string HibaLeiras { get; set; }

        [Required(ErrorMessage = "A hiba súlyossága kötelező.")]
        [Range(1, 10, ErrorMessage = "A hiba súlyossága 1 és 10 közötti érték lehet.")]
        public int HibaSulyossaga { get; set; }

        [Required(ErrorMessage = "A munka állapota kötelező.")]
        public MunkaAllapot Allapot { get; set; }
    }
}
