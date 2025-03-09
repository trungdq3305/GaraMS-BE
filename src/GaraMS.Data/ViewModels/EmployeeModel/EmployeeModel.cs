using GaraMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GaraMS.Data.ViewModels.EmployeeModel
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; }
        public decimal? Salary { get; set; }
        public int? SpecializedId { get; set; }
        public string SpecializedName { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<ServiceModel> Services { get; set; } = new List<ServiceModel>();

    }

    public class ServiceModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public class CreateEmployeeModel
    {
        public decimal? Salary { get; set; }
        public int? SpecializedId { get; set; }
        public int? UserId { get; set; }
    }

    public class UpdateEmployeeModel
    {
        public decimal? Salary { get; set; }
        public int? SpecializedId { get; set; }
    }
    
    public class AssignServiceModel
    {
        public int ServiceId { get; set; }
    }
}
