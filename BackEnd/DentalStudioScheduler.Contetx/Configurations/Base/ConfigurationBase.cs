using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalStudioScheduler.Context.Base
{
    public static class ConfigurationBase
    {
        public static void ConfigureBase<TEntity>(this EntityTypeBuilder<TEntity> entity) where TEntity : ContextModelBase
        {
            entity.Property(e => e.Ref)
                .HasColumnOrder(1)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("(host_name())");

            entity.Property(e => e.UserRef)
                .HasColumnOrder(2)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql("(suser_sname())");

            entity.Property(e => e.DateChange)
                .HasColumnOrder(3)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.DateCreate)
                .HasColumnOrder(4)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        }
    }
}
