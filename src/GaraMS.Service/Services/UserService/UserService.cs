using GaraMS.Data.Models;
using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.ViewModels.AutheticateModel;
using GaraMS.Data.ViewModels.CreateReqModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.Email;
using GaraMS.Service.Services.HashPass;
using GaraMS.Service.Services.TokenService;
using GaraMS.Service.Services.Validate;
using Microsoft.Extensions.Configuration;
using RTools_NTS.Util;
using System.Net;

namespace GaraMS.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IValidateService _Validate;
        private readonly IAccountService _accountService;
        private readonly IAuthenticateService _authentocateService;
        private readonly ITokenService _token;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepo userRepo,
            ITokenService token,
            IAuthenticateService authenticateService,
            IAccountService accountService,
            IValidateService userValidate,
            IEmailService emailService,
            IConfiguration configuration
            )
        {
            _userRepo = userRepo;
            _token = token;
            _authentocateService = authenticateService;
            _accountService = accountService;
            _Validate = userValidate;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<ResultModel> GetLoggedInUser(string token)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Invalid token."
            };

            if (string.IsNullOrEmpty(token))
            {
                return res;
            }

            var decodedUser = _token.decode(token);
            if (decodedUser == null || string.IsNullOrEmpty(decodedUser.userid))
            {
                return res;
            }

            if (!int.TryParse(decodedUser.userid, out int userId))
            {
                return res;
            }
            if (userId <= 0)
            {
                return res;
            }

            var user = await _userRepo.GetLoginAsync(userId);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                };
            }

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "User retrieved successfully.",
                Data = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.FullName,
                    user.Address,
                    user.Status,
                    user.RoleId,
                    user.CreatedAt,
                    Gara = user.Garas != null ? user.Garas.Select(g => new
                    {
                        g.GaraId,
                        g.GaraNumber,
                        g.UserId
                    }).ToList() : null
                }
            };

        }
        public async Task<ResultModel> GetUserById(int id)
        {
            

            

            var user = await _userRepo.GetLoginAsync(id);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "User not found."
                };
            }

            return new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Message = "User retrieved successfully.",
                Data = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.FullName,
                    user.Address,
                    user.Status,
                    user.RoleId,
                    user.CreatedAt,
                    Gara = user.Garas != null ? user.Garas.Select(g => new
                    {
                        g.GaraId,
                        g.GaraNumber,
                        g.UserId
                    }).ToList() : null
                }
            };

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

                Employee newEmployee = new Employee
                {
                    Salary = 0,
                    SpecializedId = null
                };

                await _userRepo.AddEmployeeAsync(newEmployee);
            }
            var phoneValidationResult = await _Validate.IsPhoneValid(model.PhoneNumber);
            if (phoneValidationResult != null) return phoneValidationResult;
            string hashedPassword = HashPass.HashPass.HashPassword(model.Password);
            var user = new User
            {
                UserName = model.UserName,
                Password = hashedPassword,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                Address = model.Address,
                Status = model.RoleId == 1 ? false : true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId = model.RoleId ?? 3
            };
            await _userRepo.AddAsync(user);
            if (model.RoleId == 1)
            {
                Customer newCustomer = new Customer
                {
                    UserId = user.UserId,
                    Gender = "none",
                    Note = ""
                };
                //await _userRepo.UpdateAsync(user);
                await SendStatusChangeEmail(user);
                await _userRepo.AddCustomerAsync(newCustomer);
            }
            if (model.RoleId == 2)
            {

                Employee newEmployee = new Employee
                {
                    Salary = 0,
                    SpecializedId = null,
                    UserId = user.UserId
                };

                await _userRepo.AddEmployeeAsync(newEmployee);
            }
            if (model.RoleId == 3)
            {

                Manager newManager = new Manager
                {
                    Salary = 0,
                    Gender = "none",
                    UserId = user.UserId
                };

                await _userRepo.AddManagerAsync(newManager);
            }
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
        private async Task SendStatusChangeEmail(User user)
        {
            var subject = "Wait for admin Accept Request";
            var body = $@"
<html>
<head></head>
<body>
    <h2>Status Change Request</h2>
    <p><strong>Hello {user.FullName},</strong></p>
    <p>Wait for admin accept request</p>
    <p>
    </p>
</body>
</html>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        public async Task<ResultModel> ConfirmUserStatus(string token,int userId)
        {
            var res = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 3, 4 });
            if (!isValidRole)
            {
                res.IsSuccess = false;
                res.Code = (int)HttpStatusCode.Forbidden;
                res.Message = "You don't have permission to perform this action.";
                return res;
            }

            var user = await _userRepo.GetLoginAsync(userId);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 404,
                    Message = "User not found."
                };
            }

            if ((bool)user.Status)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 400,
                    Message = "Account already confirmed."
                };
            }

            user.Status = true;
            await _userRepo.UpdateAsync(user);

            return new ResultModel
            {
                IsSuccess = true,
                Code = 200,
                Message = "Account confirmed successfully!"
            };
        }

        public async Task<ResultModel> GetFalseUser(string token)
        {
            var res = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };

            var decodeModel = _token.decode(token);
            var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 3, 4 });
            if (!isValidRole)
            {
                res.IsSuccess = false;
                res.Code = (int)HttpStatusCode.Forbidden;
                res.Message = "You don't have permission to perform this action.";
                return res;
            }

            var falseUser = await _userRepo.GetFalseUser();
            if (falseUser == null)
            {
                res.IsSuccess = false;
                res.Code = (int)HttpStatusCode.NotFound;
                res.Message = "No false user found.";
                return res;
            }

            res.Data = falseUser;
            res.Message = "False user retrieved successfully.";
            return res;
        }
        public async Task<ResultModel> ChangePassword(string token, ChangePasswordModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = true,
                Code = (int)HttpStatusCode.OK,
                Data = null,
                Message = null,
            };

            var decodeModel = _token.decode(token);

            var existingUser = await _userRepo.GetLoginAsync(int.Parse(decodeModel.userid));
            bool isMatch = HashPass.HashPass.VerifyPassword(model.OldPassword, existingUser.Password);
            if (!isMatch)
            {
                res.IsSuccess = false;
                res.Code = 400;
                res.Message = "Old password is wrong";
                return res;
            }
            try
            {

                string hashNewPassword = HashPass.HashPass.HashPassword(model.NewPassword);
                existingUser.Password = hashNewPassword;
                await _userRepo.UpdateAsync(existingUser);

                res.IsSuccess = true;
                res.Code = 200;
                res.Message = "Change password succesfully";
                return res;
            }
            catch (Exception e)
            {
                res.IsSuccess = false;
                res.Code = 400;
                return res;
            }



        }

        public async Task<ResultModel> EditUser(string token, EditUserModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request."
            };

            if (string.IsNullOrEmpty(token))
            {
                res.Message = "Token is required.";
                return res;
            }

            var decodeModel = _token.decode(token);
            if (decodeModel == null)
            {
                res.Code = (int)HttpStatusCode.Unauthorized;
                res.Message = "Invalid token.";
                return res;
            }

            var user = await _userRepo.GetLoginAsync(int.Parse(decodeModel.userid));
            if (user == null)
            {
                res.Code = (int)HttpStatusCode.NotFound;
                res.Message = "User not found.";
                return res;
            }

            user.UserName = model.UserName ?? user.UserName;
            user.Email = model.Email ?? user.Email;
            user.FullName = model.FullName ?? user.FullName;
            user.Address = model.Address ?? user.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            res.IsSuccess = true;
            res.Code = (int)HttpStatusCode.OK;
            res.Message = "User updated successfully.";
            res.Data = new
            {
                user.UserName,
                user.Email,
                user.FullName,
                user.Address,
                user.UpdatedAt
            };

            return res;
        }
        public async Task<ResultModel> EditUserById(int id, EditUserModel model)
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request."
            };

            

            var user = await _userRepo.GetLoginAsync(id);
            if (user == null)
            {
                res.Code = (int)HttpStatusCode.NotFound;
                res.Message = "User not found.";
                return res;
            }

            user.UserName = model.UserName ?? user.UserName;
            user.Email = model.Email ?? user.Email;
            user.FullName = model.FullName ?? user.FullName;
            user.Address = model.Address ?? user.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            res.IsSuccess = true;
            res.Code = (int)HttpStatusCode.OK;
            res.Message = "User updated successfully.";
            res.Data = new
            {
                user.UserName,
                user.Email,
                user.FullName,
                user.Address,
                user.UpdatedAt
            };

            return res;
        }
        public async Task<ResultModel> GetAllAsync()
        {
            var res = new ResultModel
            {
                IsSuccess = false,
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Invalid request."
            };
            var a = await _userRepo.GetAllUser();

            res.IsSuccess = true;
            res.Code = (int)HttpStatusCode.OK;
            res.Message = "User updated successfully.";
            res.Data = a;

            return res;
        }
    }
}
