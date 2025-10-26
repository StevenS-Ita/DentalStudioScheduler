using Mch.Authentication.ContextDb.Models;
using Mch.Internal.UnifiedContextDb.Models;
using Mch.Internal.UnifiedContextDb.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mch.Internal.UnifiedContextDb.Test
{
    public static class DatabaseHelper
    {
        public static CustomerSupplier InsertCustomerSupplier(InternalContext context)
        {
            var customerSupplier = new CustomerSupplier()
            {
                CompanyName = "Electrolux",
                Code = "ES11706M03707",
                TaxId = "01094820931",
                CompanyType = CompanyType.Company,
                FunctionType = CompanyFunctionType.Customer,
            };
            context.CustomerSuppliers.Add(customerSupplier);
            context.SaveChanges();
            return customerSupplier;
        }

        public static CustomerSupplierAddress InsertCustomerSupplierAddress(InternalContext context, Guid customerSupplierId, AddressType addressType = AddressType.Location, AddressDefault defaultAddress = AddressDefault.IsDefaultAddress)
        {
            var address = new CustomerSupplierAddress()
            {
                AddressType = addressType,
                Street = "Via Foresto Est",
                StreetNumber = 16,
                City = "Santa Lucia di Piave",
                Province = "TV",
                Region = "Veneto",
                DefaultAddress = defaultAddress,
                CustomerSupplierId = customerSupplierId,
            };
            context.CustomerSupplierAddresses.Add(address);
            context.SaveChanges();
            return address;
        }

        public static List<CustomerSupplierAddress> InsertMultyCustomerSupplierAddress(InternalContext context, Guid customerSupplierId,
                                                                                        AddressType addressType1 = AddressType.Location, AddressDefault defaultAddress1 = AddressDefault.IsDefaultAddress,
                                                                                        AddressType addressType2 = AddressType.Location, AddressDefault defaultAddress2 = AddressDefault.IsDefaultAddress)
        {
            var addresses = new List<CustomerSupplierAddress>();
            //Inserimento del secondo Indirizzo, non di Default
            var address1 = new CustomerSupplierAddress()
            {
                AddressType = addressType1,
                Street = "Via Foresto Ovest",
                StreetNumber = 20,
                City = "Santa Lucia di Piave",
                Province = "TV",
                Region = "Veneto",
                DefaultAddress = defaultAddress2,
                CustomerSupplierId = customerSupplierId,
            };
            context.CustomerSupplierAddresses.Add(address1);
            addresses.Add(address1);
            context.SaveChanges();

            //Inserimento del primo Indirizzo, quello di Default
            var address2 = InsertCustomerSupplierAddress(context, customerSupplierId, addressType2, defaultAddress2);
            context.CustomerSupplierAddresses.Add(address2);
            addresses.Add(address2);
            return addresses;
        }

        public static ArticleType InsertArticleType(InternalContext context)
        {
            var articleType = new ArticleType()
            {
                Code = "MATERIALI",
                Description = "MATERIALI"
            };

            context.ArticleTypes.Add(articleType);
            context.SaveChanges();
            return articleType;
        }

        public static Article InsertArticle(InternalContext context, Guid articleTypeId, string? code = null)
        {
            var article = new Article()
            {
                Code = code ?? "EA11706M03702",
                Description = "PIASTRA INFERIORE PER GRUPPI SOLLEVAMENTO",
                ArticleTypeId = articleTypeId
            };
            context.Articles.Add(article);
            context.SaveChanges();
            return article;
        }

        public static CNC InsertCNC(InternalContext context, string? code = null)
        {
            var cnc = new CNC()
            {
                Code = code ?? "EmcoMill",
                Description = "EmcoMill"
            };
            context.CNCs.Add(cnc);
            context.SaveChanges();
            return cnc;
        }

        /* ------------------------------ Insert CNC_Article ------------------------------ */

        public static CNC_Article InsertCNC_Article(InternalContext context, Guid CNC_Id, Guid articleId, string drawingName, string sourceMaterial = null,
                                                    string programName = null, string programContent = null, string realizationNotes = null)
        {
            var cncArticle = new CNC_Article()
            {
                CNC_Id = CNC_Id,
                ArticleId = articleId,
                DrawingName = drawingName,
                BaseMaterial = sourceMaterial,
                ProgramName = programName,
                ProgramContent = programContent,
                ProgrammingFirstTime = new TimeSpan(1, 1, 1),
                ProgrammingTime = new TimeSpan(2, 2, 2),
                SetupTime = new TimeSpan(3, 3, 3),
                ProductionTime = new TimeSpan(4, 4, 4),
                RealizationNotes = realizationNotes,
            };
            context.CNC_Articles.Add(cncArticle);
            context.SaveChanges();
            return cncArticle;
        }

        public static CNC_Article InsertCNC_Article_ProgrammingFirstTime(InternalContext context, Guid CNC_Id, Guid articleId, string drawingName, TimeSpan programmingFirstTime,
                                                                   string sourceMaterial = null, string programName = null,
                                                                   string programContent = null, string realizationNotes = null)
        {
            var cncArticle = new CNC_Article()
            {
                CNC_Id = CNC_Id,
                ArticleId = articleId,
                DrawingName = drawingName,
                BaseMaterial = sourceMaterial,
                ProgramName = programName,
                ProgramContent = programContent,
                ProgrammingFirstTime = programmingFirstTime,
                ProgrammingTime = new TimeSpan(2, 2, 2),
                SetupTime = new TimeSpan(3, 3, 3),
                ProductionTime = new TimeSpan(4, 4, 4),
                RealizationNotes = realizationNotes,
            };
            context.CNC_Articles.Add(cncArticle);
            context.SaveChanges();
            return cncArticle;
        }

        public static CNC_Article InsertCNC_Article_ProgrammingTime(InternalContext context, Guid CNC_Id, Guid articleId, string drawingName, TimeSpan programmingTime,
                                                                    string sourceMaterial = null, string programName = null,
                                                                    string programContent = null, string realizationNotes = null)
        {
            var cncArticle = new CNC_Article()
            {
                CNC_Id = CNC_Id,
                ArticleId = articleId,
                DrawingName = drawingName,
                BaseMaterial = sourceMaterial,
                ProgramName = programName,
                ProgramContent = programContent,
                ProgrammingFirstTime = new TimeSpan(2, 2, 2),
                ProgrammingTime = programmingTime,
                SetupTime = new TimeSpan(3, 3, 3),
                ProductionTime = new TimeSpan(4, 4, 4),
                RealizationNotes = realizationNotes,
            };
            context.CNC_Articles.Add(cncArticle);
            context.SaveChanges();
            return cncArticle;
        }
        public static CNC_Article InsertCNC_Article_SetupTime(InternalContext context, Guid CNC_Id, Guid articleId, string drawingName, TimeSpan setupTime,
                                                              string sourceMaterial = null, string programName = null,
                                                              string programContent = null, string realizationNotes = null)
        {
            var cncArticle = new CNC_Article()
            {
                CNC_Id = CNC_Id,
                ArticleId = articleId,
                DrawingName = drawingName,
                BaseMaterial = sourceMaterial,
                ProgramName = programName,
                ProgramContent = programContent,
                ProgrammingFirstTime = new TimeSpan(2, 2, 2),
                ProgrammingTime = new TimeSpan(3, 3, 3),
                SetupTime = setupTime,
                ProductionTime = new TimeSpan(4, 4, 4),
                RealizationNotes = realizationNotes,
            };
            context.CNC_Articles.Add(cncArticle);
            context.SaveChanges();
            return cncArticle;
        }
        public static CNC_Article InsertCNC_Article_ProductionTime(InternalContext context, Guid CNC_Id, Guid articleId, string drawingName, TimeSpan productionTime,
                                                                   string sourceMaterial = null, string programName = null,
                                                                   string programContent = null, string realizationNotes = null)
        {
            var cncArticle = new CNC_Article()
            {
                CNC_Id = CNC_Id,
                ArticleId = articleId,
                DrawingName = drawingName,
                BaseMaterial = sourceMaterial,
                ProgramName = programName,
                ProgramContent = programContent,
                ProgrammingFirstTime = new TimeSpan(2, 2, 2),
                ProgrammingTime = new TimeSpan(3, 3, 3),
                SetupTime = new TimeSpan(4, 4, 4),
                ProductionTime = productionTime,
                RealizationNotes = realizationNotes,
            };
            context.CNC_Articles.Add(cncArticle);
            context.SaveChanges();
            return cncArticle;
        }

        /* ------------------------------ Insert CNC_ArticleLog ------------------------------ */

        public static void InsertCNC_ArticleLog(InternalContext context, CNC_Article cncArticle, CNC_Article cncArticleEdit, string? logDescription = null)
        {
            var cncArticleLog = GetCNC_ArticleLog(cncArticle, logDescription);
            if (!context.CNC_ArticleLogs.Any())
            {
                cncArticleLog.CncArticleOperationType = CncArticleOperationType.CncArticleAdded;
            }
            else
            {
                cncArticleLog.CncArticleOperationType = CncArticleOperationType.CncArticleChange;
            }
            context.CNC_ArticleLogs.Add(cncArticleLog);

            cncArticle.DrawingName = cncArticleEdit.DrawingName;
            cncArticle.BaseMaterial = cncArticleEdit.BaseMaterial;
            cncArticle.ProgramName = cncArticleEdit.ProgramName;
            cncArticle.ProgramContent = cncArticleEdit.ProgramContent;
            cncArticle.ProgrammingFirstTime = cncArticleEdit.ProgrammingFirstTime;
            cncArticle.ProgrammingTime = cncArticleEdit.ProgrammingTime;
            cncArticle.SetupTime = cncArticleEdit.SetupTime;
            cncArticle.ProductionTime = cncArticleEdit.ProductionTime;
            cncArticle.RealizationNotes = cncArticleEdit.RealizationNotes;
            context.CNC_Articles.Update(cncArticle);
            context.SaveChanges();
        }

        public static CNC_ArticleLog GetCNC_ArticleLog(CNC_Article cncArticle, string? logDescription = null)
        {
            return new CNC_ArticleLog()
            {
                DrawingName = cncArticle.DrawingName,
                BaseMaterial = cncArticle.BaseMaterial,
                ProgramName = cncArticle.ProgramName,
                ProgramContent = cncArticle.ProgramContent,
                ProgrammingFirstTime = cncArticle.ProgrammingFirstTime,
                ProgrammingTime = cncArticle.ProgrammingTime,
                SetupTime = cncArticle.SetupTime,
                ProductionTime = cncArticle.ProductionTime,
                RealizationNotes = cncArticle.RealizationNotes,
                LogDescription = logDescription,
            };
        }

        /* ------------------------------ Insert CNC_ArticleDoc ------------------------------ */

        public static void InsertCNC_ArticleDoc(InternalContext context, Guid CNC_ArticleId, string fileName = "prova.pdf",
                                                DocumentType fileType = DocumentType.Invoice)
        {
            context.CNC_ArticleDocs.Add(new CNC_ArticleDoc()
            {
                FileName = fileName,
                FileType = fileType,
                Blob = new byte[10],
                CNC_ArticleId = CNC_ArticleId,
            });
            context.SaveChanges();
        }

        /* ------------------------------ Insert CNC_ArticleDocLog ------------------------------ */

        public static void InsertCNC_ArticleDocLog(InternalContext context, CNC_ArticleDoc cncArticleDoc, CNC_ArticleDoc cncArticleDocEdit, string? logDescription = null)
        {
            var cncArticleDocLog = GetCNC_ArticleDocLog(cncArticleDoc, logDescription);
            if (!context.CNC_ArticleDocLogs.Any())
            {
                cncArticleDocLog.CncArticleDocOperationType = CncArticleDocOperationType.CncArticleDocAdded;
            }
            else
            {
                cncArticleDocLog.CncArticleDocOperationType = CncArticleDocOperationType.CncArticleDocChange;
            }
            context.CNC_ArticleDocLogs.Add(cncArticleDocLog);

            cncArticleDoc.FileName = cncArticleDocEdit.FileName;
            cncArticleDoc.FileType = cncArticleDocEdit.FileType;
            cncArticleDoc.Blob = cncArticleDocEdit.Blob;
            context.CNC_ArticleDocs.Update(cncArticleDoc);
            context.SaveChanges();
        }

        public static CNC_ArticleDocLog GetCNC_ArticleDocLog(CNC_ArticleDoc cncArticleDoc, string? logDescription = null)
        {
            return new CNC_ArticleDocLog()
            {
                FileName = cncArticleDoc.FileName,
                FileType = cncArticleDoc.FileType,
                Blob = cncArticleDoc.Blob,
                LogDescription = logDescription,
            };
        }


        /* ------------------------------ Deleted CNC_Article ------------------------------ */

        public static void DeleteCNC_Article(InternalContext context, string drawingName, string sourceMaterial = null, string programName = null)
        {
            var cncArticle = FoundCNC_Article(context, drawingName, sourceMaterial, programName);
            if (cncArticle is not null)
            {
                var cncArticleLog = GetCNC_ArticleLog(cncArticle, "Deleted CNC_Article");
                cncArticleLog.CncArticleOperationType = CncArticleOperationType.CncArticleDeleted;
                context.CNC_ArticleLogs.Add(cncArticleLog);
                context.SaveChanges();

                context.CNC_Articles.Remove(cncArticle);
                context.SaveChanges();
            }
        }

        public static CNC_Article FoundCNC_Article(InternalContext context, string drawingName, string sourceMaterial = null, string programName = null)
        {
            if (drawingName is not null || sourceMaterial is not null || programName is not null)
            {
                if (drawingName is null && sourceMaterial is null)
                    return context.CNC_Articles.FirstOrDefault(x => x.ProgramName == programName);
                if (drawingName is null && programName is null)
                    return context.CNC_Articles.FirstOrDefault(x => x.BaseMaterial == sourceMaterial);
                if (sourceMaterial is null && programName is null)
                    return context.CNC_Articles.FirstOrDefault(x => x.DrawingName == drawingName);

                if (drawingName is not null && sourceMaterial is not null)
                    return context.CNC_Articles.FirstOrDefault(x => x.ProgramName == programName && x.BaseMaterial == sourceMaterial);
                if (drawingName is not null && programName is not null)
                    return context.CNC_Articles.FirstOrDefault(x => x.ProgramName == programName && x.BaseMaterial == sourceMaterial);
                if (programName is not null && sourceMaterial is not null)
                    return context.CNC_Articles.FirstOrDefault(x => x.ProgramName == programName && x.BaseMaterial == sourceMaterial);
            }
            return null;
        }

        public static void DeleteCNC_ArticleDoc(InternalContext context, DocumentType fileType, string fileName = "prova.pdf", string? logDescription = null)
        {
            var cncArticleDoc = context.CNC_ArticleDocs.FirstOrDefault(x => x.FileName == fileName && x.FileType == fileType);
            var cncArticleDocLog = GetCNC_ArticleDocLog(cncArticleDoc, logDescription);
            cncArticleDocLog.CncArticleDocOperationType = CncArticleDocOperationType.CncArticleDocDeleted;
            context.CNC_ArticleDocLogs.Add(cncArticleDocLog);
            context.SaveChanges();

            context.CNC_ArticleDocs.Remove(cncArticleDoc);
            context.SaveChanges();
        }

        /* ------------------------------ Equipment ------------------------------ */

        public static Equipment InsertEquipment(InternalContext context, string code, Guid equipmentTypeId, Guid? customerSupplierId = null,
                                                string serialNR = null, string accessories = null, string producer = null, string description = null, int yearScrapping = 0)
        {
            var equipment = new Equipment()
            {
                Code = code,
                Asset = "PC-25-07-000",
                SerialNR = serialNR,
                Accessories = accessories,
                YearAcquisition = 2020,
                Status = EquipmentStatus.InUse,
                Producer = producer,
                Description = description,
                YearScrapping = yearScrapping,
                EquipmentTypeId = equipmentTypeId,
            };
            if (customerSupplierId is not null)
            {
                equipment.CustomerSupplierId = customerSupplierId;
            }
            context.Equipment.Add(equipment);
            context.SaveChanges();
            return equipment;
        }

        public static EquipmentType InsertEquipmentType(InternalContext context, string description = "")
        {
            var equipmentType = new EquipmentType()
            {
                Code = "ET001",
                Description = description
            };
            context.EquipmentTypes.Add(equipmentType);
            context.SaveChanges();
            return equipmentType;
        }

        public static EquipmentMaintenance InsertEquipmentMaintenance(InternalContext context, Guid equipmentId, MaintenanceType maintenanceType, MaintenanceStatus status)
        {
            var equipmentMaintenance = new EquipmentMaintenance()
            {
                MaintenanceDate = new DateTime(2025, 04, 10),
                MaintenanceType = maintenanceType,
                Status = status,
                MaintenanceDateScheduled = new DateTime(2025, 09, 01),
                Note = "",
                EquipmentId = equipmentId,
            };
            context.EquipmentMaintenances.Add(equipmentMaintenance);
            context.SaveChanges();
            return equipmentMaintenance;
        }

        public static EquipmentDoc InsertEquipmentDoc(InternalContext context, Guid EquipmentId, string fileName = "prova.pdf", DocumentType documentType = DocumentType.UserAndMaintenanceManual)
        {
            var equipmentDoc = new EquipmentDoc()
            {
                FileName = fileName,
                FileType = documentType,
                Blob = new byte[10],
                EquipmentId = EquipmentId,
            };
            context.EquipmentDocs.Add(equipmentDoc);
            context.SaveChanges();
            return equipmentDoc;
        }

        public static EquipmentTool InsertEquipmentTool(InternalContext context, Guid EquipmentId, string code = "EO001", EquipmentToolStatus status = EquipmentToolStatus.Used, string description = "")
        {
            var equipmentTool = new EquipmentTool()
            {
                Code = code,
                Status = status,
                Description = description,
                EquipmentId = EquipmentId,
            };
            context.EquipmentTools.Add(equipmentTool);
            context.SaveChanges();
            return equipmentTool;
        }

        public static EquipmentMovement InsertEquipmentMovement(InternalContext context, Guid equipmentId, Guid locationFromId, Guid locationToId)
        {
            var equipmentMovement = new EquipmentMovement()
            {
                EquipmentId = equipmentId,
                LocationFromId = locationFromId,
                LocationToId = locationToId,
            };
            context.EquipmentMovements.Add(equipmentMovement);
            context.SaveChanges();
            return equipmentMovement;
        }

        /* ------------------------------ Equipment EmptyId ------------------------------ */

        public static void InsertEquipment_EmptyId(InternalContext context)
        {
            context.Equipment.Add(new Equipment()
            {
                EquipmentTypeId = Guid.Empty,
                Code = "ED001",
                Asset = "PC-25-07-000",
                YearAcquisition = 2020,
                Status = EquipmentStatus.InUse,
            });
            context.SaveChanges();
        }

        public static void InsertEquipmentTool_EmptyId(InternalContext context)
        {
            context.EquipmentTools.Add(new EquipmentTool()
            {
                EquipmentId = Guid.Empty,
                Code = "EO001",
                Status = EquipmentToolStatus.Used,
            });
            context.SaveChanges();
        }

        public static void InsertEquipmentMaintenance_EmptyId(InternalContext context)
        {
            context.EquipmentMaintenances.Add(new EquipmentMaintenance()
            {
                EquipmentId = Guid.Empty,
                MaintenanceType = MaintenanceType.RegularMaintenance,
                Status = MaintenanceStatus.ScheduledMaintenance,
            });
            context.SaveChanges();
        }

        /* ------------------------------ WorkShift ------------------------------ */

        public static WorkShiftDay InsertWorkShiftDay(InternalContext context, DateTime? dayWorkShift = null)
        {
            var workShiftDay = new WorkShiftDay()
            {
                DayWorkShift = dayWorkShift ?? DateTime.Today,
                Description = "Work Shift Day Description",
                EnableMode = ConfigEnableMode.Enabled,

            };
            context.WorkShiftDays.Add(workShiftDay);
            context.SaveChanges();
            return workShiftDay;
        }

        public static WorkShift InsertWorkShift(InternalContext context, Guid workShiftDayId, WorkShiftType workShiftType = WorkShiftType.FullDay)
        {
            var workShift = new WorkShift()
            {
                WorkShiftDayId = workShiftDayId,
                WorkShiftType = workShiftType,
                Sequence = 1,
                EnableMode = ConfigEnableMode.Enabled,
                StartSeconds = (int)new TimeSpan(8, 0, 0).TotalSeconds,
                StopSeconds = (int)new TimeSpan(16, 0, 0).TotalSeconds,
                Target = 480 // 8 hours in minutes
            };
            context.WorkShifts.Add(workShift);
            context.SaveChanges();
            return workShift;
        }

        public static WorkShiftBreak InsertWorkShiftBreak(InternalContext context, Guid workShiftId)
        {
            var workShiftBreak = new WorkShiftBreak()
            {
                WorkShiftId = workShiftId,
                StartSeconds = (int)new TimeSpan(12, 0, 0).TotalSeconds,
                StopSeconds = (int)new TimeSpan(13, 0, 0).TotalSeconds,
                EnableMode = ConfigEnableMode.Enabled,
                Sequence = 1
            };
            context.WorkShiftBreaks.Add(workShiftBreak);
            context.SaveChanges();
            return workShiftBreak;
        }

        /* ------------------------------ Location ------------------------------ */

        public static StorageUnitType InsertStorageUnitType(InternalContext context)
        {
            var storageUnitType = new StorageUnitType()
            {
                Code = "SUT01",
                Description = "StorageUnitType 1"
            };
            context.StorageUnitTypes.Add(storageUnitType);
            context.SaveChanges();
            return storageUnitType;
        }

        public static StorageUnit InsertStorageUnit(InternalContext context, Guid storageUnitTypeId)
        {
            var storageUnit = new StorageUnit()
            {
                Code = "SU01",
                Description = "StorageUnit 1",
                StorageUnitTypeId = storageUnitTypeId,
            };
            context.StorageUnits.Add(storageUnit);
            context.SaveChanges();
            return storageUnit;
        }

        public static Site InsertSite(InternalContext context)
        {
            var site = new Site
            {
                Code = "S01",
                Description = "Mch",
            };
            context.Sites.Add(site);
            context.SaveChanges();
            return site;
        }

        public static Warehouse InsertWarehouse(InternalContext context, Guid siteId)
        {
            var warehouse = new Warehouse
            {
                Code = "W01",
                Description = "Warehouse 1",
                SiteId = siteId,
            };
            context.Warehouses.Add(warehouse);
            context.SaveChanges();
            return warehouse;
        }

        public static Area InsertArea(InternalContext context, Guid areaTypeId, Guid warehouseId)
        {
            var area = new Area()
            {
                Code = "A01",
                Description = "Area 1",
                AreaTypeId = areaTypeId,
                WarehouseId = warehouseId,
            };
            context.Areas.Add(area);
            context.SaveChanges();
            return area;
        }

        public static AreaType InsertAreaType(InternalContext context)
        {
            var areaType = new AreaType
            {
                Code = "AT01",
                Description = "AreaType"
            };
            context.AreaTypes.Add(areaType);
            context.SaveChanges();
            return areaType;
        }

        public static Location InsertLocation(InternalContext context, Guid locationTypeId, Guid? areaId = null)
        {
            var location = new Location()
            {
                Code = "L01",
                Description = "Location 1",
                LocationTypeId = locationTypeId,
                AreaId = areaId,
            };
            context.Locations.Add(location);
            context.SaveChanges();
            return location;
        }

        public static LocationType InsertLocationType(InternalContext context)
        {
            var locationType = new LocationType()
            {
                Code = "LT01",
                Description = "LocationType",
            };
            context.LocationTypes.Add(locationType);
            context.SaveChanges();
            return locationType;
        }
    }
}
