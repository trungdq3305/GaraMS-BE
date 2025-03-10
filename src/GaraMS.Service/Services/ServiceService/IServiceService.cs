using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.ServiceService
{
	public interface IServiceService
	{
		Task<ResultModel> GetAllServicesAsync();
		Task<ResultModel> GetServiceByIdAsync(int id);
		Task<ResultModel> CreateServiceAsync(string token, ServiceModel model);
		Task<ResultModel> UpdateServiceAsync(string token, int id, ServiceModel model);
		Task<ResultModel> DeleteServiceAsync(string token, int id);
		Task<ResultModel> UpdateServicePromotionAsync(string? token, int serviceId, decimal promotionAmount);
		Task<ResultModel> ApplyPromotionToServiceAsync(string? token, int serviceId, decimal discountPercent);
	}
}
