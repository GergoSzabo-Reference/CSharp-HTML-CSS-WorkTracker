using Autoszerelo.Shared;
using Autoszerelo_API.Data;
using Autoszerelo_API.Interfaces;
using Autoszerelo_API.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace Autoszerelo.API.Tests
{
    public class UgyfelServiceTests
    {
        private AutoszereloDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AutoszereloDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var dbContext = new AutoszereloDbContext(options);
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        //for UgyfelService's 2. parameter
        private readonly WorkHourEstimationService _estimationServiceInstance = new WorkHourEstimationService();

        [Fact]
        public async Task GetUgyfelekAsync_UresAdatbazissal_UresListatAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);

            // Act
            var result = await ugyfelService.GetUgyfelekAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUgyfelekAsync_Adatokkal_VisszaadjaAzUgyfeleket()
        {
            var dbContext = GetInMemoryDbContext();
            dbContext.Ugyfelek.AddRange(
                new Ugyfel { UgyfelId = 1, Nev = "Teszt Elek 1", Email = "teszt1@example.com", Lakcim = "Cím 1" },
                new Ugyfel { UgyfelId = 2, Nev = "Teszt Anna 2", Email = "teszt2@example.com", Lakcim = "Cím 2" }
            );
            await dbContext.SaveChangesAsync();

            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);

            var result = await ugyfelService.GetUgyfelekAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUgyfelByIdAsync_LetezoUgyfel_VisszaadjaAzUgyfelet()
        {
            var dbContext = GetInMemoryDbContext();
            var varANDOgyfel = new Ugyfel { UgyfelId = 1, Nev = "Keresett Elek", Email = "keresett@example.com", Lakcim = "Keresett cím" };
            dbContext.Ugyfelek.Add(varANDOgyfel);
            await dbContext.SaveChangesAsync();

            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);

            var result = await ugyfelService.GetUgyfelByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Keresett Elek", result.Nev);
        }

        [Fact]
        public async Task GetUgyfelByIdAsync_NemLetezoUgyfel_NulltAdVissza()
        {
            var dbContext = GetInMemoryDbContext();
            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);

            var result = await ugyfelService.GetUgyfelByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUgyfelAsync_UjUgyfel_HozzaadjaAzAdatbazishozEsVisszaadjaIdVal()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);
            var ujUgyfel = new Ugyfel
            {
                Nev = "Új Ügyfél Kft.",
                Email = "uj@example.com",
                Lakcim = "Új cím 123"
            };

            // Act
            var createdUgyfel = await ugyfelService.CreateUgyfelAsync(ujUgyfel);

            // Assert
            Assert.NotNull(createdUgyfel);
            Assert.True(createdUgyfel.UgyfelId > 0);
            Assert.Equal("Új Ügyfél Kft.", createdUgyfel.Nev);

            var ugyfelAzAdatbazisban = await dbContext.Ugyfelek.FindAsync(createdUgyfel.UgyfelId);
            Assert.NotNull(ugyfelAzAdatbazisban);
            Assert.Equal("Új Ügyfél Kft.", ugyfelAzAdatbazisban.Nev);
        }

        [Fact]
        public async Task UpdateUgyfelAsync_LetezoUgyfel_ModositjaAzAdatokatEsTrueAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var kezdetiUgyfel = new Ugyfel { UgyfelId = 1, Nev = "Régi Név", Email = "regi@example.com", Lakcim = "Régi cím" };
            dbContext.Ugyfelek.Add(kezdetiUgyfel);
            await dbContext.SaveChangesAsync();

            //detach, so Update wont collide in the context
            dbContext.Entry(kezdetiUgyfel).State = EntityState.Detached;


            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);
            var modositottUgyfel = new Ugyfel
            {
                UgyfelId = 1,
                Nev = "Módosított Név",
                Email = "modositott@example.com",
                Lakcim = "Módosított cím"
            };

            // Act
            var success = await ugyfelService.UpdateUgyfelAsync(1, modositottUgyfel);

            // Assert
            Assert.True(success);
            var frissitettUgyfel = await dbContext.Ugyfelek.FindAsync(1);
            Assert.NotNull(frissitettUgyfel);
            Assert.Equal("Módosított Név", frissitettUgyfel.Nev);
        }

        [Fact]
        public async Task UpdateUgyfelAsync_NemLetezoUgyfel_FalseAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);
            var nemLetezoModositottUgyfel = new Ugyfel
            {
                UgyfelId = 99, // non-existent
                Nev = "Nem Létező Név",
                Email = "nemletezo@example.com",
                Lakcim = "Nem létező cím"
            };

            // Act
            var success = await ugyfelService.UpdateUgyfelAsync(99, nemLetezoModositottUgyfel);

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task UpdateUgyfelAsync_IdMismatch_FalseAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);
            var ugyfel = new Ugyfel { UgyfelId = 1, Nev = "Valaki", Email = "email@email.com", Lakcim = "Lakcím" };

            // Act
            var success = await ugyfelService.UpdateUgyfelAsync(2, ugyfel);

            // Assert
            Assert.False(success);
        }


        [Fact]
        public async Task DeleteUgyfelAsync_LetezoUgyfel_TorliEsTrueAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var torlendoUgyfel = new Ugyfel { UgyfelId = 1, Nev = "Törlendő Elek", Email = "torlendo@example.com", Lakcim = "Törlendő cím" };
            dbContext.Ugyfelek.Add(torlendoUgyfel);
            await dbContext.SaveChangesAsync();

            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);

            // Act
            var success = await ugyfelService.DeleteUgyfelAsync(1);

            // Assert
            Assert.True(success);
            var toroltUgyfel = await dbContext.Ugyfelek.FindAsync(1);
            Assert.Null(toroltUgyfel);
        }

        [Fact]
        public async Task DeleteUgyfelAsync_NemLetezoUgyfel_FalseAdVissza()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var ugyfelService = new UgyfelService(dbContext, _estimationServiceInstance);

            // Act
            var success = await ugyfelService.DeleteUgyfelAsync(99);

            // Assert
            Assert.False(success);
        }
    }
}