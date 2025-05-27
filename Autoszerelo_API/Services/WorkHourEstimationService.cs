using Autoszerelo_Shared;

namespace Autoszerelo_API.Services
{
    public class WorkHourEstimationService
    {
        public Double CalculateEstimatedHours(Munka munka)
        {
            if(munka == null)
            {
                throw new ArgumentException(nameof(munka));
            }

            double category = 0;

            switch (munka.Kategoria)
            {
                case MunkaKategoria.Karosszeria:
                    category = 3;
                    break;
                case MunkaKategoria.Motor:
                    category = 8;
                    break;
                case MunkaKategoria.Futomu:
                    category = 6;
                    break;
                case MunkaKategoria.Fekberendezes:
                    category = 4;
                    break;
                default:
                    category = 0;
                    break;
            }

            double age = 0;
            int carAge = DateTime.Now.Year - munka.GyartasiEv;

            if (carAge >= 0 && carAge <= 5)
            {
                age = 0.5;
            }
            else if (carAge > 5 && carAge <= 10)
            {
                age = 1.0;
            }
            else if (carAge > 10 && carAge <= 20)
            {
                age = 1.5;
            }
            else
            {
                age = 2.0;
            }
            if (carAge < 0)
                age = 0.5;

            double severity = 0;

            if (munka.HibaSulyossaga >= 1 && munka.HibaSulyossaga <= 2)
            {
                severity = 0.2;
            }
            else if (munka.HibaSulyossaga >= 3 && munka.HibaSulyossaga <= 4)
            {
                severity = 0.4;
            }
            else if (munka.HibaSulyossaga >= 5 && munka.HibaSulyossaga <= 7)
            {
                severity = 0.6;
            }
            else if (munka.HibaSulyossaga >= 8 && munka.HibaSulyossaga <= 9)
            {
                severity = 0.8;
            }
            else if (munka.HibaSulyossaga == 10)
            {
                severity = 1.0;
            }
            else
            {
                severity = 0;
            }

            /*if (munka.HibaSulyossaga > 0)
            {
                severity = 1 / (munka.HibaSulyossaga % 2 == 0 ?
                    munka.HibaSulyossaga :
                    munka.HibaSulyossaga == 7 ? munka.HibaSulyossaga-1 : munka.HibaSulyossaga+1);
            }
            else
            {
                severity = 0;
            }*/

            return category * age * severity;
        }
    }
}
