using FluentAssertions;
using Mch.Internal.UnifiedContextDb.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class OdlTest : BaseClassTest
    {
        // Eredito da una classe base dove è già presente il SetUp e alcune variabili di classe

        //private IServiceProvider _serviceProvider;
        //private ILogger _logger;
        ////private DbContextOptions<InternalContext> _options;
        ////private Mock<ILoggerFactory> _mockLoggerFactory;
        ////private string _connectionString;

        //[SetUp]
        //public void Setup()
        //{
        //    _serviceProvider = SetupTest.Setup();
        //    _logger = SetupTest.CreateLogger(_serviceProvider, "OdlTest");

        //    //_connectionString = "Server=192.168.2.29;Database=MchInternalTest;User Id=mchadmin;Password=admin;TrustServerCertificate=True;Encrypt=True;";

        //    //_options = new DbContextOptionsBuilder<InternalContext>()
        //    // .UseSqlServer(_connectionString)
        //    // .Options;

        //    //_mockLoggerFactory = new Mock<ILoggerFactory>();

        //    //using var context = new InternalContext(_options);
        //    //context.Database.EnsureDeleted();
        //    //context.Database.EnsureCreated();
        //}

        [Test]
        public async Task Add_Order_ShouldAddAsync()
        {
            //using var context = new InternalContext(_options);
            // Leggo il context facendolo ritornare dalla DependencyInjection
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            var articleType = new ArticleType()
            {
                Code = "MATERIALI",
                Description = "MATERIALI"
            };

            context.ArticleTypes.Add(articleType);
            await context.SaveChangesAsync();

            var article = new Article()
            {
                Code = "EA11706M03702",
                Description = "PIASTRA INFERIORE PER GRUPPI SOLLEVAMENTO",
                ArticleTypeId = articleType.ArticleTypeId
            };

            var cnc = new CNC()
            {
                Code = "EmcoMill",
                Description = "EmcoMill"
            };

            var comm = new Commission()
            {
                Code = "5382U001",
                Description = "5382U001 Test",
                Status = Models.Enums.RecordStatus.Active,
                Type = Models.Enums.CommissionType.Planning
            };

            context.Articles.Add(article);
            context.CNCs.Add(cnc);
            context.Commissions.Add(comm);
            await context.SaveChangesAsync();

            var orderNr = "100";

            var order = new Order()
            {
                Priority = 1,
                OrderStatus = Models.Enums.OrderStatus.Working,
                ArticleId = article.ArticleId,
                OrderNr = orderNr,
                Quantity = 50,
                CommissionId = comm.CommissionId,
                PickingLabel = "M050",
                CreatedAt = DateTime.Now,
                RequestedDeliveryAt = new DateTime(2025, 05, 11),
                CNC_Id = cnc.CNC_Id,
                CNC_Order = new CNC_Order()
                {
                    RealProgrammingTime = TimeSpan.FromMinutes(18),
                    RealSetupTime = TimeSpan.FromMinutes(40),
                    RealProductionTime = TimeSpan.FromMinutes(13)                    
                }
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            var savedOrder = context.Orders.FirstOrDefault();
            savedOrder.Should().NotBeNull();
            savedOrder.OrderNr.Should().Be(orderNr);
        }
    }
}