using GaraMS.Data.Models;
using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.ViewModels.CreateReqModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.TokenService;
using GaraMS.Service.Services.Validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GaraMS.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IValidateService _Validate;
        private readonly IAccountService _accountService;
        private readonly IAuthenticateService _authentocateService;
        private readonly ITokenService _token;
        public UserService(IUserRepo userRepo,
            ITokenService token,
            IAuthenticateService authenticateService,
            IAccountService accountService,
            IValidateService userValidate
            )
        {
            _userRepo = userRepo;
            _token = token;
            _authentocateService = authenticateService;
            _accountService = accountService;
            _Validate = userValidate;

        }
        public async Task<ResultModel> CreateUser(string token, CreateUserModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request."
            };

            if (!string.IsNullOrEmpty(token))
            {
                var decodeModel = _token.decode(token);
                if (decodeModel == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = (int)HttpStatusCode.Unauthorized,
                        Message = "Invalid token."
                    };
                }
            }

            var existingUser = await _userRepo.GetByUsernameAsync(model.UserName);
            if (existingUser != null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "This user is already registered."
                };
            }

            var existingEmail = await _userRepo.GetByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.Conflict,
                    Message = "This email is already registered."
                };
            }
            if (model.RoleId == null || model.RoleId < 1 || model.RoleId > 3)
            {
                res.IsSuccess = false;
                res.Code = 400;
                res.Message = "Invalid role";
                return res;
            }
            if (model.RoleId == 2)
            {
                int newEmployeeId = await GenerateEmployeeID();
                int newUserId = await GenerateID(); 

                Employee newEmployee = new Employee
                {
                    EmployeeId = newEmployeeId,
                    UserId = newUserId,
                    Salary = 0,
                    SpecializedId = null 
                };

                await _userRepo.AddEmployeeAsync(newEmployee);
            }
            if (model.RoleId == 1)
            {
                int newCustomerId = await GenerateCustomerID();
                int newUserId = await GenerateID();

                Customer newCustomer = new Customer
                {
                    CustomerId = newCustomerId,
                    UserId = newUserId,
                    Gender = "none",
                    Note = ""
                };

                await _userRepo.AddCustomerAsync(newCustomer);
            }
            if (model.RoleId == 3)
            {
                int newManagerId = await GenerateManagerID();
                int newUserId = await GenerateID();

                Manager newManager = new Manager
                {
                    ManagerId = newManagerId,
                    UserId = newUserId,
                    Salary = 0,
                    Gender = "none"
                };

                await _userRepo.AddManagerAsync(newManager);
            }


            var phoneValidationResult = await _Validate.IsPhoneValid(model.PhoneNumber);
            if (phoneValidationResult != null) return phoneValidationResult;
            string hashedPassword = HashPass.HashPass.HashPassword(model.Password);
            var user = new User
            {
                UserId = await GenerateID(),
                UserName = model.UserName,
                Password = hashedPassword,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                Address = model.Address,
                Status = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId = model.RoleId ?? 4
            };

            await _userRepo.AddAsync(user);

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.Created,
                Message = "User created successfully",
                Data = new
                {
                    Email = user.Email,
                    UserId = user.UserId
                }
            };
        }


        private async Task<int> GenerateID()
        {
            var userList = await _userRepo.GetAllUser();
            int userLength = userList.Count() + 1;
            return userLength;
        }
        private async Task<int> GenerateEmployeeID()
        {
            var userList = await _userRepo.GetAllEmployee();
            int userLength = userList.Count() + 1;
            return userLength;
        }
        private async Task<int> GenerateCustomerID()
        {
            var userList = await _userRepo.GetAllCustomer();
            int userLength = userList.Count() + 1;
            return userLength;
        }
        private async Task<int> GenerateManagerID()
        {
            var userList = await _userRepo.GetAllManager();
            int userLength = userList.Count() + 1;
            return userLength;
        }
    }
}
