using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryApp.API.Models;

namespace FoodDeliveryApp.API.Services.Interfaces
{
    public interface IAuthenticateService
    {
        AspNetUserVM Authenticate(AspNetUserVM vm);
    }
}
