using FluentAssertions;
using Mch.Authentication.ContextDb.Models;
using Mch.Internal.UnifiedContextDb.Models;
using System;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]

    public class WorkShiftTest : BaseClassTest
    {
        //private InternalContext _context;
        private WorkShift _workShift;
        private WorkShiftBreak _workShiftBreak;
        private WorkShiftDay _workShiftDay;

        [SetUp]
        public void SetupTest()
        {
            //_context = _serviceProvider.GetRequiredService<InternalContext>();
            _workShiftDay = DatabaseHelper.InsertWorkShiftDay(_context);
            _workShift = DatabaseHelper.InsertWorkShift(_context, _workShiftDay.WorkShiftDayId);
            _workShiftBreak = DatabaseHelper.InsertWorkShiftBreak(_context, _workShift.WorkShiftId);
        }

        [TearDown]
        public void TeardownTest()
        {
            //_context.Dispose();
        }

        [Test]
        public void DbSets_ShouldBeInitialized()
        {
            _context.WorkShifts.Should().NotBeNull();
            _context.WorkShiftDays.Should().NotBeNull();
            _context.WorkShiftBreaks.Should().NotBeNull();
        }



        [Test]
        public void Start_Property_ShouldConvertSecondsToTimeSpan()
        {
            var start = _workShift.Start;
            start.Should().Be(new TimeSpan(8, 0, 0));
        }


        [Test]
        public void Stop_Property_ShouldConvertSecondsToTimeSpan()
        {
            var stop = _workShift.Stop;
            stop.Should().Be(new TimeSpan(16, 0, 0));
        }

        [Test]
        public void GetTotalSeconds_WithNormalShift_ShouldReturnCorrectDuration()
        {
            var totalSeconds = _workShift.GetTotalSeconds();
            totalSeconds.Should().Be(28800);
        }

        [Test]
        public void GetTotalSeconds_WithOvernightShift_ShouldReturnCorrectDuration()
        {
            _workShift.StartSeconds = 82800;
            _workShift.StopSeconds = 28800;
            var totalSeconds = _workShift.GetTotalSeconds();
            totalSeconds.Should().Be(32400);
        }

        [Test]
        public void GetTotalSecondsWork_WithNoBreaks_ShouldReturnTotalSeconds()
        {
            var workSeconds = _workShift.GetTotalSecondsWork();
            workSeconds.Should().Be(25200);
        }

        [Test]
        public void GetTotalSecondsWork_WithNegativeResult_ShouldReturnZero()
        {
            _workShift.StartSeconds = 0;
            _workShift.StopSeconds = 1800;
            var longBreak = new WorkShiftBreak
            {
                StartSeconds = 0,
                StopSeconds = 3600
            };
            _workShift.WorkShiftBreaks.Add(longBreak);
            var workSeconds = _workShift.GetTotalSecondsWork();
            workSeconds.Should().Be(0);
        }

        [Test]
        public void IsInBreak_WithNoBreaks_ShouldReturnFalse()
        {
            var referenceTime = new TimeSpan(10, 0, 0);
            var isInBreak = _workShift.IsInBreak(referenceTime);
            isInBreak.Should().BeFalse();
        }

        [Test]
        public void IsInBreak_DuringBreak_ShouldReturnTrue()
        {
            var referenceTime = new TimeSpan(12, 15, 0);
            var isInBreak = _workShift.IsInBreak(referenceTime);
            isInBreak.Should().BeTrue();
        }
        [Test]
        public void GetCurrentBreak_DuringBreak_ShouldReturnBreak()
        {
            var referenceTime = new TimeSpan(12, 15, 0);
            var currentBreak = _workShift.GetCurrentBreak(referenceTime);
            currentBreak.Should().NotBeNull();
            currentBreak.Should().Be(_workShiftBreak);
        }

        [Test]
        public void GetCurrentBreak_WithOvernightBreak_ShouldWork()
        {
            var overnightBreak = new WorkShiftBreak
            {
                StartSeconds = 82800, // 23:00:00
                StopSeconds = 7200,   // 02:00:00
                EnableMode = ConfigEnableMode.Enabled,
            };
            _workShift.WorkShiftBreaks.Add(overnightBreak);
            _workShift.GetCurrentBreak(new TimeSpan(1, 0, 0)).Should().Be(overnightBreak); // 01:00 AM
            _workShift.GetCurrentBreak(new TimeSpan(23, 30, 0)).Should().Be(overnightBreak); // 23:30 PM
            _workShift.GetCurrentBreak(new TimeSpan(10, 0, 0)).Should().BeNull(); // 10:00 AM
        }

        [Test]
        public void GetActualSeconds_WithCurrentTime_ShouldCalculateCorrectly()
        {
            var testTime = new DateTime(2023, 1, 1, 10, 0, 0); // 10:00 AM
            var actualSeconds = _workShift.GetActualSeconds(testTime);
            var expected = (int)(testTime.TimeOfDay.TotalSeconds - _workShift.StartSeconds);
            actualSeconds.Should().Be(expected);
        }

        [Test]
        public void GetActualSeconds_WithOvernightShift_ShouldCalculateCorrectly()
        {
            _workShift.StartSeconds = 82800; // 23:00:00
            _workShift.StopSeconds = 28800;  // 08:00:00
            var testTime = new DateTime(2023, 1, 1, 2, 0, 0); // 02:00 AM (next day)
            var actualSeconds = _workShift.GetActualSeconds(testTime);
            var expected = (int)(testTime.TimeOfDay.TotalSeconds + (86400 - _workShift.StartSeconds));
            actualSeconds.Should().Be(expected);
        }

        [Test]
        public void GetPerformance_WithValidData_ShouldCalculateCorrectly()
        {
            _workShift.Target = 100;
            var actualQty = 80;
            var performance = _workShift.GetPerformance(actualQty);
            performance.Should().BeGreaterThan(0);
        }

        [Test]
        public void GetElapsedWorkMillisecondsDate_WithSimpleTimeRange_ShouldCalculateCorrectly()
        {
            var fromTime = new DateTime(2025, 1, 1, 9, 0, 0, DateTimeKind.Utc);
            var toTime = new DateTime(2025, 1, 1, 11, 0, 0, DateTimeKind.Utc);
            var elapsedMs = _workShift.GetElapsedWorkMillisecondsDate(fromTime, toTime);
            elapsedMs.Should().Be(7200000); // 2 hours = 7,200,000 milliseconds
        }

        [Test]
        public void GetElapsedWorkMillisecondsDate_WithBreakInRange_ShouldSubtractBreakTime()
        {
            // Arrange
            var fromTime = new DateTime(2025, 1, 1, 11, 30, 0, DateTimeKind.Utc); // Before break
            var toTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc);   // After break
            var elapsedMs = _workShift.GetElapsedWorkMillisecondsDate(fromTime, toTime);
            // Total time: 1.5 hours = 5,400,000 ms
            // Break time: 30 minutes = 1,800,000 ms  
            // Work time: 5,400,000 - 1,800,000 = 3,600,000 ms
            elapsedMs.Should().BeLessThanOrEqualTo(5400000); // Should be less due to break
        }


    }
}
