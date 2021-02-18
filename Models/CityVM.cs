using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryApp.API.Models
{
    public class CityVM
    {
        [JsonProperty(PropertyName = "CityCode")]
        public int CityCode { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "ShiipingCost")]
        public decimal ShiipingCost { get; set; }
    }
}
