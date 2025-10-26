using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class ArticleTest : BaseClassTest
    {

        [Test, Order(1)]
        public void DbSets_ShouldBeInitialized()
        {
            _context.ArticleTypes.Should().NotBeNull();
            _context.Articles.Should().NotBeNull();
        }

        [Test, Order(2)]
        public void ArtticleType_Article_Insert_Should_Be_No_Exception()
        {
            var art = _fakeDataGenerate.GenerateArticles();
            Assert.Multiple(() =>
            {
                Assert.That(art, Is.Not.Null);
                Assert.That(art.Count, Is.EqualTo(50));
                Assert.That(art.Select(x => x.ArticleId).Distinct().Count(), Is.EqualTo(50));
                Assert.That(art.Select(x => x.ArticleTypeId).Distinct().Count(), Is.EqualTo(5));
            });
        }

        #region ArticleType
        [Test]
        public void ArtticleType_Insert_Duplicate_Code_Should_Be_Excemption()
        {
            SetCulture();

            var art = _fakeDataGenerate.GenerateArticleTypes();
            _context.ArticleTypes.Add(new Models.ArticleType
            {
                Code = art[3].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30)
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "ArticleTypes", "IX_ArticleType_Code");
        }

        [Test]
        public void ArtticleType_Insert_Duplicate_Id_Should_Be_Excemption()
        {
            SetCulture();

            var art = _fakeDataGenerate.GenerateArticleTypes();
            _context.ArticleTypes.Add(art[1]);

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertPrimaryKey(ex, "ArticleTypes", "PK_ArticleType");
        }

        [Test]
        [TestCase("Code", 51, 20)]
        [TestCase("Description", 10, 251)]
        public void ArtticleType_Insert_Field_ExtraLength_Should_Be_Excemption(string columnName, int codeLen, int descriptionLen)
        {
            SetCulture();

            //var art = _fakeDataGenerate.GenerateArticleTypes();
            _context.ArticleTypes.Add(new Models.ArticleType
            {
                Code = _fakeDataGenerate.FakerGenerate.Random.String2(codeLen),
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(descriptionLen)
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertStringLength(ex, "ArticleTypes", columnName);
        }

        [Test]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public void ArtticleType_Insert_Field_Null_Should_Be_Excemption(string columnName, string? code, string? description)
        {
            SetCulture();

            _context.ArticleTypes.Add(new Models.ArticleType
            {
                Code = code,
                Description = description
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "ArticleTypes", columnName);
        }
        #endregion

        #region ArticleType
        [Test]
        public void Artticle_Insert_Duplicate_Id_Should_Be_Excemption()
        {
            SetCulture();

            CultureInfo current = Thread.CurrentThread.CurrentUICulture;
            var art = _fakeDataGenerate.GenerateArticles();
            _context.Articles.Add(art[1]);

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertPrimaryKey(ex, "Articles", "PK_Article");
        }

        [Test]
        public void Artticle_Insert_Duplicate_Code_Should_Be_Excemption()
        {
            SetCulture();

            var art = _fakeDataGenerate.GenerateArticles();
            _context.Articles.Add(new Models.Article
            {
                Code = art[3].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30),
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "Articles", "IX_Article_Code");
        }

        [Test]
        [TestCase("Code", 51, 20)]
        [TestCase("Description", 10, 251)]
        public void Artticle_Insert_Field_ExtraLength_Should_Be_Excemption(string columnName, int codeLen, int descriptionLen)
        {
            SetCulture();

            _context.Articles.Add(new Models.Article
            {
                Code = _fakeDataGenerate.FakerGenerate.Random.String2(codeLen),
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(descriptionLen),
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertStringLength(ex, "Articles", columnName);
        }

        [Test]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public void Artticle_Insert_Field_Null_Should_Be_Excemption(string columnName, string? code, string? description)
        {
            SetCulture();

            _context.Articles.Add(new Models.Article
            {
                Code = code,
                Description = description
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "Articles", columnName);
        }

        [Test]
        [TestCase("FK_Article_ArticleType", "ArticleTypes", "ArticleTypeId", "00000000-0000-0000-0000-000000000000")]
        [TestCase("FK_Article_ArticleType", "ArticleTypes", "ArticleTypeId", "cab23c71-c2e5-49cd-b752-d745302fde99")]
        public void Artticle_Insert_ForeignKey_Invalid_Should_Be_Excemption(string foreignKeyName, string tableName, string columnName, string id)
        {
            SetCulture();
            var art = _fakeDataGenerate.GenerateArticles();
            _context.Articles.Add(new Models.Article
            {
                Code = _fakeDataGenerate.FakerGenerate.Random.String2(10),
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(10),
                ArticleTypeId = Guid.Parse(id)
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertForeignKeyIvalid(ex, foreignKeyName, tableName, columnName);
        }
        #endregion
    }
}
