using Mch.StdCore.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Threading;

namespace Mch.Internal.UnifiedContextDb.Test
{
    /// <summary>
    /// Classe base da ereditare sulle classi di test per avere il servi provider ed il log
    /// _serviceProvider contiene la DependecyInjection
    /// _logger contiene un logger a console che se usata scrive sull'output del test, può essere utilizzato per specificare aclune funzionalità
    /// </summary>
    public class BaseClassTest
    {
        protected IServiceProvider _serviceProvider;
        protected ILogger _logger;
        protected FakeDataGenerate.FakeDataGenerate _fakeDataGenerate;
        protected InternalContext _context;

        [OneTimeSetUp]
        public void GlobalInit()
        {
            _serviceProvider = SetupTest.Setup();
            _fakeDataGenerate = new FakeDataGenerate.FakeDataGenerate(_serviceProvider);
            _logger = SetupTest.CreateLogger(_serviceProvider, GetType());
            _logger.LogTrace("Setup Test completed");

            if (!OnCreateNewDatabasesForEcthTest())
            {
                CreateNewDatabase();
            }
        }

        [SetUp]
        public void Setup()
        {
            if (OnCreateNewDatabasesForEcthTest())
            {
                CreateNewDatabase();
            }
            _context = _serviceProvider.GetRequiredService<InternalContext>();
        }

        /// <summary>
        /// Overload if need do not create ne database for ecth test defalt
        /// 
        /// </summary>
        /// <returns>Return True create a new database, retun False do not delete database</returns>
        protected virtual bool OnCreateNewDatabasesForEcthTest()
        {
            return true;
        }

        private void CreateNewDatabase()
        {
            // Remove and create database
            var ctx = _serviceProvider.GetRequiredService<InternalContext>();
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown()
        {
            _context?.Dispose();
        }


        [OneTimeTearDown]
        public void GlobalCleanup()
        {

        }

        protected void SetCulture(string? culture = null)
        {
            if (culture == null)
            {
                culture = "en-US";
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        protected void AssertPrimaryKey(Exception ex, string tableName, string keyIndexName)
        {
            Assert.Multiple(() =>
            {
                // Verifica che il messaggio di errore sia quello atteso
                Assert.That(ex.InnerException?.InnerException, Is.TypeOf<SqlException>());
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain("PRIMARY KEY"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"'dbo.{tableName}'"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"'{keyIndexName}'"));
            });
        }

        protected void AssertUniqueIndex(Exception ex, string tableName, string uniqueIndexName)
        {
            Assert.Multiple(() =>
            {
                // Verifica che il messaggio di errore sia quello atteso
                Assert.That(ex.InnerException?.InnerException, Is.TypeOf<SqlException>());
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"'dbo.{tableName}'"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"'{uniqueIndexName}'"));
            });
        }

        protected void AssertStringLength(Exception ex, string tableName, string columnName)
        {
            Assert.Multiple(() =>
            {
                // Verifica che il messaggio di errore sia quello atteso
                Assert.That(ex.InnerException?.InnerException, Is.TypeOf<SqlException>());
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"dbo.{tableName}"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"'{columnName}'"));
            });
        }

        protected void AssertCannotInsertNull(Exception ex, string tableName, string columnName)
        {
            Assert.Multiple(() =>
            {
                // Verifica che il messaggio di errore sia quello atteso
                Assert.That(ex.InnerException?.InnerException, Is.TypeOf<SqlException>());
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain("NULL"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain("INSERT"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"dbo.{tableName}"));
            });
        }

        protected void AssertForeignKeyIvalid(Exception ex, string foreignKeyName, string tableName, string columnName)
        {
            var connectionString = GetConnectionStringFromOptions();

            Assert.Multiple(() =>
            {
                // Verifica che il messaggio di errore sia quello atteso
                Assert.That(ex.InnerException?.InnerException, Is.TypeOf<SqlException>());
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain("INSERT"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain("FOREIGN KEY"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"\"{foreignKeyName}\""));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"\"{connectionString.InitialCatalog}\""));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"dbo.{tableName}"));
                Assert.That(ex.InnerException?.InnerException?.Message, Does.Contain($"'{columnName}'"));
            });
        }

        private ConnectionString? GetConnectionStringFromOptions()
        {
            var options = _serviceProvider.GetService<IOptions<ConnectionString>>();
            return options?.Value;
        }
    }
}
