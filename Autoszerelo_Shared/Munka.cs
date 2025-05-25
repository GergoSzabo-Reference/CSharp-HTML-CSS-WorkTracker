using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoszerelo_Shared
{
    public class Munka
    {
        public int MunkaId { get; set; }
        public int UgyfelId { get; set; }
        public string Rendszam { get; set; }
        public int GyartasiEv { get; set; }
        public MunkaKategoria Kategoria { get; set; }
        public string HibaLeiras { get; set; }
        public int HibaSulyossaga { get; set; }
        public MunkaAllapot Allapot { get; set; }
    }
}
