using FluentAssertions;
using Mch.Internal.UnifiedContextDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class ActivityTest : BaseClassTest
    {
        [Test, Order(1)]
        public void DbSets_ShouldBeInitialized()
        {
            _context.ActivityTypes.Should().NotBeNull();
            _context.Activities.Should().NotBeNull();
        }

        #region Insert
        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public async Task InsertActivityType_ValidActivityType_Should_Succeed(int numActivityType)
        {
            var activityTypes = _fakeDataGenerate.GenerateActivityTypes(numActivityType);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(activityTypes.Count, numActivityType);
                Assert.AreEqual(activityTypes.Select(x => x.ActivityTypeId).Distinct().Count(), numActivityType);
            });
        }

        [Test]
        [TestCase(5, 6, 1)]
        [TestCase(5, 6, 10)]
        public async Task InsertActivity_ValidActivity_Should_Succeed(int numCommissions, int numActivityType, int numActivityForType)
        {
            var activities = _fakeDataGenerate.GenerateActivities(numCommissions, numActivityType, numActivityForType);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(activities.Count, numActivityType * numActivityForType);
                Assert.AreEqual(activities.Select(x => x.ActivityId).Distinct().Count(), numActivityType * numActivityForType);
            });
        }

        [Test]
        public async Task InsertActivity_ValidActivityCommission_Should_Succeed()
        {
            var numActivityType = 6;
            _fakeDataGenerate.GenerateActivityCommissions(5, numActivityType, 1);
            using (var context = _serviceProvider.GetRequiredService<InternalContext>())
            {
                var activities = await context.Activities
                    .Include(a => a.Commissions)
                    .ToListAsync();
                Assert.Multiple(() =>
                {
                    Assert.NotNull(activities);
                    Assert.AreEqual(activities.Count, numActivityType);
                    Assert.IsTrue(activities[0].Commissions.Count >= 2 && activities[1].Commissions.Count >= 2);
                    Assert.IsTrue(activities[2].Commissions.Count == 1 && activities[3].Commissions.Count == 1 && activities[4].Commissions.Count == 1);
                    Assert.IsTrue(activities[5].Commissions.Count == 0);
                });
            }
        }

        [Test]
        public async Task InsertActivity_ValidParentActivity_Should_Succeed()
        {
            int numCommissions = 5, numActivityType = 6, numActivityForType = 1;
            var parentActivityId = _fakeDataGenerate.GenerateActivities(1, 1, 1).FirstOrDefault().ActivityId;
            var activities = _fakeDataGenerate.GenerateActivities(numCommissions, numActivityType, numActivityForType, parentActivityId);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(activities.Count, numActivityType * numActivityForType);
                var rand = new Random().Next(0, (numActivityType * numActivityForType) - 1);
                Assert.AreEqual(activities[rand].ParentActivityId, parentActivityId);
            });
        }
        #endregion

        #region Duplicete
        [Test]
        public async Task InsertActivityType_DuplicateActivityType_Should_BeExcemption()
        {
            var activityTypes = _fakeDataGenerate.GenerateActivityTypes();
            _context.ActivityTypes.Add(new ActivityType()
            {
                ActivityTypeId = activityTypes[0].ActivityTypeId,
                Code = activityTypes[0].Code,
                Description = activityTypes[0].Description,
            });
            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });
            AssertPrimaryKey(ex, "ActivityTypes", "PK_ActivityType");
        }

        [Test]
        public async Task InsertActivity_DuplicatePrimaryKeyActivity_Should_BeExcemption()
        {
            var activities = _fakeDataGenerate.GenerateActivities();
            _context.Activities.Add(new Activity()
            {
                ActivityId = activities[0].ActivityId,
                ActivityTypeId = activities[0].ActivityTypeId,
                Name = "Name",
                Status = 0,
                EstimateDuration = 0,
            });
            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });
            AssertPrimaryKey(ex, "Activities", "PK_Activity_ActivityId");
        }

        [Test]
        public async Task InsertActivity_DuplicateActivity_Should_BeExcemption()
        {
            var activities = _fakeDataGenerate.GenerateActivities();
            _context.Activities.Add(new Activity()
            {
                ActivityTypeId = activities[0].ActivityTypeId,
                Name = activities[0].Name,
                Status = 0,
                EstimateDuration = 0,
            });
            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });
            AssertUniqueIndex(ex, "Activities", "IX_Activities_Name");
        }
        #endregion

        #region Integry
        [Test]
        public async Task InsertActivityType_InvalidActivityType_Should_BeExcemption()
        {
            _fakeDataGenerate.GenerateActivities();
            _context.Activities.Add(new Activity()
            {
                ActivityTypeId = Guid.NewGuid(),
                Name = "Name",
                Status = 0,
                EstimateDuration = 0,
            });
            var ex = Assert.Throws<Exception>(() =>
            {
                _context.SaveChanges();
            });
            AssertForeignKeyIvalid(ex, "FK_Activity_ActivityType", "ActivityTypes", "ActivityTypeId");
        }
        #endregion
    }
}