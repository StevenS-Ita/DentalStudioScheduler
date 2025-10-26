using DentalStudioScheduler.Models;

namespace DentalStudioScheduler.Data.ViewModels.Filters
{
    public class FilterAppointmentViewModel
    {
        public Guid? AppointmentId { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }
        public DateTime? Date { get; set; }
        public TreatmentType? TreatmentType { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
