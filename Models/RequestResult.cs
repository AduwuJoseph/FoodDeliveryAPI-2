using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryApp.API.Models
{
    public class RequestResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public virtual OrderVM Order { get; set; }
    }
}
