using GaraMS.Data.Models;
using GaraMS.Data.Repositories.UserRepo;
using GaraMS.Data.ViewModels.CreateReqModel;
using GaraMS.Data.ViewModels.ResultModel;
using GaraMS.Service.Services.AccountService;
using GaraMS.Service.Services.AutheticateService;
using GaraMS.Service.Services.Email;
using GaraMS.Service.Services.TokenService;
using GaraMS.Service.Services.Validate;
using Microsoft.Extensions.Configuration;
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
                Status = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RoleId = model.RoleId ?? 4
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
                    SpecializedId = null
                };

                await _userRepo.AddEmployeeAsync(newEmployee);
            }
            if (model.RoleId == 3)
            {

                Manager newManager = new Manager
                {
                    Salary = 0,
                    Gender = "none"
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
            var confirmationLink = $"https://localhost:5001/api/User/confirm?userId={user.UserId}";
            var subject = "Status Change Request";
            var body = $@"
        <html>
        <head></head>
        <body>
            <h2>Status Change Request</h2>
            <p>Please click the button below to change the status. This link can only be used once.</p>
            <p><strong>Hello {user.FullName},</strong></p>
            <p>Please confirm your account by clicking the button below:</p>
            <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: white; background-color: #4CAF50; text-decoration: none; border-radius: 5px;'>
                Confirm Account
            </a>
        </body>
        </html>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }


        public async Task<ResultModel> ConfirmUserStatus(int userId)
        {
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
    }
}
