using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FoodDeliveryApp.API.Models;
using FoodDeliveryApp.BL.BusinessLogic.Interfaces;
using FoodDeliveryApp.BL.DVM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FoodDeliveryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesCartItemsController : ControllerBase
    {
        private readonly IUsersBL usersBL;
        private readonly ISetupsBL setupBL;
        private readonly TextInfo textInfo;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ITransactionsBL transactionsBL;
        private readonly ICustomersBL customersBL;
        private readonly ICartItemsBL cartItemsBL;

        public SalesCartItemsController(IConfiguration _configuration,
            IUsersBL _usersBL,
            ICustomersBL _customersBL,
            ITransactionsBL _transactionsBL,
             IWebHostEnvironment _hostingEnvironment,
            ISetupsBL _setupBL,
            ICartItemsBL _cartItemsBL)
        {
            usersBL = _usersBL;
            setupBL = _setupBL;
            configuration = _configuration;
            textInfo = new CultureInfo("en-US", false).TextInfo;
            hostingEnvironment = _hostingEnvironment;
            transactionsBL = _transactionsBL;
            customersBL = _customersBL;
            cartItemsBL = _cartItemsBL;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllCartItems")]
        public async Task<ActionResult<List<CartItemVM>>> GetAllCartItems()
        {
            var cus = await cartItemsBL.GetAllCartItems();
            return cus.Select(x => new CartItemVM
            {
                CartId = x.CartId,
                CustomerId = x.CustomerId,
                DateCreated = x.DateCreated,
                Discount = x.Discount,
                ItemId = x.ItemId,
                MealId = x.MealId,
                MealName = setupBL.GetMealByID(x.MealId).Result.Name,
                Quantity = x.Quantity,
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetCartItem/{id}")]
        public async Task<ActionResult<CartItemVM>> GetCartItem(string id)
        {
            CartItemVM p = new CartItemVM();
            if (!string.IsNullOrEmpty(id))
            {
                var x = await cartItemsBL.GetCartItemByID(id);
                if (x != null)
                {
                    p = new CartItemVM
                    {
                        CartId = x.CartId,
                        CustomerId = x.CustomerId,
                        DateCreated = x.DateCreated,
                        Discount = x.Discount,
                        ItemId = x.ItemId,
                        MealId = x.MealId,
                        MealName = setupBL.GetMealByID(x.MealId).Result.Name,
                        Quantity = x.Quantity,
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
        [HttpGet("UpdateCartItem/{id}")]
        public async Task<ActionResult<bool>> UpdateCartItem(CartItemVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await cartItemsBL.GetCartItemByID(p.ItemId);
                if (cus != null)
                {
                    cus.Quantity = p.Quantity;
                    if (await cartItemsBL.UpdateCartItem(cus))
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
        [HttpGet("DeleteCartItem/{id}")]
        public async Task<ActionResult<bool>> DeleteCartItem(string id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                if (await cartItemsBL.DeleteCartItem(id))
                {
                    result = true;
                }
            }
            return result;
        }

        // POST api/Transactions/AddOrder
        [HttpPost]
        [Route("AddOrder")]
        public async Task<RequestResult> AddOrder(OrderVM model)
        {
            if (!ModelState.IsValid)
            {
                return new RequestResult { Message = "All fields are required", Status = false };
            }
            if (model.OrderId == 0)
            {
                var cust = await customersBL.GetCustomerByID(model.CustomerId);
                var order = new OrderDVM
                {
                    CustomerId = model.CustomerId,
                    DateCreated = DateTime.Now,
                    CityCode = cust.CityCode,
                    Discount = model.Discount,
                    IsCheckout = false,
                    OrderStatus = "New",
                    ShippingAddress = model.ShippingAddress,
                    TotalAmount = model.TotalAmount,

                };
                //var cid = await customersBL.AddCustomer(cus);
                //var user = usersBL.GetUserByEmail(model.Email);
                //if (!string.IsNullOrEmpty(cid) && user == null)
                //{
                //    AspNetUserDVM usern = new AspNetUserDVM
                //    {
                //        Email = model.Email.ToLower(),
                //        UserName = model.Email.ToLower(),
                //        Password = model.Password
                //    };

                //    bool result = await usersBL.AddUser(usern, "Customer");
                //    if (result)
                //    {
                //        return new RequestResult { Message = "Account created successfully.", Status = true };
                //    }
                //}
                return new RequestResult { Message = "Unable to create account. Please try again.", Status = false };
            }
            else
            {
                return new RequestResult { Message = "Email address already taken. Please try again with another email.", Status = false };
            }
        }

    }
}
