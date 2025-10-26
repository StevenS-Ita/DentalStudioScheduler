using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;

namespace DentalStudioScheduler.Context.Base
{
    /// <summary>
    /// Base class for any context than user ef core.
    /// </summary>
    /// <typeparam name="T">The type of the context.</typeparam>
    public abstract class CoreContextBase<T> : DbContext where T : CoreContextBase<T>
    {
        private readonly string connString;
        private readonly ILoggerFactory loggerFactory;
        private readonly bool readOnly;

        public CoreContextBase() : base() { }

        public CoreContextBase(DbContextOptions options) : base(options) { }

        public CoreContextBase(string connectionString, ILoggerFactory loggerFactory = null) : base()
        {
            connString = connectionString;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Costruttore.
        /// </summary>
        public CoreContextBase(string connectionString, ILoggerFactory loggerFactory, bool isReadOnly) : base()
        {
            connString = connectionString;
            this.loggerFactory = loggerFactory;
            readOnly = isReadOnly;
        }

        /// <summary>
        /// Crea instanza di un context.
        /// </summary>
        /// <param name="conString">Stringa di connessione.</param>
        /// <returns></returns>
        public static T Create(string conString)
        {
            if (string.IsNullOrWhiteSpace(conString))
                throw new ArgumentNullException("conString");

            T ctx = default;
            ctx = (T)Activator.CreateInstance(typeof(T), conString);

            return ctx;
        }

        /// <summary>
        /// Crea context di sola lettura.
        /// </summary>
        /// <param name="conString">Stringa di connessione.</param>
        /// <param name="loggerFactory"></param>
        /// <returns>Istanza del context.</returns>
        public static T CreateReadContext(string conString, ILoggerFactory loggerFactory = null)
        {
            T ctx = (T)Activator.CreateInstance(typeof(T), conString, loggerFactory, true);
            ctx.ChangeTracker.AutoDetectChangesEnabled = false;
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            return ctx;
        }

        /// <summary>
        /// Crea context di scrittura.
        /// </summary>
        /// <param name="conString">Stringa di connessione.</param>
        /// <param name="loggerFactory"></param>
        /// <returns>Istanza del context.</returns>
        public static T CreateWriteContext(string conString, ILoggerFactory loggerFactory = null)
        {
            T ctx = (T)Activator.CreateInstance(typeof(T), conString, loggerFactory, false);
            ctx.ChangeTracker.AutoDetectChangesEnabled = true;

            return ctx;
        }

        /// <summary>
        /// Automatically attach the logger using the eventual logger factory.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(connString))
            {
                optionsBuilder.UseSqlServer(connString, x =>
                {
                    x.EnableRetryOnFailure();
                });

                if (Debugger.IsAttached) optionsBuilder.EnableSensitiveDataLogging(true);
            }
            if (loggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
        }

        /// <summary>
        /// Save changes to the database.
        /// </summary>
        /// <returns>Number of rows changed.</returns>
        public override int SaveChanges()
        {
            try
            {
                if (readOnly == true)
                {
                    throw new InvalidOperationException("This context is read-only.");
                }
                return base.SaveChanges();
            }
            catch (DataException ex)
            {
                throw new Exception($"{ex.Message} - {ex.Data}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"{ex.Message} - {ex.Data}", ex);
            }
        }
    }
}
