using DentalStudioScheduler.Context.Base;
using DentalStudioScheduler.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalStudioScheduler.Context
{
    /// <summary>
    /// Registration Log Context
    /// </summary>
    public class DentalStudioContext : CoreContextBase<DentalStudioContext>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public DentalStudioContext(DbContextOptions<DentalStudioContext> options): base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
    }
}
