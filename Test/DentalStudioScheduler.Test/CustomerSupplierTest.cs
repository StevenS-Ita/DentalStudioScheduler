using FluentAssertions;
using Mch.Internal.UnifiedContextDb.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class CustomerSupplierTest : BaseClassTest
    {
        //private DbContextOptions<InternalContext> _options;
        //private Mock<ILoggerFactory> _mockLoggerFactory;
        //private string _connectionString;

        //[SetUp]
        //public void Setup()
        //{
        //    _connectionString = "Server=192.168.2.29;Database=MchInternalTest;User Id=mchadmin;Password=admin;TrustServerCertificate=True;Encrypt=True;";

        //    _options = new DbContextOptionsBuilder<InternalContext>()
        //     .UseSqlServer(_connectionString)
        //     .Options;

        //    _mockLoggerFactory = new Mock<ILoggerFactory>();

        //    using var context = new InternalContext(_options);
        //    context.Database.EnsureDeleted();
        //    context.Database.EnsureCreated();
        //}

        [Test, Category("Insert one CustomerSupplier and one CustomerSupplierAddress - true")]
        public async Task Add_CustomerSupplier_CustomerSupplierAddress_ShouldAddAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var customerSupplier = DatabaseHelper.InsertCustomerSupplier(context);
                DatabaseHelper.InsertCustomerSupplierAddress(context, customerSupplier.CustomerSupplierId);
            }

            var _customerSupplier = context.CustomerSuppliers.Include(x => x.CustomerSupplierAddresses).FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(_customerSupplier.CustomerSupplierAddresses.Count, 1);
                Assert.IsNotNull(_customerSupplier.CustomerSupplierAddresses.Where(x => x.DefaultAddress == AddressDefault.IsDefaultAddress).FirstOrDefault());
            });
        }

        [Test, Category("Insert one CustomerSupplier and two CustomerSupplierAddress - true")]
        public async Task Add_CustomerSupplier_MultyCustomerSupplierAddress_ShouldAddAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var customerSupplier = DatabaseHelper.InsertCustomerSupplier(context);
                DatabaseHelper.InsertMultyCustomerSupplierAddress(context, customerSupplier.CustomerSupplierId);
            }

            var _customerSupplier = context.CustomerSuppliers.Include(x => x.CustomerSupplierAddresses).FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(_customerSupplier.CustomerSupplierAddresses.Count, 2);
                Assert.IsNotNull(_customerSupplier.CustomerSupplierAddresses.Where(x => x.DefaultAddress == AddressDefault.IsDefaultAddress).FirstOrDefault());
            });
        }

        [Test, Category("Insert one CustomerSupplier and one CustomerSupplierAddress Not Default Address - true")]
        public async Task Add_CustomerSupplier_CustomerSupplierAddress_NotDefaultAddress_ShouldAddAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var customerSupplier = DatabaseHelper.InsertCustomerSupplier(context);
                DatabaseHelper.InsertCustomerSupplierAddress(context, customerSupplier.CustomerSupplierId, defaultAddress: AddressDefault.NotDefaultAddress);
            }

            var _customerSupplier = context.CustomerSuppliers.Include(x => x.CustomerSupplierAddresses).FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(_customerSupplier.CustomerSupplierAddresses.Count, 1);
                Assert.IsNull(_customerSupplier.CustomerSupplierAddresses.Where(x => x.DefaultAddress == AddressDefault.IsDefaultAddress).FirstOrDefault());
            });
        }

        [Test, Category("Insert one CustomerSupplier and two CustomerSupplierAddress Not Default Address - true")]
        public async Task Add_CustomerSupplier_MultyCustomerSupplierAddress_NotDefaultAddress_ShouldAddAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var customerSupplier = DatabaseHelper.InsertCustomerSupplier(context);
                DatabaseHelper.InsertMultyCustomerSupplierAddress(context, customerSupplier.CustomerSupplierId, defaultAddress1: AddressDefault.NotDefaultAddress, defaultAddress2: AddressDefault.NotDefaultAddress);
            }

            var _customerSupplier = context.CustomerSuppliers.Include(x => x.CustomerSupplierAddresses).FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(_customerSupplier.CustomerSupplierAddresses.Count, 2);
                Assert.IsNull(_customerSupplier.CustomerSupplierAddresses.Where(x => x.DefaultAddress == AddressDefault.IsDefaultAddress).FirstOrDefault());
            });
        }

        [Test, Category("Insert one CustomerSupplier - true")]
        public async Task Add_CustomerSupplier_NotCustomerSupplierAddress_ShouldAddAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var customerSupplier = DatabaseHelper.InsertCustomerSupplier(context);
            }

            var _customerSupplier = context.CustomerSuppliers.Include(x => x.CustomerSupplierAddresses).FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(_customerSupplier.CustomerSupplierAddresses.Count, 0);
                Assert.IsNull(_customerSupplier.CustomerSupplierAddresses.Where(x => x.DefaultAddress == AddressDefault.IsDefaultAddress).FirstOrDefault());
            });
        }
    }
}