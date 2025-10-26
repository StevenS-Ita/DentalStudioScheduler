using FluentAssertions;
using Mch.Internal.UnifiedContextDb.Models;
using Mch.Internal.UnifiedContextDb.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class EquipmentTest : BaseClassTest
    {
        [Test, Category("Should be Initialized")]
        public void DbSets_ShouldBeInitialized()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();

            context.Equipment.Should().NotBeNull();
            context.EquipmentTools.Should().NotBeNull();
            context.EquipmentDocs.Should().NotBeNull();
            context.EquipmentTypes.Should().NotBeNull();
            context.EquipmentMaintenances.Should().NotBeNull();
        }

        /* ------------------------------ Positive Test ------------------------------ */
        [Test, Category("Insert EquipmentType and Insert Equipment")]
        [TestCase(null, null, null, null, 0)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025)]
        public void InsertEquipment_EquipmentTypeAndEquipment_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping)
        {
            string code = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);
            });
        }

        [Test, Category("Insert Equipment and EquipmentTool")]
        [TestCase(null, null, null, null, 0)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025)]
        public void InsertEquipment_EquipmentAndEquipmentTool_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping)
        {
            string code = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                var equipment = DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
                DatabaseHelper.InsertEquipmentTool(context, equipment.EquipmentId);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment
                .Include(x => x.EquipmentTools)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);
                Assert.AreEqual(equipment_.EquipmentTools.Count, 1);
            });
        }

        [Test, Category("Insert EquipmentType and Insert Equipment and EquipmentTools")]
        [TestCase(null, null, null, null, 0)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025)]
        public void InsertEquipment_EquipmentAndEquipmentTools_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping)
        {
            string code = "ED001";
            string tCode1 = "TOOL123", tCode2 = "TOOL124", tCode3 = "TOOL125";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                var equipment = DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
                DatabaseHelper.InsertEquipmentTool(context, equipment.EquipmentId, tCode1, EquipmentToolStatus.New);
                DatabaseHelper.InsertEquipmentTool(context, equipment.EquipmentId, tCode2, EquipmentToolStatus.Used);
                DatabaseHelper.InsertEquipmentTool(context, equipment.EquipmentId, tCode3, EquipmentToolStatus.Retired);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment
                .Include(x => x.EquipmentTools)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);
                Assert.AreEqual(equipment_.EquipmentTools.Count, 3);

                var tool1 = equipment_.EquipmentTools.FirstOrDefault(d => d.Code == tCode1);
                var tool2 = equipment_.EquipmentTools.FirstOrDefault(d => d.Code == tCode2);
                var tool3 = equipment_.EquipmentTools.FirstOrDefault(d => d.Code == tCode3);
                Assert.IsNotNull(tool1);
                Assert.IsNotNull(tool2);
                Assert.IsNotNull(tool3);
                Assert.AreEqual(tool1.Status, EquipmentToolStatus.New);
                Assert.AreEqual(tool2.Status, EquipmentToolStatus.Used);
                Assert.AreEqual(tool3.Status, EquipmentToolStatus.Retired);
            });
        }

        [Test, Category("Insert Equipment and EquipmentDoc")]
        [TestCase(null, null, null, null, 0)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025)]
        public void InsertEquipment_EquipmentAndEquipmentDoc_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping)
        {
            string code = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                var equipment = DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
                DatabaseHelper.InsertEquipmentDoc(context, equipment.EquipmentId, documentType: DocumentType.Generic_Undefined);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment
                .Include(x => x.EquipmentDocs)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);

                Assert.AreEqual(equipment_.EquipmentDocs.Count, 1);
                var doc = equipment_.EquipmentDocs.FirstOrDefault();
                Assert.IsNotNull(doc);
                Assert.IsTrue(doc.FileType == DocumentType.Generic_Undefined);

            });
        }

        [Test, Category("Insert Equipment and EquipmentDocs")]
        [TestCase(null, null, null, null, 0, DocumentType.UserAndMaintenanceManual)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025, DocumentType.UserAndMaintenanceManual)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025, DocumentType.Generic_Undefined)]
        public void InsertEquipment_EquipmentAndEquipmentDocs_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping,
                                                                              DocumentType documentType)
        {
            string code = "ED001";
            string fileName1 = "prova.pdf", fileName2 = "prova_220701.pdf";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                var equipment = DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
                DatabaseHelper.InsertEquipmentDoc(context, equipment.EquipmentId, fileName1, documentType);
                DatabaseHelper.InsertEquipmentDoc(context, equipment.EquipmentId, fileName2, DocumentType.Documentation);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment
                .Include(x => x.EquipmentDocs)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);
                Assert.AreEqual(equipment_.EquipmentDocs.Count, 2);

                // Verifica documenti
                var doc1 = equipment_.EquipmentDocs.FirstOrDefault(d => d.FileName == fileName1);
                var doc2 = equipment_.EquipmentDocs.FirstOrDefault(d => d.FileName == fileName2);
                Assert.IsNotNull(doc1);
                Assert.IsNotNull(doc2);
                Assert.IsTrue(doc1.FileType == documentType);
                Assert.IsTrue(doc2.FileType == DocumentType.Documentation);

                Assert.AreEqual(equipment_.EquipmentId, doc1.EquipmentId);
                Assert.AreNotEqual(doc1.EquipmentDocId, doc2.EquipmentDocId);
                Assert.AreEqual(equipment_.EquipmentId, doc2.EquipmentId);
            });
        }

        [Test, Category("Insert Equipment and EquipmentMaintenance")]
        [TestCase(null, null, null, null, 0)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025)]
        public void InsertEquipment_EquipmentAndEquipmentMaintenance_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping)
        {
            string code = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                var equipment = DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
                DatabaseHelper.InsertEquipmentMaintenance(context, equipment.EquipmentId, MaintenanceType.RegularMaintenance, MaintenanceStatus.ScheduledMaintenance);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment
                .Include(x => x.EquipmentMaintenance)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);

                Assert.AreEqual(equipment_.EquipmentMaintenance.Count, 1);
                var main = equipment_.EquipmentMaintenance.FirstOrDefault();
                Assert.IsTrue(main.MaintenanceType == MaintenanceType.RegularMaintenance && main.Status == MaintenanceStatus.ScheduledMaintenance);
            });
        }

        [Test, Category("Insert EquipmentType and Insert Equipment")]
        [TestCase(null, null, null, null, 0)]
        [TestCase("SN-84JX-29LM-Q7RT", "Monitor Del", "Dell", "Personal compouter", 2025)]
        public void InsertEquipment_Equipment_ShouldSucceedAsync(string? serialNR, string? accessories, string? producer, string? description, int yearScrapping)
        {
            string code = "ED001";
            string tCode1 = "TOOL123";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                var equipment = DatabaseHelper.InsertEquipment(context, code, equipmentType.EquipmentTypeId, serialNR: serialNR, accessories: accessories, producer: producer, description: description, yearScrapping: yearScrapping);
                DatabaseHelper.InsertEquipmentTool(context, equipment.EquipmentId, tCode1, EquipmentToolStatus.New);
                DatabaseHelper.InsertEquipmentDoc(context, equipment.EquipmentId, documentType: DocumentType.Generic_Undefined);
                DatabaseHelper.InsertEquipmentMaintenance(context, equipment.EquipmentId, MaintenanceType.RegularMaintenance, MaintenanceStatus.ScheduledMaintenance);
            }

            var equipmentType_ = context.EquipmentTypes.FirstOrDefault();
            var equipment_ = context.Equipment
                .Include(x => x.EquipmentTools)
                .Include(x => x.EquipmentDocs)
                .Include(x => x.EquipmentMaintenance)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(equipmentType_);
                Assert.IsNotNull(equipment_);
                Assert.AreEqual(equipment_.Code, code);

                Assert.AreEqual(equipment_.EquipmentTools.Count, 1);
                var tool = equipment_.EquipmentTools.FirstOrDefault(d => d.Code == tCode1);
                Assert.IsNotNull(tool);
                Assert.AreEqual(tool.Status, EquipmentToolStatus.New);

                Assert.AreEqual(equipment_.EquipmentDocs.Count, 1);
                var doc = equipment_.EquipmentDocs.FirstOrDefault();
                Assert.IsNotNull(doc);
                Assert.IsTrue(doc.FileType == DocumentType.Generic_Undefined);

                Assert.AreEqual(equipment_.EquipmentMaintenance.Count, 1);
                var main = equipment_.EquipmentMaintenance.FirstOrDefault();
                Assert.IsTrue(main.MaintenanceType == MaintenanceType.RegularMaintenance && main.Status == MaintenanceStatus.ScheduledMaintenance);
            });
        }

        /* ------------------------------ EquipmentTypeId is Empty ------------------------------ */
        [Test, Category("Try insert Equipment wicth EquipmentTypeId is Empty")]
        public void TryInsertEquipment_EquipmentIdEmpty_ShouldExceptionAsync()
        {
            string code = "ED001";
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                    DatabaseHelper.InsertEquipment_EmptyId(context);
                }
            });

            // Verifica opzionale sul messaggio
            AssertForeignKeyIvalid(ex, "FK_EquipmentType_Equipment", "EquipmentTypes", "EquipmentTypeId");
        }

        /* ------------------------------ EquipmentId is Empty ------------------------------ */
        [Test, Category("Try Insert EquipmentTool wicth EquipmentId is Empty")]
        public void TryInsertEquipment_EquipmentToolIdEmpty_ShouldExceptionAsync()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                    var equipment = DatabaseHelper.InsertEquipment(context, "ED001", equipmentType.EquipmentTypeId, serialNR: "SN-84JX-29LM-Q7RT", accessories: "Monitor Del", producer: "Dell", description: "Personal compouter", yearScrapping: 2025);
                    DatabaseHelper.InsertEquipmentTool_EmptyId(context);
                }
            });

            // Verifica opzionale sul messaggio
            AssertForeignKeyIvalid(ex, "FK_EquipmentTools_Equipment", "Equipment", "EquipmentId");
        }

        [Test, Category("Try Insert EquipmentMaintenance wicth EquipmentId is Empty")]
        public void TryInsertEquipment_EquipmentMaintenanceEquipmentIdEmpty_ShouldExceptionAsync()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                    var equipment = DatabaseHelper.InsertEquipment(context, "ED001", equipmentType.EquipmentTypeId, serialNR: "SN-84JX-29LM-Q7RT", accessories: "Monitor Del", producer: "Dell", description: "Personal compouter", yearScrapping: 2025);
                    DatabaseHelper.InsertEquipmentMaintenance_EmptyId(context);
                }
            });

            // Verifica opzionale sul messaggio
            AssertForeignKeyIvalid(ex, "FK_EquipmentMaintenance_Equipment", "Equipment", "EquipmentId");
        }

        /* ------------------------------ Insert Multy ------------------------------ */
        [Test, Category("Insert Multy EquipmentType")]
        public void InsertMulty_EquipmentType_Should_Exception_DuplicateKey()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                    var equipmentType2 = new EquipmentType()
                    {
                        EquipmentTypeId = equipmentType.EquipmentTypeId,
                        Code = "ET001",
                        Description = "description"
                    };
                    context.EquipmentTypes.Add(equipmentType);
                    context.SaveChanges();
                }
            });

            // Verifica opzionale sul messaggio
            AssertPrimaryKey(ex, "EquipmentTypes", "PK_EquipmentType");
        }

        [Test, Category("Insert Multy Equipment")]
        public void InsertMulty_Equipment_Should_Exception_DuplicateKey()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                    var equipment = DatabaseHelper.InsertEquipment(context, "ED001", equipmentType.EquipmentTypeId, serialNR: "SN-84JX-29LM-Q7RT", accessories: "Monitor Del", producer: "Dell", description: "Personal compouter", yearScrapping: 2025);
                    var equipment2 = new Equipment()
                    {
                        EquipmentId = equipment.EquipmentId,
                        Code = "ED001",
                        Asset = "PC-25-07-000",
                        YearAcquisition = 2020,
                        Status = EquipmentStatus.InUse,
                        EquipmentTypeId = equipmentType.EquipmentTypeId,
                    };
                    context.Equipment.Add(equipment);
                    context.SaveChanges();
                }
            });

            // Verifica opzionale sul messaggio
            AssertPrimaryKey(ex, "Equipment", "PK_Equipment");
        }

        [Test, Category("Insert Multy EquipmentTool")]
        public void InsertMulty_EquipmentTool_Should_Exception_DuplicateKey()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var equipmentType = DatabaseHelper.InsertEquipmentType(context);
                    var equipment = DatabaseHelper.InsertEquipment(context, "ED001", equipmentType.EquipmentTypeId, serialNR: "SN-84JX-29LM-Q7RT", accessories: "Monitor Del", producer: "Dell", description: "Personal compouter", yearScrapping: 2025);
                    var equipmentTool = DatabaseHelper.InsertEquipmentTool(context, equipment.EquipmentId);
                    var equipmentTool2 = new EquipmentTool()
                    {
                        EquipmentToolId = equipmentTool.EquipmentId,
                        Code = "EO001",
                        Status = EquipmentToolStatus.Used,
                        EquipmentId = equipment.EquipmentId,
                    };
                    context.EquipmentTools.Add(equipmentTool);
                    context.SaveChanges();
                }
            });

            // Verifica opzionale sul messaggio
            AssertPrimaryKey(ex, "EquipmentTools", "PK_EquipmentTool");
        }
    }
}
