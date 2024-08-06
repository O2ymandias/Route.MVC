using DAL.Entities.BusinessModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations
{
    internal class DepartmentConfig : IEntityTypeConfiguration<Department>
	{
		public void Configure(EntityTypeBuilder<Department> builder)
		{
			builder
				.Property(d => d.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder
				.Property(d => d.Code)
				.IsRequired();

			builder
				.Property(d => d.Id);
		}
	}
}
