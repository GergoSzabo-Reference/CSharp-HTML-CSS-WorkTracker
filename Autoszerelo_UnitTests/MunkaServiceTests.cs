using Autoszerelo.Shared;
using Autoszerelo_API.Data;
using Autoszerelo_API.Interfaces;
using Autoszerelo_API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System;
using Autoszerelo_Shared;

namespace Autoszerelo.API.Tests
{
    public class MunkaServiceTests
    {
        private AutoszereloDbContext GetInMemoryDbContext() //always fresh db
        {
            var options = new DbContextOptionsBuilder<AutoszereloDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) //random name
                .Options;
            var dbContext = new AutoszereloDbContext(options);
            // dbContext.Database.EnsureCreated(); -> we will fill it
            return dbContext;
        }

        private readonly WorkHourEstimationService _estimationService;
        private Mock<IUgyfelService> _mockUgyfelService; // Mock for IUgyfelService-hez

        public MunkaServiceTests()
        {
            _estimationService = new WorkHourEstimationService();
            _mockUgyfelService = new Mock<IUgyfelService>();
        }

        // --- GET MUNKAK ASYNC TESTS ---
        [Fact]
        public async Task GetMunkakAsync_UresAdatbazissal_UresListatAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var result = await munkaService.GetMunkakAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMunkakAsync_Adatokkal_VisszaadjaAMunkakatBecsultOraval()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munka1 = new Munka { MunkaId = 1, UgyfelId = 1, Rendszam = "AAA-111", GyartasiEv = DateTime.Now.Year - 2, Kategoria = MunkaKategoria.Motor, HibaSulyossaga = 5, HibaLeiras = "hiba1" };
            var munka2 = new Munka { MunkaId = 2, UgyfelId = 2, Rendszam = "BBB-222", GyartasiEv = DateTime.Now.Year - 7, Kategoria = MunkaKategoria.Karosszeria, HibaSulyossaga = 8, HibaLeiras = "hiba2" };
            dbContext.Munkak.AddRange(munka1, munka2);
            await dbContext.SaveChangesAsync();

            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var result = (await munkaService.GetMunkakAsync()).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.True(result.First(m => m.MunkaId == 1).BecsultMunkaorak > 0);
            Assert.True(result.First(m => m.MunkaId == 2).BecsultMunkaorak > 0);
        }

        // --- GET MUNKA BY ID ASYNC TESTS ---
        [Fact]
        public async Task GetMunkaByIdAsync_LetezoMunka_VisszaadjaBecsultOraval()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munka = new Munka { MunkaId = 1, UgyfelId = 1, Rendszam = "AAA-111", GyartasiEv = DateTime.Now.Year - 2, Kategoria = MunkaKategoria.Motor, HibaSulyossaga = 5, HibaLeiras = "teszt" };
            dbContext.Munkak.Add(munka);
            await dbContext.SaveChangesAsync();

            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var result = await munkaService.GetMunkaByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MunkaId);
            Assert.True(result.BecsultMunkaorak > 0);
        }

        [Fact]
        public async Task GetMunkaByIdAsync_NemLetezoMunka_NulltAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var result = await munkaService.GetMunkaByIdAsync(99);

            // Assert
            Assert.Null(result);
        }


        // --- UPDATE MUNKA ASYNC TESTS ---
        [Fact]
        public async Task UpdateMunkaAsync_LetezoMunka_ValidUgyfelId_ModositjaEsTrueAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaKezdeti = new Munka { MunkaId = 1, UgyfelId = 1, Rendszam = "AAA-111", GyartasiEv = 2020, Kategoria = MunkaKategoria.Motor, HibaSulyossaga = 5, HibaLeiras = "Kezdeti" };
            dbContext.Munkak.Add(munkaKezdeti);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(munkaKezdeti).State = EntityState.Detached; // Detach, so the EF can follow the new instance

            var munkaModositott = new Munka
            {
                MunkaId = 1,
                UgyfelId = 1,
                Rendszam = "AAA-111",
                GyartasiEv = 2020,
                Kategoria = MunkaKategoria.Motor,
                HibaSulyossaga = 7,
                HibaLeiras = "Módosított leírás",
                Allapot = MunkaAllapot.ElvegzesAlatt
            };

            // Mock UgyfelService: GetUgyfelByIdAsync return an existing client
            _mockUgyfelService.Setup(s => s.GetUgyfelByIdAsync(1)).ReturnsAsync(new Ugyfel { UgyfelId = 1 });

            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var success = await munkaService.UpdateMunkaAsync(1, munkaModositott);

            // Assert
            Assert.True(success);
            var frissitettMunka = await dbContext.Munkak.FindAsync(1);
            Assert.NotNull(frissitettMunka);
            Assert.Equal("Módosított leírás", frissitettMunka.HibaLeiras);
            Assert.Equal(MunkaAllapot.ElvegzesAlatt, frissitettMunka.Allapot);
            _mockUgyfelService.Verify(s => s.GetUgyfelByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task UpdateMunkaAsync_LetezoMunka_InvalidUgyfelId_ArgumentExceptiontDob()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaKezdeti = new Munka { MunkaId = 1, UgyfelId = 1, Rendszam = "AAA-111", GyartasiEv = 2020, Kategoria = MunkaKategoria.Motor, HibaSulyossaga = 5, HibaLeiras = "Kezdeti" };
            dbContext.Munkak.Add(munkaKezdeti);
            await dbContext.SaveChangesAsync();
            dbContext.Entry(munkaKezdeti).State = EntityState.Detached;

            var munkaModositott = new Munka { MunkaId = 1, UgyfelId = 99, Rendszam = "AAA-111", HibaLeiras = "Módosítani próbált" }; // Nem létező UgyfelId

            _mockUgyfelService.Setup(s => s.GetUgyfelByIdAsync(99)).ReturnsAsync((Ugyfel?)null);

            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => munkaService.UpdateMunkaAsync(1, munkaModositott));
            _mockUgyfelService.Verify(s => s.GetUgyfelByIdAsync(99), Times.Once);
        }

        [Fact]
        public async Task UpdateMunkaAsync_IdMismatch_FalseAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaModositott = new Munka { MunkaId = 1, UgyfelId = 1, Rendszam = "AAA-111", HibaLeiras = "Valami" };
            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            // The path ID (2) and body ID (1) are not eq
            var success = await munkaService.UpdateMunkaAsync(2, munkaModositott);

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task UpdateMunkaAsync_NemLetezoMunka_FalseAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaModositott = new Munka { MunkaId = 99, UgyfelId = 1, Rendszam = "AAA-111", HibaLeiras = "Nem létező" };
            _mockUgyfelService.Setup(s => s.GetUgyfelByIdAsync(1)).ReturnsAsync(new Ugyfel { UgyfelId = 1 });
            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var success = await munkaService.UpdateMunkaAsync(99, munkaModositott);

            // Assert
            Assert.False(success);
        }

        // --- DELETE MUNKA ASYNC TESTS ---
        [Fact]
        public async Task DeleteMunkaAsync_LetezoMunka_TorliEsTrueAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaTorlendo = new Munka { MunkaId = 1, UgyfelId = 1, Rendszam = "TOR-LENDO", GyartasiEv = 2020, Kategoria = MunkaKategoria.Futomu, HibaSulyossaga = 3, HibaLeiras = "Ezt törölni kell" };
            dbContext.Munkak.Add(munkaTorlendo);
            await dbContext.SaveChangesAsync();

            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var success = await munkaService.DeleteMunkaAsync(1);

            // Assert
            Assert.True(success);
            var toroltMunka = await dbContext.Munkak.FindAsync(1);
            Assert.Null(toroltMunka);
        }

        [Fact]
        public async Task DeleteMunkaAsync_NemLetezoMunka_FalseAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var munkaService = new MunkaService(dbContext, _estimationService, _mockUgyfelService.Object);

            // Act
            var success = await munkaService.DeleteMunkaAsync(99);

            // Assert
            Assert.False(success);
        }
    }
}