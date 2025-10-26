using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class MigrationsTest : BaseClassTest
    {
        private string _connectionString;
        private DbContextOptions<InternalContext> _options;

        [SetUp]
        public void SetUp()
        {
            //_connectionString = "Server=192.168.2.29;Database=MchInternalTest;User Id=mchadmin;Password=admin;TrustServerCertificate=True;Encrypt=True;";

            //_options = new DbContextOptionsBuilder<InternalContext>()
            // .UseSqlServer(_connectionString)
            // .Options;

            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            context.Database.EnsureDeleted();
        }

        [Test]
        public void ApplyAllMigrations_DoesNotThrow()
        {
            // Verifica che ApplyMigrations non lanci eccezioni
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            Assert.DoesNotThrow(() => context.Database.Migrate());
        }
    }
}
