using FluentAssertions;
using Mch.Internal.UnifiedContextDb.Models;
using Mch.StdCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class InternalContextTest : BaseClassTest
    {
        private DbContextOptions<InternalContext> _options;
        private Mock<ILoggerFactory> _mockLoggerFactory;
        private string _connectionString;

        [SetUp]
        public void Setup()
        {
            _connectionString = _serviceProvider.GetService<IOptions<ConnectionString>>()?.Value?.ToString();

            _options = new DbContextOptionsBuilder<InternalContext>()
             .UseSqlServer(_connectionString)
             .Options;

            _mockLoggerFactory = new Mock<ILoggerFactory>();

            //using var context = new InternalContext(_options);
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
        }

        [Test]
        public void Constructor_WithOptions_CreatesInstance()
        {
            using var context = new InternalContext(_options);

            context.Should().NotBeNull();
        }

        [Test]
        public void Constructor_WithConnectionString_CreatesInstance()
        {
            using var context = new InternalContext(_connectionString, _mockLoggerFactory.Object);

            context.Should().NotBeNull();
        }

        [Test]
        public void Constructor_WithConnectionStringAndReadOnly_CreatesInstance()
        {
            using var context = new InternalContext(_connectionString, _mockLoggerFactory.Object, true);

            context.Should().NotBeNull();
        }

        [Test]
        public void DbSets_ShouldBeInitialized()
        {
            using var context = new InternalContext(_options);

            context.Articles.Should().NotBeNull();
            context.ArticleTypes.Should().NotBeNull();
            context.CNCs.Should().NotBeNull();
            context.CNC_Orders.Should().NotBeNull();
            context.Commissions.Should().NotBeNull();
            context.Orders.Should().NotBeNull();
            context.OrderLogs.Should().NotBeNull();

            context.Groups.Should().NotBeNull();
            context.Menus.Should().NotBeNull();
            context.Roles.Should().NotBeNull();
            context.RoleGroups.Should().NotBeNull();
            context.Users.Should().NotBeNull();
            context.UserRoles.Should().NotBeNull();
            context.CustomerSuppliers.Should().NotBeNull();
            context.CustomerSupplierAddresses.Should().NotBeNull();
            context.MenuRoles.Should().NotBeNull();
        }

        [Test]
        public void Add_Commission_ShouldAddToContext()
        {
            using var context = new InternalContext(_options);

            var comm = new Commission()
            {
                Code = "TestCommission",
                Description = "Test Commission Description",
                Ref = "InternalContextTests",
                Status = Models.Enums.RecordStatus.Active,
                Type = Models.Enums.CommissionType.Planning
            };

            context.Commissions.Add(comm);
            context.SaveChanges();

            var savedSite = context.Commissions.FirstOrDefault();
            savedSite.Should().NotBeNull();
            savedSite.Code.Should().Be("TestCommission");
        }

        [Test]
        public void Query_Articles_ShouldReturnEmptyListWhenNoData()
        {
            // Arrange
            using var context = new InternalContext(_options);

            // Act
            var articles = context.Articles.ToList();

            // Assert
            articles.Should().NotBeNull();
            articles.Should().BeEmpty();
        }
    }
}
