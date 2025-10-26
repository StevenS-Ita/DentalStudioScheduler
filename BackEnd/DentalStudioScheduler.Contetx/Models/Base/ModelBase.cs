using System.ComponentModel.DataAnnotations;

namespace DentalStudioScheduler.Context.Models.Base
{
    /// <summary>
    /// Base model for almost every database model.
    /// </summary>
    public abstract class ModelBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModelBase()
        {
            Ref = Environment.MachineName;
            UserRef = Environment.UserName;
            DateCreate = DateTime.Now;
            DateChange = DateTime.Now;
        }

        /// <summary>
        /// User name.
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public string UserRef { get; set; }

        /// <summary>
        /// Computer name.
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public string Ref { get; set; }

        /// <summary>
        /// Creation date.
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Edit date.
        /// </summary>
        public DateTime DateChange { get; set; }

        /// <summary>
        /// Call on update operations for update user and date changes.
        /// </summary>
        /// <param name="machineRef"></param>
        /// <param name="userRef"></param>
        public void UpdateBase(string machineRef = null, string userRef = null)
        {
            DateChange = DateTime.Now;
            Ref = machineRef ?? Environment.MachineName;
            UserRef = userRef ?? Environment.UserName;
        }
    }
}
