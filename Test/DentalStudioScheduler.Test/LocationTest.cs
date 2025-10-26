using FluentAssertions;
using Mch.Internal.UnifiedContextDb.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class LocationTest : BaseClassTest
    {
        [Test, Category("Should be Initialized")]
        public void DbSets_ShouldBeInitialized()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();

            context.Sites.Should().NotBeNull();
            context.Warehouses.Should().NotBeNull();
            context.Areas.Should().NotBeNull();
            context.Locations.Should().NotBeNull();
            context.EquipmentMovements.Should().NotBeNull();

            context.StorageUnits.Should().NotBeNull();

            context.AreaTypes.Should().NotBeNull();
            context.LocationTypes.Should().NotBeNull();
            context.StorageUnitTypes.Should().NotBeNull();
        }

        #region Insert
        [Test]
        [Category("Site")]
        [TestCase(1)]
        [TestCase(10)]
        public async Task InsertSite_ValidSite_Should_Succeed(int numToInsert)
        {
            var sites = _fakeDataGenerate.GenerateSites(numToInsert);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(sites.Count, numToInsert);
                Assert.AreEqual(sites.Select(x => x.SiteId).Distinct().Count(), numToInsert);
            });
        }

        [Test]
        [Category("Site-Warehouse")]
        [TestCase(3, 5)]
        public async Task InsertSitesAndWarehouses_MultySitesAndMultyWarehouses_Should_Succeed(int countSite, int countWare)
        {
            var sites = _fakeDataGenerate.GenerateSitesAndWarehouses(countSite, countWare);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(sites.Count, countSite);
                Assert.AreEqual(sites.Select(x => x.SiteId).Distinct().Count(), countSite);
                var warehouses = sites.Select(x => x.Warehouses).FirstOrDefault();
                Assert.AreEqual(warehouses.Select(x => x.WarehouseId).Distinct().Count(), countWare);
            });
        }

        [Test]
        [Category("Warehouse")]
        [TestCase(1)]
        [TestCase(10)]
        public async Task InsertWarehouse_ValidWarehouse_Should_Succeed(int numToInsert)
        {
            var warehouses = _fakeDataGenerate.GenerateWarehouses(numToInsert);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(warehouses.Count, numToInsert);
                Assert.AreEqual(warehouses.Select(x => x.WarehouseId).Distinct().Count(), numToInsert);
            });
        }

        [Test]
        [Category("Area")]
        [TestCase(1, 1)]
        [TestCase(5, 10)]
        public async Task InsertArea_ValidAreaAndAreaType_Should_Succeed(int numAreaType, int numAreaForType)
        {
            var areas = _fakeDataGenerate.GenerateAreas(numAreaType, numAreaForType);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(areas.Count, numAreaType * numAreaForType);
                Assert.AreEqual(areas.Select(x => x.AreaId).Distinct().Count(), numAreaType * numAreaForType);
            });
        }

        [Test]
        [Category("Location")]
        [TestCase(1, 1)]
        [TestCase(5, 10)]
        public async Task InsertLocation_ValidLocationAndLocationType_Should_Succeed(int numLocationType, int numLocationForType)
        {
            var locations = _fakeDataGenerate.GenerateLocations(numLocationType, numLocationForType);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(locations.Count, numLocationType * numLocationForType);
                Assert.AreEqual(locations.Select(x => x.LocationId).Distinct().Count(), numLocationType * numLocationForType);
            });
        }

        [Test]
        [Category("Site-Warehouse-Area-Location")]
        [TestCase(2, 3, 4, 5)]
        public async Task Insert_MultySitesMultyWarehousesMultyAreasMultyLocations_Should_Succeed(int countSite, int countWare, int countArea, int countLocation)
        {
            var sites = _fakeDataGenerate.GenerateMultySitesWarehousesAreasLocations(countSite, countWare, countArea, countLocation);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(sites.Count, countSite);
                Assert.AreEqual(sites.Select(x => x.SiteId).Distinct().Count(), countSite);
                var warehouses = sites.Select(x => x.Warehouses).FirstOrDefault();
                Assert.AreEqual(warehouses.Select(x => x.WarehouseId).Distinct().Count(), countWare);
                var area = warehouses.Select(x => x.Areas).FirstOrDefault();
                Assert.AreEqual(area.Select(x => x.AreaId).Distinct().Count(), countArea);
                var location = area.Select(x => x.Locations).FirstOrDefault();
                Assert.AreEqual(location.Select(x => x.LocationId).Distinct().Count(), countLocation);
            });
        }

        [Test]
        [Category("StorageUnit")]
        [TestCase(1, 1)]
        [TestCase(5, 10)]
        public async Task InsertStorageUnit_ValidStorageUnitAndStorageUnitType_Should_Succeed(int numStorageType, int numStorageForType)
        {
            var storageUnites = _fakeDataGenerate.GenerateStorageUnits(numStorageType, numStorageForType);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(storageUnites.Count, numStorageType * numStorageForType);
                Assert.AreEqual(storageUnites.Select(x => x.StorageUnitId).Distinct().Count(), numStorageType * numStorageForType);
            });
        }
        #endregion

        #region Integry
        [Test]
        [Category("Warehouse")]
        public async Task InsertWarehouse_InvalidSiteId_Should_BeExcemption()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var ctx = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    DatabaseHelper.InsertWarehouse(ctx, Guid.NewGuid());
                }
            });

            // Verifica che non sia stato inserito il warehouse perchè la chiave ereditata non è valida
            AssertForeignKeyIvalid(ex, "FK_Warehouse_Site", "Sites", "SiteId");
        }

        [Test]
        [Category("Area")]
        public async Task InsertArea_InvalidAreaTypeId_Should_BeExcemption()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var ctx = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var site = DatabaseHelper.InsertSite(ctx);
                    var warehouse = DatabaseHelper.InsertWarehouse(ctx, site.SiteId);
                    var areaType = DatabaseHelper.InsertAreaType(ctx);
                    DatabaseHelper.InsertArea(ctx, Guid.NewGuid(), warehouse.WarehouseId);
                }
            });

            // Verifica che non sia stato inserito il warehouse perchè la chiave ereditata non è valida
            AssertForeignKeyIvalid(ex, "FK_Area_AreaType", "AreaTypes", "AreaTypeId");
        }

        [Test]
        [Category("Area")]
        public async Task InsertArea_InvalidWarehouseId_Should_BeExcemption()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var ctx = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var areaType = DatabaseHelper.InsertAreaType(ctx);
                    DatabaseHelper.InsertArea(ctx, areaType.AreaTypeId, Guid.NewGuid());
                }
            });

            // Verifica che non sia stato inserito il warehouse perchè la chiave ereditata non è valida
            AssertForeignKeyIvalid(ex, "FK_Area_Warehouse", "Warehouses", "WarehouseId");
        }

        [Test]
        [Category("Location")]
        public async Task InsertLocation_InvalidAreaId_Should_BeExcemption()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var ctx = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var locationT = DatabaseHelper.InsertLocationType(ctx);
                    DatabaseHelper.InsertLocation(ctx, locationT.LocationTypeId, Guid.NewGuid());
                }
            });

            // Verifica che non sia stato inserito il warehouse perchè la chiave ereditata non è valida
            AssertForeignKeyIvalid(ex, "FK_Location_Area", "Areas", "AreaId");
        }

        [Test]
        [Category("Location")]
        public async Task InsertLocation_InvalidLocationTypeId_Should_BeExcemption()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var ctx = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var site = DatabaseHelper.InsertSite(ctx);
                    var warehouse = DatabaseHelper.InsertWarehouse(ctx, site.SiteId);
                    var areaType = DatabaseHelper.InsertAreaType(ctx);
                    var area = DatabaseHelper.InsertArea(ctx, areaType.AreaTypeId, warehouse.WarehouseId);
                    DatabaseHelper.InsertLocation(ctx, Guid.NewGuid(), area.AreaId);
                }
            });

            // Verifica che non sia stato inserito il warehouse perchè la chiave ereditata non è valida
            AssertForeignKeyIvalid(ex, "FK_Location_LocationType", "LocationTypes", "LocationTypeId");
        }
        #endregion

        #region Duplicete
        [Test]
        [Category("Site")]
        public async Task InsertSite_DuplicateSite_Should_BeExcemption()
        {
            var sites = _fakeDataGenerate.GenerateSites();
            _context.Sites.Add(new Site
            {
                Code = sites[1].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30),
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "Sites", "IX_Site_Code");
        }

        [Test]
        [Category("Warehouse")]
        public async Task InsertWarehouse_DuplicateWarehouse_Should_BeExcemption()
        {
            var warehouses = _fakeDataGenerate.GenerateWarehouses();
            _context.Warehouses.Add(new Warehouse
            {
                Code = warehouses[1].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30),
                SiteId = warehouses[1].SiteId,
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "Warehouses", "IX_Warehouse_SiteId_Code");
        }

        [Test]
        [Category("Area")]
        public async Task InsertArea_DuplicateArea_Should_BeExcemption()
        {
            var areas = _fakeDataGenerate.GenerateAreas();
            _context.Areas.Add(new Area
            {
                Code = areas[1].Code,
                AreaTypeId = areas[1].AreaId,
                WarehouseId = areas[1].WarehouseId,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30),
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "Areas", "IX_Area_WarehouseId_Code");
        }

        [Test]
        [Category("Location")]
        public async Task InsertLocation_DuplicateLocation_Should_BeExcemption()
        {
            var locations = _fakeDataGenerate.GenerateLocations();
            _context.Locations.Add(new Location
            {
                Code = locations[1].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30),
                LocationTypeId = locations[1].LocationTypeId,
                AreaId = locations[1].AreaId,
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "Locations", "IX_Location_AreaId_Code");
        }

        [Test]
        [Category("StorageUnit")]
        public async Task InsertStorageUnit_DuplicateStorageUnit_Should_BeExcemption()
        {
            var storageUnits = _fakeDataGenerate.GenerateStorageUnits();
            _context.StorageUnits.Add(new StorageUnit
            {
                Code = storageUnits[1].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30),
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "StorageUnits", "IX_StorageUnit_Code");
        }
        #endregion

        #region AssertCannotInsertNull
        [Test]
        [Category("Site")]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public void InsertSite_InsertFieldNull_Should_BeExcemption(string columnName, string? code, string? description)
        {
            _context.Sites.Add(new Site
            {
                Code = code,
                Description = description
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "Sites", columnName);
        }

        [Test]
        [Category("Warehouse")]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public async Task InsertWarehouse_InsertFieldNull_Should_BeExcemption(string columnName, string? code, string? description)
        {
            var warehouses = _fakeDataGenerate.GenerateWarehouses();
            _context.Warehouses.Add(new Warehouse
            {
                Code = code,
                Description = description,
                SiteId = warehouses[1].SiteId,
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "Warehouses", columnName);
        }

        [Test]
        [Category("Area")]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public async Task InsertArea_InsertFieldNull_Should_BeExcemption(string columnName, string? code, string? description)
        {
            var areas = _fakeDataGenerate.GenerateAreas();
            _context.Areas.Add(new Area
            {
                Code = code,
                Description = description,
                AreaTypeId = areas[1].AreaId,
                WarehouseId = areas[1].WarehouseId,
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "Areas", columnName);
        }

        [Test]
        [Category("Location")]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public async Task InsertLocation_InsertFieldNull_Should_BeExcemption(string columnName, string? code, string? description)
        {
            var locations = _fakeDataGenerate.GenerateLocations();
            _context.Locations.Add(new Location
            {
                Code = code,
                Description = description,
                LocationTypeId = locations[1].LocationTypeId,
                AreaId = locations[1].AreaId,
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "Locations", columnName);
        }

        [Test]
        [Category("StorageUnit")]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public async Task InsertStorageUnit_InsertFieldNull_Should_BeExcemption(string columnName, string? code, string? description)
        {
            var storageUnits = _fakeDataGenerate.GenerateStorageUnits();
            _context.StorageUnits.Add(new StorageUnit
            {
                Code = code,
                Description = description,
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "StorageUnits", columnName);
        }
        #endregion
    }
}
