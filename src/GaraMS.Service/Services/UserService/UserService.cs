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
        private readonly GaraManagementSystemContext _context;
        public UserService(IUserRepo userRepo,
            ITokenService token,
            IAuthenticateService authenticateService,
            IAccountService accountService,
            IValidateService userValidate,
            IEmailService emailService,
            IConfiguration configuration,
            GaraManagementSystemContext context
            )
        {
            _userRepo = userRepo;
            _token = token;
            _authentocateService = authenticateService;
            _accountService = accountService;
            _Validate = userValidate;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
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
                var newii = new InventoryInvoice
                {
                    Price = 0,
                    DiliverType = "Pending",
                    PaymentMethod = "Pending",
                    TotalAmount = 0,
                    Status = "True",
                    UserId = user.UserId,
                };
                _context.InventoryInvoices.Add(newii);
                await _context.SaveChangesAsync();
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
            try
            {
                // Generate a random 6-digit confirmation code
                Random random = new Random();
                string confirmationCode = random.Next(100000, 999999).ToString();
                
                // Store the confirmation code in the user object or a separate table
                // For simplicity, we'll use a temporary approach - storing it in the user's address field
                // In a real application, you should create a proper table for confirmation codes
                user.Address = $"{user.Address}|CONFIRM:{confirmationCode}";
                await _userRepo.UpdateAsync(user);
                
                string subject = "Confirm Your Account Status - GaraMS";
                string body = $@"
                    <h2>Account Status Confirmation</h2>
                    <p>Hello {user.FullName},</p>
                    <p>Your confirmation code is:</p>
                    <h1 style='text-align: center; font-size: 32px; letter-spacing: 5px; padding: 10px; background-color: #f0f0f0; border-radius: 5px;'>{confirmationCode}</h1>
                    <p>Please enter this code in the application to confirm your account.</p>
                    <p>This code will expire in 24 hours.</p>
                    <p>Thank you,<br>GaraMS Team</p>
                ";
                
                await _emailService.SendEmailAsync(user.Email, subject, body);
                Console.WriteLine($"Status confirmation email sent to {user.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send status confirmation email: {ex.Message}");
                // Don't throw the exception - we don't want to fail the entire operation if email sending fails
            }
        }

        public async Task<ResultModel> ConfirmUserStatus(string token, int userId)
        {
            try
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

                // If token is provided, check permissions
                if (!string.IsNullOrEmpty(token))
                {
                    var decodeModel = _token.decode(token);
                    var isValidRole = _accountService.IsValidRole(decodeModel.role, new List<int>() { 3, 4 });
                    if (!isValidRole)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            Code = 403,
                            Message = "You don't have permission to perform this action."
                        };
                    }
                }

                user.Status = true;
                await _userRepo.UpdateAsync(user);

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Account confirmed successfully!",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
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
                IsSuccess = false,
                Code = 400,
                Message = "Invalid request."
            };

            if (string.IsNullOrEmpty(token))
            {
                res.Message = "Authentication required.";
                return res;
            }

            if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmationCode))
            {
                res.Message = "Old password, new password, and confirmation code are required.";
                return res;
            }

            var decodeModel = _token.decode(token);
            if (decodeModel == null || string.IsNullOrEmpty(decodeModel.userid))
            {
                res.Code = 401;
                res.Message = "Invalid token.";
                return res;
            }

            var existingUser = await _userRepo.GetLoginAsync(int.Parse(decodeModel.userid));
            if (existingUser == null)
            {
                res.Code = 404;
                res.Message = "User not found.";
                return res;
            }

            // Verify old password
            bool isMatch = HashPass.HashPass.VerifyPassword(model.OldPassword, existingUser.Password);
            if (!isMatch)
            {
                res.Message = "Old password is incorrect.";
                return res;
            }

            // Verify confirmation code
            if (existingUser.Address == null || !existingUser.Address.Contains($"|PWCHANGE:{model.ConfirmationCode}"))
            {
                res.Message = "Invalid confirmation code.";
                return res;
            }

            try
            {
                // Hash the new password
                string hashNewPassword = HashPass.HashPass.HashPassword(model.NewPassword);
                existingUser.Password = hashNewPassword;
                
                // Remove the confirmation code from the address
                existingUser.Address = existingUser.Address.Split("|")[0];
                
                await _userRepo.UpdateAsync(existingUser);

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Password changed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
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
            if (!string.IsNullOrEmpty(model.Phone) && model.Phone != user.PhoneNumber)
            {
                var phoneValidationResult = await _Validate.IsPhoneValid(model.Phone);
                if (phoneValidationResult != null && !phoneValidationResult.IsSuccess)
                {
                    res.Code = (int)HttpStatusCode.NotFound;
                    res.Message = "Invalid phone number.";
                    return res;
                }
            }

            user.PhoneNumber = model.Phone ?? user.PhoneNumber;
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
                user.PhoneNumber,
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

            user.PhoneNumber = model.Phone ?? user.PhoneNumber;
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
                user.PhoneNumber,
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

        public async Task<ResultModel> ConfirmWithCode(string email, string code)
        {
            try
            {
                // Get the user by email
                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }
                
                // Check if the address field contains the confirmation code
                if (user.Address != null && user.Address.Contains($"|CONFIRM:{code}"))
                {
                    // Update user status
                    user.Status = true;
                    // Remove the confirmation code from the address
                    user.Address = user.Address.Split('|')[0];
                    await _userRepo.UpdateAsync(user);
                    
                    return new ResultModel
                    {
                        IsSuccess = true,
                        Code = 200,
                        Message = "Account confirmed successfully!",
                        Data = user
                    };
                }
                else
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Invalid confirmation code."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> RequestPasswordReset(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Email is required."
                    };
                }

                // Trim email to remove any accidental whitespace
                email = email.Trim();

                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    // For security reasons, don't reveal that the user doesn't exist
                    return new ResultModel
                    {
                        IsSuccess = true,
                        Code = 200,
                        Message = "If your email is registered, you will receive a password reset code."
                    };
                }

                // Generate a random 6-digit reset code
                Random random = new Random();
                string resetCode = random.Next(100000, 999999).ToString();
                
                // Store the reset code in a consistent format
                // If Address is null, initialize it to an empty string
                string addressBase = user.Address ?? "";
                
                // Remove any existing reset codes
                if (addressBase.Contains("|RESET:"))
                {
                    addressBase = addressBase.Split("|RESET:")[0];
                }
                
                // Add the new reset code
                user.Address = $"{addressBase}|RESET:{resetCode}";
                
                // Log for debugging
                Console.WriteLine($"Setting reset code for {email}: {resetCode}");
                Console.WriteLine($"New address value: {user.Address}");
                
                await _userRepo.UpdateAsync(user);
                
                // Send the reset code via email
                string subject = "Password Reset - GaraMS";
                string body = $@"
                    <h2>Password Reset</h2>
                    <p>Hello {user.FullName},</p>
                    <p>You requested to reset your password. Your password reset code is:</p>
                    <h1 style='text-align: center; font-size: 32px; letter-spacing: 5px; padding: 10px; background-color: #f0f0f0; border-radius: 5px;'>{resetCode}</h1>
                    <p>Please enter this code in the application to reset your password.</p>
                    <p>This code will expire in 1 hour.</p>
                    <p>If you didn't request this password reset, please ignore this email.</p>
                    <p>Thank you,<br>GaraMS Team</p>
                ";
                
                await _emailService.SendEmailAsync(user.Email, subject, body);
                
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Password reset code sent to your email."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RequestPasswordReset: {ex.Message}");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> VerifyResetCode(string email, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Email and reset code are required."
                    };
                }

                // Trim inputs to remove any accidental whitespace
                email = email.Trim();
                code = code.Trim();

                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }

                // Log for debugging
                Console.WriteLine($"Verifying reset code for {email}: {code}");
                Console.WriteLine($"User address value: {user.Address}");

                // Check if Address is null
                if (user.Address == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "No reset code found. Please request a new password reset."
                    };
                }

                // More flexible checking for the reset code
                // Check for both formats: "|RESET:code" and "RESET:code"
                bool hasResetCode = user.Address.Contains($"|RESET:{code}") || 
                                   user.Address.Contains($"RESET:{code}");

                if (hasResetCode)
                {
                    return new ResultModel
                    {
                        IsSuccess = true,
                        Code = 200,
                        Message = "Reset code verified successfully."
                    };
                }
                else
                {
                    // Provide more detailed error for debugging
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Invalid reset code. Please check and try again or request a new code."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerifyResetCode: {ex.Message}");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newPassword))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Email, reset code, and new password are required."
                    };
                }

                // Trim inputs
                email = email.Trim();
                code = code.Trim();

                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }
                
                // Check if Address is null
                if (user.Address == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "No reset code found. Please request a new password reset."
                    };
                }

                // More flexible checking for the reset code
                bool hasResetCode = user.Address.Contains($"|RESET:{code}") || 
                                   user.Address.Contains($"RESET:{code}");
                
                if (!hasResetCode)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Invalid reset code."
                    };
                }
                
                // Hash the new password
                string hashedPassword = HashPass.HashPass.HashPassword(newPassword);
                
                // Update the user's password
                user.Password = hashedPassword;
                
                // Remove the reset code from the address
                if (user.Address.Contains("|RESET:"))
                {
                    user.Address = user.Address.Split("|RESET:")[0];
                }
                else if (user.Address.Contains("RESET:"))
                {
                    user.Address = user.Address.Split("RESET:")[0];
                }
                
                // Update the user
                await _userRepo.UpdateAsync(user);
                
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Password reset successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ResetPassword: {ex.Message}");
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ResultModel> RequestChangePassword(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 401,
                        Message = "Authentication required."
                    };
                }

                var decodeModel = _token.decode(token);
                if (decodeModel == null || string.IsNullOrEmpty(decodeModel.userid))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 401,
                        Message = "Invalid token."
                    };
                }

                var userId = int.Parse(decodeModel.userid);
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

                // Generate a random 6-digit confirmation code
                Random random = new Random();
                string confirmationCode = random.Next(100000, 999999).ToString();
                
                // Store the confirmation code in the user's address field
                string addressBase = user.Address ?? "";
                
                // Remove any existing codes
                if (addressBase.Contains("|"))
                {
                    addressBase = addressBase.Split("|")[0];
                }
                
                // Add the new confirmation code
                user.Address = $"{addressBase}|PWCHANGE:{confirmationCode}";
                
                await _userRepo.UpdateAsync(user);
                
                // Send the confirmation code via email
                string subject = "Password Change Confirmation - GaraMS";
                string body = $@"
                    <h2>Password Change Confirmation</h2>
                    <p>Hello {user.FullName},</p>
                    <p>You have requested to change your password. Your confirmation code is:</p>
                    <h1 style='text-align: center; font-size: 32px; letter-spacing: 5px; padding: 10px; background-color: #f0f0f0; border-radius: 5px;'>{confirmationCode}</h1>
                    <p>Please enter this code in the application to confirm your password change.</p>
                    <p>This code will expire in 1 hour.</p>
                    <p>If you didn't request this password change, please secure your account immediately.</p>
                    <p>Thank you,<br>GaraMS Team</p>
                ";
                
                await _emailService.SendEmailAsync(user.Email, subject, body);
                
                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Confirmation code sent to your email."
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
    }
}
