using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.ViewModels.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.Validate
{
    public class ValidateService : IValidateService
    {
        private readonly IUserRepo _userRepo;
        public ValidateService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ResultModel> IsEmailUnique(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Email cannot be empty."
                };
            }

            email = email.Trim().ToLower();
            var existingUser = await _userRepo.GetByEmailAsync(email);

            if (existingUser != null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "This email is already in use."
                };
            }

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "Email is unique."
            };
        }

        public async Task<ResultModel> IsPhoneValid(string phone)
        {
            var res = new ResultModel();

            try
            {
                if (!Regex.IsMatch(phone, "^0\\d{9,10}$"))
                {
                    res.IsSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid phone number";
                    return res;

                }
                var userList = await _userRepo.GetAllUser();
                var existedPhone = userList.FirstOrDefault(x => x.PhoneNumber == phone);
                if (existedPhone != default)
                {
                    res.IsSuccess = false;
                    res.Code = (int)HttpStatusCode.BadRequest;
                    res.Message = "The provided phone has already existed";
                    return res;
                }
                return null;


            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = ex.Message;
                return res;
            }
            return null;
        }

        public async Task<ResultModel> IsUserNameUnique(string username)
        {
            var result = new ResultModel();
            try
            {
                var existedUser = await _userRepo.GetByUsernameAsync(username);
                if (existedUser != null)
                {
                    result.IsSuccess = false;
                    result.Code = (int)HttpStatusCode.BadRequest;
                    result.Message = "The provided username has already existed";
                }
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)HttpStatusCode.BadRequest;
                result.Message = ex.Message;
                return result;
            }

        }
    }
}
