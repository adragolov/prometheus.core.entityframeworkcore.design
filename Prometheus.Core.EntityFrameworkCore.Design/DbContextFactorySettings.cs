namespace Prometheus.Core.EntityFrameworkCore.Design
{
    /// <summary>
    ///     Settings object associated with the <seealso cref="DbContextFactoryBase{TContext}"/> and
    ///     its derivatives.
    /// </summary>
    public class DbContextFactorySettings
    {
        /// <summary>
        ///     Gets indication, if the design time factory will *not* perform runtime logging.
        /// </summary>
        public bool LoggingDisabled { get; set; }
    }
}