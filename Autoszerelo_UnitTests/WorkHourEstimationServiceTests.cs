using Autoszerelo_Shared;
using Autoszerelo_API.Services;
using Xunit;
using System;

namespace Autoszerelo_UnitTests
{
    public class WorkHourEstimationServiceTests
    {

        private readonly WorkHourEstimationService _service;

        public WorkHourEstimationServiceTests()
        {
            _service = new WorkHourEstimationService();
        }

        [Fact] //test case
        public void CalculateEstimatedHours_ValidInput_CorrectEstimation()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Motor,       // 8 óra
                GyartasiEv = DateTime.Now.Year - 7,     // 7 éves autó: 1.0 
                HibaSulyossaga = 5                      // Súlyosság 5: 0.6 
            };

            double expectedHours = 4.8; // 8 * 1.0 * 0.6;

            double actualHours = _service.CalculateEstimatedHours(munka);

            Assert.Equal(expectedHours, actualHours, precision: 2); //till what precision should it compare
        }

        [Fact]
        public void CalculateEstimatedHours_Karosszeria_3yrs_Severity8_CorrectEstimation()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Karosszeria,
                GyartasiEv = DateTime.Now.Year - 7,
                HibaSulyossaga = 8
            };

            double expectedHours = 2.4; // 3* 1.0*0.8
            double actualHours = _service.CalculateEstimatedHours(munka);

            Assert.Equal(expectedHours, actualHours, precision: 2);
        }

        [Fact]
        public void CalculateEstimatedHours_Fekberendezes_15yrs_Sevirity2_CorrectEstimation()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Fekberendezes,
                GyartasiEv = DateTime.Now.Year - 15,
                HibaSulyossaga = 2
            };

            double expectedHours = 1.2; // 4 * 1.5 * 0.2 = 1.2
            // Act
            double actualHours = _service.CalculateEstimatedHours(munka);

            Assert.Equal(expectedHours, actualHours, precision: 2);
        }

        [Fact]
        public void CalculateEstimatedHours_Futomu_2yrs_Severity10()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Futomu, // 6 óra
                GyartasiEv = DateTime.Now.Year - 2, // 0.5
                HibaSulyossaga = 10 // Súlyosság 10: 1.0
            };
            double expectedHours = 3; // 6 * 0.5 * 1.0
            double actualHours = _service.CalculateEstimatedHours(munka);
            Assert.Equal(expectedHours, actualHours, precision: 2);
        }

        [Fact]
        public void CalculateEstimatedHours_Motor_10yrs_Severity4()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Motor,             // 8 óra
                GyartasiEv = DateTime.Now.Year - 10,            // 10 éves: 1.0
                HibaSulyossaga = 4                             // Súlyosság 4: 0.4
            };
            double expectedHours = 3.2; // 8 * 1.0 * 0.4
            double actualHours = _service.CalculateEstimatedHours(munka);
            Assert.Equal(expectedHours, actualHours, precision: 2);
        }

        [Fact]
        public void CalculateEstimatedHours_Motor_UjAuto_LowSeverity()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Motor,
                GyartasiEv = DateTime.Now.Year,
                HibaSulyossaga = 1
            };
            double expectedHours = 0.8; // 8 * 0.5 * 0.2
            double actualHours = _service.CalculateEstimatedHours(munka);
            Assert.Equal(expectedHours, actualHours, precision: 2);
        }


        [Fact]
        public void CalculateEstimatedHours_Karosszeria_OldCar_HighSeverity()
        {
            var munka = new Munka
            {
                Kategoria = MunkaKategoria.Karosszeria,
                GyartasiEv = DateTime.Now.Year - 20,
                HibaSulyossaga = 9
            };
            double expectedHours = 3.6; // 3 * 1.5 * 0.8 (9-es súlyosságnál a felső határt vesszük)
            double actualHours = _service.CalculateEstimatedHours(munka);
            Assert.Equal(expectedHours, actualHours, precision: 2);
        }
    }
}
