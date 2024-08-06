using DAL.Entities.BusinessModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Data.Configurations
{
    internal class EmployeeConfig : IEntityTypeConfiguration<Employee>
	{
		public void Configure(EntityTypeBuilder<Employee> builder)
		{
			builder
				.Property(e => e.Name)
				.HasMaxLength(50);

			builder
				.Property(e => e.Salary)
				.HasColumnType("Decimal(18,2)");

			builder
				.Property(e => e.Gender)
				.HasConversion(to => to.ToString(),
					from => Enum.Parse<Gender>(from));

			builder
				.Property(e => e.EmployeeType)
				.HasConversion(to => to.ToString(),
					from => Enum.Parse<EmployeeType>(from));

			builder
				.HasOne(e => e.Department)
				.WithMany(d => d.Employees)
				.HasForeignKey(e => e.DepartmentId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
