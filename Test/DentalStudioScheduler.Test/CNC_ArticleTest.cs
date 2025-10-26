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
    public class CNC_ArticleTest : BaseClassTest
    {


        [Test, Order(1)]
        public void DbSets_ShouldBeInitialized()
        {
            _context.CNC_Articles.Should().NotBeNull();
            _context.CNC_ArticleDocs.Should().NotBeNull();

            _context.CNC_ArticleLogs.Should().NotBeNull();
            _context.CNC_ArticleDocLogs.Should().NotBeNull();
        }

        [Test, Order(2)]
        public void CNC_Article_Insert_Should_Be_No_Exception()
        {
            var dt = _fakeDataGenerate.GenerateCNCArticles();
            Assert.Multiple(() =>
            {
                Assert.That(dt, Is.Not.Null);
                Assert.That(dt.Count, Is.EqualTo(50));
            });
        }

        [Test]
        public void CNC_Article_Insert_Duplicate_Id_Should_Be_Excemption()
        {
            SetCulture();

            var dt = _fakeDataGenerate.GenerateCNCArticles();
            _context.CNC_Articles.Add(dt[1]);

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertPrimaryKey(ex, "CNC_Articles", "PK_CNC_Article");
        }

        [Test]
        public void CNC_ArticleDoc_Insert_Duplicate_Id_Should_Be_Excemption()
        {
            SetCulture();

            var dt = _fakeDataGenerate.GenerateCNCArticles()
                .Where(x => x.CNC_ArticleDocs.Count > 0)
                .Select(x => x.CNC_ArticleDocs)
                .First()
                .First();
            dt.CNC_Article = null;
            _context.CNC_ArticleDocs.Add(dt);

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertPrimaryKey(ex, "CNC_ArticleDocs", "PK_CNC_ArticleDoc");
        }


        [Test, Category("CNC_Article is Empty")]
        public void Empty_CNC_Article_ShouldSucceedAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001");
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault(x => x.DrawingName == "ED002");
            var cncArticleDocLogs_ = context.CNC_ArticleDocLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNull(cncArticles_);
                Assert.IsNull(cncArticleDocLogs_);
            });
        }

        [Test, Category("CNC_ArticleDoc is Empty")]
        public void Empty_CNC_ArticleDoc_ShouldSucceedAsync()
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                var cncArt = DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001");
                DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, "prova", DocumentType.Invoice);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleDocs_ = context.CNC_ArticleDocs.FirstOrDefault(x => x.FileName == "prova2");
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.IsNull(cncArticleDocs_);
            });
        }

        #region Insert CNC_Article

        [Test, Category("Insert CNC_Article")]
        [TestCase(null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes")]
        public void Insert_CNC_Article_ShouldSucceedAsync(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes)
        {
            string drawingName = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName, sourceMaterial, programName, programContent, realizationNotes);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, drawingName);
            });
        }

        [Test, Category("Insert Multy CNC_Article")]
        [TestCase(null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes")]
        public void InsertMulty_CNC_Article_ShouldSucceedAsync(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes)
        {
            string drawingName1 = "ED001";
            string drawingName2 = "ED002";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art1 = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                var art2 = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId, "Art02");
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art1.ArticleId, drawingName1, sourceMaterial, programName, programContent, realizationNotes);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art2.ArticleId, drawingName2, sourceMaterial, programName, programContent, realizationNotes);
            }

            var cncArticles_ = context.CNC_Articles.ToList();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.Count, 2);

                var cncArt1 = cncArticles_.FirstOrDefault(x => x.DrawingName == drawingName1);
                var cncArt2 = cncArticles_.FirstOrDefault(x => x.DrawingName == drawingName2);
                Assert.IsNotNull(cncArt1);
                Assert.IsNotNull(cncArt2);
            });
        }

        [Test, Category("Insert Multy CNC_Article")]
        public void InsertMulty_CNC_Article_Should_Exception_DuplicateKey()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var cnc = DatabaseHelper.InsertCNC(context);
                    var artType = DatabaseHelper.InsertArticleType(context);
                    var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                    DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001", "sourceMaterial", "programName", "programContent", "realizationNotes");
                    DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED002", "sourceMaterial2", "programName2", "programContent2", "realizationNotes2");
                }
            });

            // Verifica opzionale sul messaggio
            AssertUniqueIndex(ex, "CNC_Articles", "IX_CNC_Article_CNC_Id_ArticleId");
        }
        #endregion

        #region Insert CNC_ArticleDoc

        [Test, Category("Insert CNC_ArticleDoc")]
        [TestCase(DocumentType.Invoice)]
        [TestCase(DocumentType.Generic_Undefined)]
        public void InsertMulty_CNC_ArticleDoc_ShouldSucceedAsync(DocumentType fileType)
        {
            var drawingName = "ED001";
            var fileName = "prova.pdf";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                var cncArt = DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName);
                DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, fileName, fileType);
            }

            var cncArticles_ = context.CNC_Articles
                .Include(x => x.CNC_ArticleDocs)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, drawingName);
                Assert.AreEqual(cncArticles_.CNC_ArticleDocs.Count, 1);
                var doc1 = cncArticles_.CNC_ArticleDocs.FirstOrDefault();
                Assert.IsTrue(doc1.FileType == fileType);
            });
        }

        [Test, Category("Insert Multy CNC_ArticleDoc")]
        [TestCase(DocumentType.Documentation, DocumentType.Invoice)]
        [TestCase(DocumentType.Invoice, DocumentType.Documentation)]
        public void Insert_CNC_MultyArticleDoc_ShouldSucceedAsync(DocumentType fileType1, DocumentType fileType2)
        {
            var drawingName = "ED001";
            var fileName1 = "prova.pdf";
            var fileName2 = "prova.doc";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                var cncArt = DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName);
                DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, fileName1, fileType1);
                DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, fileName2, fileType2);
            }

            var cncArticles_ = context.CNC_Articles
                .Include(x => x.CNC_ArticleDocs)
                .FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, drawingName);

                Assert.AreEqual(cncArticles_.CNC_ArticleDocs.Count, 2);
                var doc1 = cncArticles_.CNC_ArticleDocs.FirstOrDefault(x => x.FileName == fileName1);
                var doc2 = cncArticles_.CNC_ArticleDocs.FirstOrDefault(x => x.FileName == fileName2);
                Assert.IsTrue(doc1.FileType == fileType1);
                Assert.IsTrue(doc2.FileType == fileType2);
            });
        }
        #endregion

        #region Insert e Edit paramiters for CNC_Article
        [Test, Category("Insert and Edit CNC_Article - DrawingNameChange")]
        [TestCase(null, null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_DrawingNameChange_ShouldSucceedAsync(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes,
                                                                                string? logDescription)
        {
            string origin = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, origin, sourceMaterial, programName, programContent, realizationNotes);
            }

            string edit = "ED002";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = edit,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.DrawingName, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - SourceMaterial")]
        [TestCase(null, null, null, null)]
        [TestCase("programName", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_SourceMaterial_ShouldSucceedAsync(string? programName, string? programContent, string? realizationNotes,
                                                                             string? logDescription)
        {
            string origin = "Inox";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001", origin, programName, programContent, realizationNotes);
            }

            string edit = "Accaio";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = edit,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.BaseMaterial, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.BaseMaterial, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - ProgramName")]
        [TestCase(null, null, null, null)]
        [TestCase("sourceMaterial", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_ProgramName_ShouldSucceedAsync(string? sourceMaterial, string? programContent, string? realizationNotes,
                                                                          string? logDescription)
        {
            string origin = "ProgrammaOriginale";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001", sourceMaterial, origin, programContent, realizationNotes);
            }

            string edit = "ProgrammaModificato";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = edit,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.ProgramName, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.ProgramName, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - ProgramContent")]
        [TestCase(null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_ProgramContent_ShouldSucceed(string? sourceMaterial, string? programName, string? realizationNotes,
                                                                        string? logDescription)
        {
            string origin = "ProgrammaOriginale";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001", sourceMaterial, programName, origin, realizationNotes);
            }

            string edit = "ProgrammaModificato";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = edit,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.ProgramContent, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.ProgramContent, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - ProgrammingFirstTime")]
        [TestCase(null, null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_ProgrammingFirstTime_ShouldSucceed(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes,
                                                                              string? logDescription)
        {
            var origin = new TimeSpan(05, 04, 10);
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article_ProgrammingFirstTime(context, cnc.CNC_Id, art.ArticleId, "ED001", origin, sourceMaterial, programName, programContent, realizationNotes);
            }

            var edit = new TimeSpan(10, 10, 10);
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = edit,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.ProgrammingFirstTime, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.ProgrammingFirstTime, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - ProgrammingTime")]
        [TestCase(null, null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_ProgrammingTime_ShouldSucceed(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes,
                                                                         string? logDescription)
        {
            var origin = new TimeSpan(05, 04, 10);
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article_ProgrammingTime(context, cnc.CNC_Id, art.ArticleId, "ED001", origin, sourceMaterial, programName, programContent, realizationNotes);
            }

            var edit = new TimeSpan(07, 04, 10);
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = edit,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.ProgrammingTime, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.ProgrammingTime, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - SetupTime")]
        [TestCase(null, null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_SetupTime_ShouldSucceed(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes,
                                                                   string? logDescription)
        {
            var origin = new TimeSpan(05, 04, 10);
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article_SetupTime(context, cnc.CNC_Id, art.ArticleId, "ED001", origin, sourceMaterial, programName, programContent, realizationNotes);
            }

            var edit = new TimeSpan(07, 04, 10);
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = edit,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                    ArticleId = cncArticleOld.ArticleId,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.SetupTime, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.SetupTime, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - ProductionTime")]
        [TestCase(null, null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes", "logDescription")]
        public void Edit_CNC_Article_ProductionTime_ShouldSucceed(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes,
                                                                        string? logDescription)
        {
            var origin = new TimeSpan(05, 04, 10);
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article_ProductionTime(context, cnc.CNC_Id, art.ArticleId, "ED001", origin, sourceMaterial, programName, programContent, realizationNotes);
            }

            var edit = new TimeSpan(07, 04, 10);
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = edit,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.ProductionTime, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.ProductionTime, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_Article - RealizationNotes")]
        [TestCase(null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "logDescription")]
        public void Edit_CNC_Article_RealizationNotes_ShouldSucceed(string? sourceMaterial, string? programName, string? programContent,
                                                                          string? logDescription)
        {
            var origin = "vuoto";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "ED001", sourceMaterial, programName, programContent, origin);
            }

            var edit = "modifica";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = edit,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.RealizationNotes, edit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.RealizationNotes, origin);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }


        [Test, Category("Insert and Edit CNC_Article - Multy Paramiters")]
        [TestCase(null)]
        [TestCase("logDescription")]
        public void EditMultyParamiters_CNC_Article_ShouldSucceed(string? logDescription)
        {
            string drawingName = "ED001", programName = "programName", realizationNotes = "vuoto";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName, "sourceMaterial", programName, "programContent", realizationNotes);
            }

            string drawingNameEdit = "ED002", programNameEdit = "programNameEdit", realizationNotesEdit = "modifica";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = drawingNameEdit,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = programNameEdit,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = realizationNotesEdit,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, drawingNameEdit);
                Assert.AreEqual(cncArticles_.ProgramName, programNameEdit);
                Assert.AreEqual(cncArticles_.RealizationNotes, realizationNotesEdit);

                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.DrawingName, drawingName);
                Assert.AreEqual(cncArticleLogs_.ProgramName, programName);
                Assert.AreEqual(cncArticleLogs_.RealizationNotes, realizationNotes);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleAdded);
            });
        }


        [Test, Category("Insert and Edit CNC_Article - Multy Edit")]
        [TestCase(null, null, null, null, null)]
        [TestCase("sourceMaterial", "programName", "programContent", "realizationNotes", "logDescription")]
        public void EditMulty_CNC_Article_ShouldSucceed(string? sourceMaterial, string? programName, string? programContent, string? realizationNotes,
                                                                  string? logDescription)
        {
            string drawingName = "ED001";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName, sourceMaterial, programName, programContent, "");
            }

            string drawingNameEdit1 = "ED002";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = drawingNameEdit1,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = cncArticleOld.RealizationNotes,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            string realizationNotesEdit = "drawingNameEdit = ED002";
            {
                var cncArticleOld = context.CNC_Articles
                    .Include(x => x.CNC)
                    .FirstOrDefault();
                var cncArticleEdit = new CNC_Article()
                {
                    DrawingName = cncArticleOld.DrawingName,
                    BaseMaterial = cncArticleOld.BaseMaterial,
                    ProgramName = cncArticleOld.ProgramName,
                    ProgramContent = cncArticleOld.ProgramContent,
                    ProgrammingFirstTime = cncArticleOld.ProgrammingFirstTime,
                    ProgrammingTime = cncArticleOld.ProgrammingTime,
                    SetupTime = cncArticleOld.SetupTime,
                    ProductionTime = cncArticleOld.ProductionTime,
                    RealizationNotes = realizationNotesEdit,
                    CNC_Id = cncArticleOld.CNC_Id,
                };
                DatabaseHelper.InsertCNC_ArticleLog(context, cncArticleOld, cncArticleEdit, logDescription);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.ToList();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, drawingNameEdit1);
                Assert.AreEqual(cncArticles_.RealizationNotes, realizationNotesEdit);

                Assert.AreEqual(cncArticleLogs_.Count, 2);
                var cncArtLog1 = cncArticleLogs_.FirstOrDefault(x => x.DrawingName == drawingName) ?? new CNC_ArticleLog();
                var cncArtLog2 = cncArticleLogs_.FirstOrDefault(x => x.DrawingName == drawingNameEdit1) ?? new CNC_ArticleLog();
                Assert.IsTrue(cncArtLog1.DrawingName == drawingName && cncArtLog1.CncArticleOperationType == CncArticleOperationType.CncArticleAdded);
                Assert.IsTrue(cncArtLog2.DrawingName == drawingNameEdit1 && cncArtLog2.CncArticleOperationType == CncArticleOperationType.CncArticleChange);
            });
        }
        #endregion

        #region Insert and Edit CNC_ArticleDoc

        [Test, Category("Insert and Edit CNC_ArticleDoc")]
        [TestCase(DocumentType.Generic_Undefined)]
        [TestCase(DocumentType.Invoice)]
        public void Edit_CNC_ArticleDoc_ShouldSucceedAsync(DocumentType fileType)
        {
            var drawingName = "ED001";
            var original = "prova.pdf";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                var cncArt = DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName);
                DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, original, fileType);
            }

            var edit = "prova2.pdf";
            {
                var cncArticleDoc = context.CNC_ArticleDocs.FirstOrDefault();
                var cncArticleDocEdit = new CNC_ArticleDoc()
                {
                    FileName = edit,
                    FileType = cncArticleDoc.FileType,
                    Blob = cncArticleDoc.Blob,
                };
                DatabaseHelper.InsertCNC_ArticleDocLog(context, cncArticleDoc, cncArticleDocEdit);
            }

            var cncArticles_ = context.CNC_Articles
                .Include(x => x.CNC_ArticleDocs)
                .FirstOrDefault();
            var cncArticleDocLogs_ = context.CNC_ArticleDocLogs
                .ToList();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(cncArticles_);
                Assert.AreEqual(cncArticles_.DrawingName, drawingName);

                Assert.AreEqual(cncArticles_.CNC_ArticleDocs.Count, 1);
                var doc1 = cncArticles_.CNC_ArticleDocs.FirstOrDefault();
                Assert.IsTrue(doc1.FileType == fileType);

                Assert.AreEqual(cncArticleDocLogs_.Count, 1);
                var docLog = cncArticleDocLogs_.FirstOrDefault(x => x.FileName == original);

                Assert.IsNotNull(docLog);
                Assert.AreEqual(docLog.CncArticleDocOperationType, CncArticleDocOperationType.CncArticleDocAdded);
            });
        }

        [Test, Category("Insert and Edit CNC_ArticleDoc")]
        public void Edit_CNC_ArticleDoc_Should_Exception_DuplicateKey()
        {
            var ex = Assert.Throws<Exception>(() =>
            {
                var drawingName = "ED001";
                var original = "prova.pdf";
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                {
                    var cnc = DatabaseHelper.InsertCNC(context);
                    var artType = DatabaseHelper.InsertArticleType(context);
                    var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                    var cncArt = DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName);
                    DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, original, DocumentType.Generic_Undefined);
                    DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, original, DocumentType.Generic_Undefined);
                }
            });

            // Verifica opzionale sul messaggio
            AssertUniqueIndex(ex, "CNC_ArticleDocs", "IX_CNC_ArticleDoc_CNC_ArticleId_FileName");
        }
        #endregion

        #region Insert and Deleted CNC_Article 

        [Test, Category("Insert and Deleted CNC_Article")]
        [TestCase("sourceMaterial", null, null)]
        [TestCase("sourceMaterial", "programName", null)]
        [TestCase("sourceMaterial", null, "ED001")]
        [TestCase(null, "programName", null)]
        [TestCase(null, "programName", "ED001")]
        [TestCase(null, null, "ED001")]
        [TestCase("sourceMaterial", "programName", "ED001")]
        public void Deleted_CNC_Article_ShouldSucceedAsync(string? sourceMaterial, string? programName, string? drawingName)
        {
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, drawingName, sourceMaterial, programName);
                DatabaseHelper.DeleteCNC_Article(context, drawingName, sourceMaterial, programName);
            }

            var cncArticles_ = context.CNC_Articles.FirstOrDefault();
            var cncArticleLogs_ = context.CNC_ArticleLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNull(cncArticles_);
                Assert.IsNotNull(cncArticleLogs_);
                Assert.AreEqual(cncArticleLogs_.CncArticleOperationType, CncArticleOperationType.CncArticleDeleted);
            });
        }
        #endregion

        #region Insert and Deleted CNC_ArticleDoc

        [Test, Category("Insert and Deleted CNC_ArticleDoc")]
        [TestCase(DocumentType.Invoice)]
        [TestCase(DocumentType.Documentation)]
        public void Deleted_CNC_ArticleDoc_ShouldSucceedAsync(DocumentType fileType)
        {
            var fileName = "prova";
            using var context = _serviceProvider.GetRequiredService<InternalContext>();
            {
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                var cncArt = DatabaseHelper.InsertCNC_Article(context, cnc.CNC_Id, art.ArticleId, "drawingName");
                DatabaseHelper.InsertCNC_ArticleDoc(context, cncArt.CNC_ArticleId, fileName, fileType);
                DatabaseHelper.DeleteCNC_ArticleDoc(context, fileType, fileName);
            }

            var cncArticleDoc_ = context.CNC_ArticleDocs.FirstOrDefault();
            var cncArticleDocLogs_ = context.CNC_ArticleDocLogs.FirstOrDefault();
            Assert.Multiple(() =>
            {
                Assert.IsNull(cncArticleDoc_);
                Assert.IsNotNull(cncArticleDocLogs_);
                Assert.AreEqual(cncArticleDocLogs_.CncArticleDocOperationType, CncArticleDocOperationType.CncArticleDocDeleted);
            });
        }
        #endregion

        #region CNC_Article Time Validate

        [Test, Category("Insert Time Values from 0 to 120000 seconds")]
        [TestCase(0)]
        [TestCase(600)]
        [TestCase(6000)]
        [TestCase(12000)]
        [TestCase(60000)]
        [TestCase(120000)]
        public void Insert_CNC_Article_With_Time_Value_Over24h_Shold_No_Exception(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            int length = 4;
            ArticleType artType = new ArticleType();
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                artType = DatabaseHelper.InsertArticleType(context);
            }
            for (int i = 0; i < length; i++)
            {
                Guid cncId = Guid.Empty;
                Guid articleId = Guid.Empty;
                {
                    using var context = _serviceProvider.GetRequiredService<InternalContext>();
                    var cnc1 = DatabaseHelper.InsertCNC(context, $"CNC_{i}");
                    var art1 = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId, $"Code_{i}");
                    cncId = cnc1.CNC_Id;
                    articleId = art1.ArticleId;
                }
                {
                    using var ctxI = _serviceProvider.GetRequiredService<InternalContext>();
                    ctxI.CNC_Articles.Add(new CNC_Article
                    {
                        ArticleId = articleId,
                        CNC_Id = cncId,
                        ProgrammingFirstTime = timeSpan
                    });
                    ctxI.SaveChanges();
                }
            }

            using var ctx = _serviceProvider.GetRequiredService<InternalContext>();

            int numRec = ctx.CNC_Articles.Count();
            var s = TimeSpan.FromSeconds(ctx.CNC_Articles.Sum(x => x.ProgrammingFirstTimeInSeconds));
            Assert.That(s, Is.EqualTo(TimeSpan.FromSeconds(seconds * numRec)));
        }

        [Test, Category("Check time value")]
        [TestCase(0)]
        [TestCase(600)]
        [TestCase(6000)]
        [TestCase(12000)]
        [TestCase(60000)]
        [TestCase(120000)]
        public void Insert_CNC_Article_With_Time_Value_Shold_Equal_In_Read(int seconds)
        {
            TimeSpan simeSp = TimeSpan.FromSeconds(seconds);
            Guid cncId = Guid.Empty;
            Guid articleId = Guid.Empty;
            {
                using var context = _serviceProvider.GetRequiredService<InternalContext>();
                var cnc = DatabaseHelper.InsertCNC(context);
                var artType = DatabaseHelper.InsertArticleType(context);
                var art = DatabaseHelper.InsertArticle(context, artType.ArticleTypeId);
                cncId = cnc.CNC_Id;
                articleId = art.ArticleId;
            }
            using var ctx = _serviceProvider.GetRequiredService<InternalContext>();
            ctx.CNC_Articles.Add(new CNC_Article
            {
                ArticleId = articleId,
                CNC_Id = cncId,
                ProgrammingFirstTime = simeSp,
                ProductionTime = simeSp,
                SetupTime = simeSp,
                ProgrammingTime = simeSp
            });
            ctx.SaveChanges();

            var s = ctx.CNC_Articles.First();
            Assert.That(s.ProgrammingFirstTime, Is.EqualTo(simeSp));
            Assert.That(s.ProgrammingTime, Is.EqualTo(simeSp));
            Assert.That(s.SetupTime, Is.EqualTo(simeSp));
            Assert.That(s.ProgrammingTime, Is.EqualTo(simeSp));
        }
        #endregion
    }
}