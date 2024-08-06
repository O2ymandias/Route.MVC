using AutoMapper;
using DAL.Entities.BusinessModule;
using DAL.Entities.IdentityModule;
using DPL.ViewModels;
using DPL.ViewModels.Auth.Role;
using DPL.ViewModels.Auth.User;
using Microsoft.AspNetCore.Identity;

namespace DPL.Helpers
{
    public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			CreateMap<Department, DepartmentVM>().ReverseMap();
			CreateMap<Employee, EmployeeVm>().ReverseMap();
			CreateMap<ApplicationUser, UserVM>();
			CreateMap<IdentityRole, RoleVM>();
		}
	}
}
