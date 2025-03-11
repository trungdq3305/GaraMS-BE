using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Data.ViewModels.ReportModel
{
    public class ReportViewModel
    {
        public int ReportId { get; set; }
        public string Problem { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class UpdateReportModel
    {
        public string Problem { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class CreateReportModel
    {
        public string Problem { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
    }
}
