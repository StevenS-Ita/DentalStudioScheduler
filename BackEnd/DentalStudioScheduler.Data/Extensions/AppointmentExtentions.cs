using DentalStudioScheduler.Model;
using DentalStudioScheduler.Models;

namespace DentalStudioScheduler.Data.Extensions
{
    public static class AppointmentExtentions
    {
        public static IQueryable<AppointmentViewModel> ToAppointmentVM(this IQueryable<Appointment> appointment)
        {
            return appointment
                .Select(x => new AppointmentViewModel()
                {
                    AppointmentId = x.AppointmentId,
                    PatientFirstName = x.PatientFirstName,
                    PatientLastName = x.PatientLastName,
                    Date = x.Date,
                    TimeSlot = x.TimeSlot,
                    TreatmentType = x.TreatmentType,
                    IsConfirmed = x.IsConfirmed,
                    IsCompleted = x.IsCompleted,
                });
        }
    }
}
