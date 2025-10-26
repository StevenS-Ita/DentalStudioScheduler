using FluentAssertions;
using System;

namespace Mch.Internal.UnifiedContextDb.Test
{
    public class CNCTest : BaseClassTest
    {

        [Test, Order(1)]
        public void DbSets_ShouldBeInitialized()
        {
            _context.CNCs.Should().NotBeNull();
        }

        [Test, Order(2)]
        public void CNC_Insert_Should_Be_No_Exception()
        {
            var art = _fakeDataGenerate.GenerateCNCs();
            Assert.Multiple(() =>
            {
                Assert.That(art, Is.Not.Null);
                Assert.That(art.Count, Is.EqualTo(10));
            });
        }


        [Test]
        public void CNC_Insert_Duplicate_Code_Should_Be_Excemption()
        {
            SetCulture();

            var cNCs = _fakeDataGenerate.GenerateCNCs();
            _context.CNCs.Add(new Models.CNC
            {
                Code = cNCs[3].Code,
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(30)
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertUniqueIndex(ex, "CNCs", "IX_CNC_Code");
        }

        [Test]
        public void CNC_Insert_Duplicate_Id_Should_Be_Excemption()
        {
            SetCulture();

            var cNCs = _fakeDataGenerate.GenerateCNCs();
            _context.CNCs.Add(cNCs[1]);

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertPrimaryKey(ex, "CNCs", "PK_CNC");
        }

        [Test]
        [TestCase("Code", 51, 20)]
        [TestCase("Description", 10, 251)]
        public void CNC_Insert_Field_ExtraLength_Should_Be_Excemption(string columnName, int codeLen, int descriptionLen)
        {
            SetCulture();

            _context.CNCs.Add(new Models.CNC
            {
                Code = _fakeDataGenerate.FakerGenerate.Random.String2(codeLen),
                Description = _fakeDataGenerate.FakerGenerate.Random.String2(descriptionLen)
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertStringLength(ex, "CNCs", columnName);
        }

        [Test]
        [TestCase("Code", null, "Description")]
        [TestCase("Description", "Code", null)]
        public void CNC_Insert_Field_Null_Should_Be_Excemption(string columnName, string? code, string? description)
        {
            SetCulture();

            _context.CNCs.Add(new Models.CNC
            {
                Code = code,
                Description = description
            });

            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });

            AssertCannotInsertNull(ex, "CNCs", columnName);
        }
    }
}
