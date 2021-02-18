using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FoodDeliveryApp.API.Models;
using FoodDeliveryApp.BL.BusinessLogic.Interfaces;
using FoodDeliveryApp.BL.DVM;
using FoodDeliveryApp.API.Helpers;

namespace FoodDeliveryApp.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IUsersBL usersBL;
        private readonly ISetupsBL setupBL;
        private readonly ICustomersBL customersBL;
        private readonly IConfiguration configuration;

        public CustomerController(IConfiguration _configuration,
            IUsersBL _usersBL,
            ICustomersBL _customersBL,
            ISetupsBL _setupBL)
        {
            usersBL = _usersBL;
            setupBL = _setupBL;
            customersBL = _customersBL;
            configuration = _configuration;
        }

        // POST api/Customer/Register
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

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<List<CustomerVM>>> GetAllCustomers()
        {
            var cus = await customersBL.GetAllCustomers();
            return cus.Select(x => new CustomerVM
            { 
                DateCreated=x.DateCreated,
                Address = x.Address,
                CityCode = x.CityCode,
                CityName = x.CityCode > 0 ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                CustomerId = x.CustomerId,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,
                Status = x.Status
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetCustomer/{id}")]
        public async Task<ActionResult<CustomerVM>> GetCustomer(string id)
        {
            CustomerVM p = new CustomerVM();
            if (!string.IsNullOrEmpty(id))
            {
                var x = await customersBL.GetCustomerByID(id);
                if (x != null)
                {
                    p = new CustomerVM
                    {
                        DateCreated = x.DateCreated,
                        Address = x.Address,
                        CityCode = x.CityCode,
                        CityName = x.CityCode > 0 ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                        CustomerId = x.CustomerId,
                        Email = x.Email,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Phone = x.Phone,
                        Status = x.Status
                    };
                }
            }
            return p;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("UpdateCustomer/{id}")]
        public async Task<ActionResult<bool>> UpdateCustomer(CustomerVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await customersBL.GetCustomerByID(p.CustomerId);
                if (cus != null)
                {
                    cus.Phone = p.Phone;
                    cus.Status = p.Status;
                    cus.Address = p.Address;
                    cus.CityCode = p.CityCode;
                    if (await customersBL.UpdateCustomer(cus))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("DeleteCustomer/{id}")]
        public async Task<ActionResult<bool>> DeleteCustomer(string id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                if (await customersBL.DeleteCustomer(id))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
