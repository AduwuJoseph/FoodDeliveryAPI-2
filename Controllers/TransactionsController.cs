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
    public class TransactionsController : ControllerBase
    {
        private readonly IUsersBL usersBL;
        private readonly ISetupsBL setupBL;
        private readonly TextInfo textInfo;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ITransactionsBL transactionsBL;
        private readonly ICustomersBL customersBL;

        public TransactionsController(IConfiguration _configuration,
            IUsersBL _usersBL,
            ICustomersBL _customersBL,
            ITransactionsBL _transactionsBL,
             IWebHostEnvironment _hostingEnvironment,
            ISetupsBL _setupBL)
        {
            usersBL = _usersBL;
            setupBL = _setupBL;
            configuration = _configuration;
            textInfo = new CultureInfo("en-US", false).TextInfo;
            hostingEnvironment = _hostingEnvironment;
            transactionsBL = _transactionsBL;
            customersBL = _customersBL;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<List<OrderVM>>> GetAllOrders()
        {
            var cus = await transactionsBL.GetAllOrders();
            return cus.Select(x => new OrderVM
            {
                OrderId = x.OrderId,
                CityCode = x.CityCode,
                CityName = x.CityCode != null ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                CustomerId = x.CustomerId,
                DateCreated = x.DateCreated,
                Discount = x.Discount,
                IsCheckout = x.IsCheckout,
                OrderStatus = x.OrderStatus,
                RevisionNumber = x.RevisionNumber,
                ShippedDate = x.ShippedDate,
                ShippingAddress = x.ShippingAddress,
                Tax = x.Tax,
                TotalAmount = x.TotalAmount,
                Vat = x.Vat,
                CustomerName = customersBL.GetCustomerByID(x.CustomerId).Result.FullName,
                
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetOrder/{id}")]
        public async Task<ActionResult<OrderVM>> GetOrder(string id)
        {
            OrderVM p = new OrderVM();
            if (!string.IsNullOrEmpty(id))
            {
                var x = await transactionsBL.GetOrderByID(id);
                if (x != null)
                {
                    var xx = await customersBL.GetCustomerByID(x.CustomerId);
                    var customer = new CustomerVM
                    {
                        DateCreated = xx.DateCreated,
                        Address = xx.Address,
                        CityCode = xx.CityCode,
                        CityName = xx.CityCode > 0 ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                        CustomerId = xx.CustomerId,
                        Email = xx.Email,
                        FirstName = xx.FirstName,
                        LastName = xx.LastName,
                        Phone = xx.Phone,
                        Status = xx.Status
                    };
                    p = new OrderVM
                    {
                        OrderId = x.OrderId,
                        CityCode = x.CityCode,
                        CityName = x.CityCode != null ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                        CustomerId = x.CustomerId,
                        DateCreated = x.DateCreated,
                        Discount = x.Discount,
                        IsCheckout = x.IsCheckout,
                        OrderStatus = x.OrderStatus,
                        RevisionNumber = x.RevisionNumber,
                        ShippedDate = x.ShippedDate,
                        ShippingAddress = x.ShippingAddress,
                        Tax = x.Tax,
                        TotalAmount = x.TotalAmount,
                        Vat = x.Vat,
                        OrderDetails = transactionsBL.GetOrderDetailsByOrderID(x.OrderId).Result.Select(m => new OrderDetailVM
                        {
                            Discount = m.Discount,
                            MealId = m.MealId,
                            MealName = setupBL.GetMealByID(m.MealId).Result.Name,
                            OrderId = m.OrderId,
                            Price = m.Price,
                            Quantity = m.Quantity,
                        }).ToList(),
                    };
                    p.Customer = customer;
                }
            }
            return p;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("UpdateOrder/{id}")]
        public async Task<ActionResult<bool>> UpdateOrder(OrderVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await transactionsBL.GetOrderByID(p.OrderId);
                if (cus != null)
                {
                    if (p.ShippedDate != null)
                    {
                        cus.ShippedDate = p.ShippedDate;
                    }
                    if (p.OrderStatus != null)
                    {
                        cus.OrderStatus = p.OrderStatus;
                    }
                    if (p.ShippingAddress != null)
                    {
                        cus.ShippingAddress = p.ShippingAddress;
                    }
                    if (p.TotalAmount != null)
                    {
                        cus.TotalAmount = p.TotalAmount;
                    }
                    if (p.Discount != null)
                    {
                        cus.Discount = p.Discount;
                    }
                    if (p.CityCode != null)
                    {
                        cus.CityCode = p.CityCode;
                    }
                        cus.IsCheckout = p.IsCheckout;
                    if (await transactionsBL.UpdateOrder(cus))
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
        [HttpGet("DeleteOrder/{id}")]
        public async Task<ActionResult<bool>> DeleteOrder(int id)
        {
            bool result = false;
            if (id > 0)
            {
                if (await transactionsBL.DeleteOrder(id))
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllOrderDetails")]
        public async Task<ActionResult<List<OrderDetailVM>>> GetAllOrderDetails()
        {
            var cus = await transactionsBL.GetAllOrderDetails();
            return cus.Select(m => new OrderDetailVM
            {
                Discount = m.Discount,
                MealId = m.MealId,
                MealName = setupBL.GetMealByID(m.MealId).Result.Name,
                OrderId = m.OrderId,
                Price = m.Price,
                Quantity = m.Quantity,
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetOrderDetail/{id}")]
        public async Task<ActionResult<OrderDetailVM>> GetOrderDetail(int oid, int mid)
        {
            OrderDetailVM p = new OrderDetailVM();
            if (oid > 0 && mid > 0)
            {
                object[] id = { oid, mid };
                var m = await transactionsBL.GetOrderDetailByID(id);
                if (m != null)
                {
                    p = new OrderDetailVM
                    {
                        Discount = m.Discount,
                        MealId = m.MealId,
                        MealName = setupBL.GetMealByID(m.MealId).Result.Name,
                        OrderId = m.OrderId,
                        Price = m.Price,
                        Quantity = m.Quantity,
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
        [HttpGet("UpdateOrderDetail/{id}")]
        public async Task<ActionResult<bool>> UpdateOrderDetail(OrderDetailVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                object[] id = { p.OrderId, p.MealId };
                var cus = await transactionsBL.GetOrderDetailByID(id);
                if (cus != null)
                {
                    cus.Quantity = p.Quantity;
                    cus.Price = p.Price;
                    if (await transactionsBL.UpdateOrderDetail(cus))
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
        [HttpGet("DeleteOrderDetail/{id}")]
        public async Task<ActionResult<bool>> DeleteOrderDetail(int oid, int mid)
        {
            bool result = false;
            if (oid > 0 && mid > 0)
            {
                object[] id = { oid, mid };
                if (await transactionsBL.DeleteOrderDetail(id))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllPayments")]
        public async Task<ActionResult<List<PaymentVM>>> GetAllPayments()
        {
            var cus = await transactionsBL.GetAllPayments();
            return cus.Select(x => new PaymentVM
            {
                TransactionDate = x.TransactionDate,
                TransactionRef = x.TransactionRef,
                Amount = x.Amount,
                OrderId = x.OrderId,
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetPayment/{id}")]
        public async Task<ActionResult<PaymentVM>> GetPayment(string id)
        {
            PaymentVM p = new PaymentVM();
            if (!string.IsNullOrEmpty(id))
            {
                var x = await transactionsBL.GetPaymentByID(id);
                if (x != null)
                {
                    p = new PaymentVM
                    {
                        TransactionDate = x.TransactionDate,
                        TransactionRef = x.TransactionRef,
                        Amount = x.Amount,
                        OrderId = x.OrderId,
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
        [HttpGet("UpdatePayment/{id}")]
        public async Task<ActionResult<bool>> UpdatePayment(PaymentVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await transactionsBL.GetPaymentByID(p.TransactionRef);
                if (cus != null)
                {
                    cus.Amount = p.Amount;
                    cus.TransactionDate = p.TransactionDate;
                    if (await transactionsBL.UpdatePayment(cus))
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
        [HttpGet("DeletePayment/{id}")]
        public async Task<ActionResult<bool>> DeletePayment(string id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                if (await transactionsBL.DeletePayment(id))
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
                            Discount = item.Discount != null ? Convert.ToDecimal(item.Discount) : 0,
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


        // POST api/Transactions/AddOrder
        [HttpPost]
        [Route("AddPayment")]
        public async Task<RequestResult> AddPayment(PaymentVM model)
        {
            if (!ModelState.IsValid)
            {
                return new RequestResult { Message = "All fields are required", Status = false };
            }
            if (model.TransactionRef != null && model.OrderId > 0)
            {
                var pay = new PaymentDVM
                {
                    TransactionRef = model.TransactionRef,
                    TransactionDate = model.TransactionDate,
                    OrderId = model.OrderId,
                    Amount = model.Amount,
                };
                var payid = await transactionsBL.AddPayment(pay);
                
                if (!string.IsNullOrEmpty(payid))
                {
                    return new RequestResult { Message = "Payment was successfully done.", Status = true };
                }
            }
            return new RequestResult { Message = "An error occured, payment could not be completed.", Status = true };
        }

    }
}
