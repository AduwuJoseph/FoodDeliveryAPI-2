using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FoodDeliveryApp.API.Models;
using FoodDeliveryApp.API.Helpers.Interfaces;
using FoodDeliveryApp.API.Services.Interfaces;
using FoodDeliveryApp.BL.BusinessLogic.Interfaces;
using FoodDeliveryApp.BL.DVM;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using FoodDeliveryApp.API.Helpers;

namespace FoodDeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUsersBL usersBL;
        private readonly ISetupsBL setupBL;
        private readonly ICustomersBL customersBL;
        private readonly IConfiguration configuration;
        private readonly IUtilities utilities;
        private readonly IAuthenticateService authentication;

        public AccountController(IConfiguration _configuration,
            IUsersBL _usersBL,
            ICustomersBL _customersBL,
            IUtilities _utilities,
            IAuthenticateService _authentication,
            ISetupsBL _setupBL)
        {
            usersBL = _usersBL;
            setupBL = _setupBL;
            customersBL = _customersBL;
            configuration = _configuration;
            utilities = _utilities;
            authentication = _authentication;
        }

        [HttpPost]
        [Route("PasswordRecovery")]
        public async Task<RequestResult> PasswordRecovery(ForgotPasswordViewModel form)
        {
            try
            {
                var email = string.Empty;
                if (ModelState.IsValid) {
                    var user = usersBL.GetUserByEmail(email);
                    if (user == null)
                    {
                        return new RequestResult { Message = "Not found!", Status = false };
                    }
                    var random = new Random();
                    var newPassword = string.Format("{0}", random.Next(100000, 999999));
                    user.NewPassword = newPassword;

                    string response = await usersBL.ForgotPassword(user);
                    if (!string.IsNullOrEmpty(response))
                    {
                        var subject = "Aduwu Food Delivery - Password Recovery";
                        var body = string.Format(@"<h1>Aduwu Food Delivery - Password Recovery</h1><p>Your new password is: <strong> {0} </strong></p><p>Please, don't forget to change the password.", newPassword);
                        bool result = utilities.SendMail(email, body, null, subject);
                        return new RequestResult { Status = true, Message = "We have sent an email with your new password. Kindly change it when you login" };
                    }
                    return new RequestResult { Message = "The password can't be changed.", Status = false };
                }
                return new RequestResult { Message = "Bad Request", Status = false };
            }
            catch (Exception ex)
            {
                return new RequestResult { Message = ex.Message, Status = false };
            }
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<RequestResult> ChangePassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new RequestResult { Message = "All fields are required", Status=false };
            }
            var user = usersBL.GetUserByEmail(model.Email);
            if(user != null)
            {
                user.NewPassword = model.ConfirmPassword;
                user.Password = model.Password;
                bool result = await usersBL.ChangePassword(user);
                if (result)
                {
                    return new RequestResult { Message = "Password has been changed.", Status = true };
                }
            }
            return new RequestResult { Message = "Unable to change your password. Please try again.", Status=false };
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("Register")]
        public async Task<RequestResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new RequestResult { Message = "All fields are required", Status = false };
            }
            var cus = await customersBL.GetCustomerByEmailID(model.Email.ToLower());
            if (cus == null)
            {
                cus = new CustomerDVM
                {
                    CustomerId = Helper.GenerateGuid(),
                    Address = model.Address,
                    CityCode = model.CityCode,
                    DateCreated = DateTime.Now,
                    Email = model.Email.ToLower(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    Status = Status.Active
                };
                var cid = await customersBL.AddCustomer(cus);
                var user = usersBL.GetUserByEmail(model.Email);
                if (!string.IsNullOrEmpty(cid) && user == null)
                {
                    AspNetUserDVM usern = new AspNetUserDVM
                    {
                        Email = model.Email.ToLower(),
                        UserName = model.Email.ToLower(),
                        Password = model.Password
                    };

                    bool result = await usersBL.AddUser(usern, "Customer");
                    if (result)
                    {
                        return new RequestResult { Message = "Account created successfully.", Status = true };
                    }
                }
                return new RequestResult { Message = "Unable to create account. Please try again.", Status = false };
            }
            else
            {
                return new RequestResult { Message = "Email address already taken. Please try again with another email.", Status = false };
            }
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public async Task<RequestResult> Logout()
        {
            bool result = await usersBL.SignOut();
            return new RequestResult { Message="Signed Out", Status=result };
        }

        [HttpPost]
        public IActionResult Post([FromBody] AspNetUserVM vm)
        {
            var user = authentication.Authenticate(vm);
            if (user == null)
                return BadRequest(new { message = "Username and Password is incorrect" });
            return Ok(user);
        }
    }
}
