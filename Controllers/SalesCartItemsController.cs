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
                return new RequestResult { Order = model, Message = "All fields are required", Status = false };
            }
            OrderVM vm = new OrderVM();
            
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
                    TotalAmount = 0,

                };
                var oid = await transactionsBL.AddOrder(order);
                decimal total = 0;
                decimal discount = 0;
                if (oid > 0)
                {
                    string result = string.Empty;
                    foreach (var item in model.CartItems)
                    {
                        var meal = await setupBL.GetMealByID(item.MealId);
                        var detailsDVM = new OrderDetailDVM
                        {
                            OrderId = oid,
                            MealId = Convert.ToInt32(item.MealId),
                            Discount = item.Discount != null? Convert.ToDecimal(item.Discount): 0,
                            Price = meal.Price,
                            Quantity = Convert.ToInt16(item.Quantity),
                        };
                        discount = item.Discount != null ? Convert.ToDecimal(item.Discount) : 0;
                       
                        total += Convert.ToDecimal(item.Quantity * meal.Price) - discount;
                        result = await transactionsBL.AddOrderDetail(detailsDVM);
                    }
                    order = await transactionsBL.GetOrderByID(oid);
                    order.TotalAmount = total;
                    await transactionsBL.UpdateOrder(order);
                    vm = new OrderVM
                    {
                        CustomerId = order.CustomerId,
                        CityCode = order.CityCode,
                        Discount = order.Discount,
                        IsCheckout = false,
                        OrderStatus = "New",
                        ShippingAddress = order.ShippingAddress,
                        TotalAmount = total,
                    };
                    if (!string.IsNullOrEmpty(result))
                    {
                        return new RequestResult { Order = vm, Message = "Order successfully created.", Status = true };
                    }
                }
                return new RequestResult { Order = model, Message = "Order failed to create. Please try again.", Status = false };
            }
            else
            {
                var order = await transactionsBL.GetOrderByID(model.OrderId);

                string result = string.Empty;
                decimal total = 0;
                decimal discount = 0;
                foreach (var item in model.CartItems)
                {
                    var meal = await setupBL.GetMealByID(item.MealId);
                    object[] id = { model.OrderId, item.MealId };
                    var detchk = await transactionsBL.GetOrderDetailByID(id);
                    if (detchk == null)
                    {
                        var detailsDVM = new OrderDetailDVM
                        {
                            OrderId = model.OrderId,
                            MealId = Convert.ToInt32(item.MealId),
                            Discount = item.Discount != null ? Convert.ToDecimal(item.Discount) : 0,
                            Price = meal.Price,
                            Quantity = Convert.ToInt16(item.Quantity),
                        };
                        discount = item.Discount != null ? Convert.ToDecimal(item.Discount) : 0;
                        total += Convert.ToDecimal(item.Quantity * meal.Price) - discount;

                        result = await transactionsBL.AddOrderDetail(detailsDVM);
                    }
                    else
                    {
                        discount = item.Discount != null ? Convert.ToDecimal(item.Discount) : 0;
                        total += Convert.ToDecimal(item.Quantity * meal.Price) - discount;

                        detchk.Quantity = Convert.ToInt16(item.Quantity);
                        bool result2 = await transactionsBL.UpdateOrderDetail(detchk);
                    }
                }
                order.TotalAmount = total;
                await transactionsBL.UpdateOrder(order);
                vm = new OrderVM
                {
                    CustomerId = order.CustomerId,
                    CityCode = order.CityCode,
                    Discount = order.Discount,
                    IsCheckout = false,
                    OrderStatus = "New",
                    ShippingAddress = order.ShippingAddress,
                    TotalAmount = total,
                };
                if (!string.IsNullOrEmpty(result))
                {
                    return new RequestResult { Order = vm, Message = "Order successfully created.", Status = true };
                }
            }
            return new RequestResult { Order = model, Message = "An error occured, process not completed.", Status = true };
        }

    }
}
