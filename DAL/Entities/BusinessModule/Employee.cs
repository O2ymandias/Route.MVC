namespace DAL.Entities.BusinessModule
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum EmployeeType
    {
        FullTime,
        PartTime
    }

    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime HiringDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public Gender Gender { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public string? ImageName { get; set; }


        public virtual Department Department { get; set; }
        public int? DepartmentId { get; set; }
    }
}
