using DentalStudioScheduler.Context.Base;

namespace DentalStudioScheduler.Models
{
    public class Appointment : ContextModelBase
    {
        public Guid AppointmentId { get; set; }

        /// <summary>
        /// Il nome del paziente è obbligatorio.
        /// </summary>
        public string PatientFirstName { get; set; }

        /// <summary>
        /// Il cognome del paziente è obbligatorio.
        /// </summary>
        public string PatientLastName { get; set; }

        /// <summary>
        /// Il nome del paziente è obbligatorio.
        /// </summary>
        public string PatientTelephoneNumber { get; set; }

        /// <summary>
        /// La data dell'appuntamento è obbligatoria.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// L'orario dell'appuntamento è obbligatorio.
        /// </summary>
        public TimeSpan TimeSlot { get; set; }

        /// <summary>
        /// Tipologia di appuntamento
        /// </summary>
        public TreatmentType TreatmentType { get; set; }

        /// <summary>
        /// Stato conferma appuntamento
        /// </summary>
        public bool IsConfirmed { get; set; } = false;

        /// <summary>
        /// Stato completamento appuntamento
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        public string? Notes { get; set; }

        //Avrebe senso gestire anche una corellazione con una tabella Clienti
    }
}
