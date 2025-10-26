using DentalStudioScheduler.Models;

namespace DentalStudioScheduler.Model
{
    public class AppointmentViewModel
    {   
        public Guid? AppointmentId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeSlot { get; set; }
        public TreatmentType TreatmentType { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
    }
}