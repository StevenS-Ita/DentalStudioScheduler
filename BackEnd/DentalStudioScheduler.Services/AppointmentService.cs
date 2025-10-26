using DentalStudioScheduler.Context;
using DentalStudioScheduler.Data.Extensions;
using DentalStudioScheduler.Data.Localization;
using DentalStudioScheduler.Data.Models;
using DentalStudioScheduler.Data.ViewModels.Filters;
using DentalStudioScheduler.Model;
using DentalStudioScheduler.Models;
using DentalStudioScheduler.Services.Base;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DentalStudioScheduler.Services
{
    public class AppointmentService
    {
        private int startHour = 8, startBreak = 12, endBreak = 13, endHour = 17;

        private readonly DentalStudioContext _context;

        public AppointmentService(DentalStudioContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentViewModel>> GetAllAppointmentAsync()
        {
            var appointments = await _context.Appointments
                .OrderBy(x => x.Date)
                .ToAppointmentVM()
                .ToListAsync();

            return appointments;
        }

        /* ------------------------------ Get by Id Async ---------------------------- */
        public async Task<AppointmentViewModel> GetAppointmentByIdAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments.Where(x => x.AppointmentId == appointmentId)
                .ToAppointmentVM()
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                throw new HttpException(HttpStatusCode.NotFound, TranslationStrings.COMMON_NOT_FOUND);
            }

            return appointment;
        }

        public async Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(DateTime date)
        {
            var allSlots = Enumerable.Range(startHour, endHour - startHour)
                .Where(h => h < startBreak || h >= endBreak) 
                .Select(h => new TimeSpan(h, 0, 0))
                .ToList();

            var bookedSlots = await _context.Appointments
                .Where(a => a.Date.Date == date.Date)
                .Select(a => a.TimeSlot)
                .ToListAsync();

            return allSlots.Except(bookedSlots).ToList();
        }

        /* ------------------------------ Find Async --------------------------------- */
        public async Task<PageResult<AppointmentViewModel>> FindAppointmentsAsync(FilterAppointmentViewModel filter, int page, int size)
        {
            var query = _context.Appointments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.PatientFirstName))
            {
                query = query.Where(x => x.PatientFirstName.Contains(filter.PatientFirstName));
            }

            if (!string.IsNullOrWhiteSpace(filter.PatientLastName))
            {
                query = query.Where(x => x.PatientLastName.Contains(filter.PatientLastName));
            }

            if (filter.Date.HasValue)
            {
                query = query.Where(x => x.Date == filter.Date);
            }

            if (filter.TreatmentType.HasValue)
            {
                query = query.Where(x => x.TreatmentType == filter.TreatmentType);
            }

            if (filter.IsConfirmed.HasValue)
            {
                query = query.Where(x => x.IsConfirmed == filter.IsConfirmed);
            }

            if (filter.IsCompleted.HasValue)
            {
                query = query.Where(x => x.IsCompleted == filter.IsCompleted);
            }

            // D Y N A M I C   S O R T

            query.OrderBy(x => x.Date);

            return new PageResult<AppointmentViewModel>()
            {
                CollectionSize = await query.CountAsync(),
                Result = await query
                .Paginate(page, size)
                .ToAppointmentVM()
                .ToListAsync()
            };
        }

        /* ------------------------------ Create or Update Async --------------------- */
        public async Task<Guid> CreateOrUpdateAppointmentAsync(Appointment model, string userRef)
        {
            model.PatientFirstName = model.PatientFirstName.Trim();
            model.PatientLastName = model.PatientLastName.Trim();

            var appointment = await _context.Appointments.FirstOrDefaultAsync(x => x.AppointmentId == model.AppointmentId);
            if (await _context.Appointments.AnyAsync(x => (x.PatientFirstName != model.PatientFirstName && x.PatientLastName != model.PatientLastName && x.Date == model.Date && x.TimeSlot == model.TimeSlot) && x.AppointmentId != model.AppointmentId))
            {
                throw new HttpException(HttpStatusCode.BadRequest, TranslationStrings.ACTIVITY_CODE_DUPLICATE);
            }

            bool isValidTime = (model.TimeSlot >= TimeSpan.FromHours(startHour) && model.TimeSlot < TimeSpan.FromHours(startBreak)) ||
                               (model.TimeSlot >= TimeSpan.FromHours(endBreak)  && model.TimeSlot < TimeSpan.FromHours(endHour));
            if (!isValidTime)
            {
                throw new HttpException(HttpStatusCode.BadRequest, TranslationStrings.ACTIVITY_TIME_SLOT_INVALID);
            }

            bool isSlotTaken = await _context.Appointments.AnyAsync(x =>
                x.Date == model.Date &&
                x.TimeSlot == model.TimeSlot &&
                x.AppointmentId != model.AppointmentId);
            if (isSlotTaken)
            {
                throw new HttpException(HttpStatusCode.BadRequest, TranslationStrings.ACTIVITY_TIME_SLOT_DUPLICATE);
            }

            if (appointment == null)
            {
                appointment = new Appointment();
                _context.Appointments.Add(appointment);
                model.IsCompleted = false;
                model.IsConfirmed = false;
            }

            appointment.Date = model.Date;
            appointment.TimeSlot = model.TimeSlot;
            appointment.PatientFirstName = model.PatientFirstName;
            appointment.PatientLastName = model.PatientLastName;
            appointment.PatientTelephoneNumber = model.PatientTelephoneNumber;
            appointment.TreatmentType = model.TreatmentType;
            appointment.IsConfirmed = model.IsConfirmed;
            appointment.IsCompleted = model.IsCompleted;
            appointment.Notes = model.Notes;
            appointment.UpdateBase(userRef: userRef);

            await _context.SaveChangesAsync();
            return appointment.AppointmentId;
        }

        /* ------------------------------ Delete Async ------------------------------- */
        public async Task DeleteAppointmentAsync(Guid appointmentId)
        {
            var item = await _context.Appointments.FirstOrDefaultAsync(x => x.AppointmentId == appointmentId);

            if (item == null)
            {
                throw new HttpException(HttpStatusCode.NotFound, TranslationStrings.COMMON_NOT_FOUND);
            }

            _context.Appointments.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
