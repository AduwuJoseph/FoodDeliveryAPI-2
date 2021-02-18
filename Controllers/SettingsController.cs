using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FoodDeliveryApp.API.Models;
using FoodDeliveryApp.BL.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FoodDeliveryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IUsersBL usersBL;
        private readonly ISetupsBL setupBL;
        private readonly TextInfo textInfo;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;

        public SettingsController(IConfiguration _configuration,
            IUsersBL _usersBL,
             IWebHostEnvironment _hostingEnvironment,
            ISetupsBL _setupBL)
        {
            usersBL = _usersBL;
            setupBL = _setupBL;
            configuration = _configuration;
            textInfo = new CultureInfo("en-US", false).TextInfo;
            hostingEnvironment = _hostingEnvironment;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllMealCategories")]
        public async Task<ActionResult<List<CategoryVM>>> GetAllMealCategories()
        {
            var cus = await setupBL.GetAllCategorys();
            return cus.Select(x => new CategoryVM
            {
                CategoryId = x.CategoryId,
                Name = x.Name
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetCategory/{id}")]
        public async Task<ActionResult<CategoryVM>> GetCategory(int id)
        {
            CategoryVM p = new CategoryVM();
            if (id > 0)
            {
                var x = await setupBL.GetCategoryByID(id);
                if (x != null)
                {
                    p = new CategoryVM
                    {
                        CategoryId = x.CategoryId,
                        Name = x.Name
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
        [HttpGet("UpdateCategory/{id}")]
        public async Task<ActionResult<bool>> UpdateCategory(CategoryVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await setupBL.GetCategoryByID(p.CategoryId);
                if (cus != null)
                {
                    cus.Name = p.Name;
                    if (await setupBL.UpdateCategory(cus))
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
        [HttpGet("DeleteCategory/{id}")]
        public async Task<ActionResult<bool>> DeleteCategory(int id)
        {
            bool result = false;
            if (id > 0)
            {
                if (await setupBL.DeleteCategory(id))
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
        [HttpGet("GetAllServiceProviders")]
        public async Task<ActionResult<List<ServiceProviderVM>>> GetAllServiceProviders()
        {
            var cus = await setupBL.GetAllServiceProviders();
            return cus.Select(x => new ServiceProviderVM
            {
                CityCode = x.CityCode,
                CityName = x.CityCode > 0 ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                ProviderId = x.ProviderId,
                Email = x.Email,
                Logo = x.Logo,
                Name = x.Name,
                PhoneNumber = x.PhoneNumber
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetServiceProvider/{id}")]
        public async Task<ActionResult<ServiceProviderVM>> GetServiceProvider(string id)
        {
            ServiceProviderVM p = new ServiceProviderVM();
            if (!string.IsNullOrEmpty(id))
            {
                var x = await setupBL.GetServiceProviderByID(id);
                if (x != null)
                {
                    p = new ServiceProviderVM
                    {
                        CityCode = x.CityCode,
                        CityName = x.CityCode > 0 ? setupBL.GetCityByIDAsync(x.CityCode).Result.Name : "",
                        ProviderId = x.ProviderId,
                        Email = x.Email,
                        Logo = x.Logo,
                        Name = x.Name,
                        PhoneNumber = x.PhoneNumber
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
        [HttpGet("UpdateServiceProvider/{id}")]
        public async Task<ActionResult<bool>> UpdateServiceProvider(ServiceProviderVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await setupBL.GetServiceProviderByID(p.ProviderId);
                if (cus != null)
                {
                    cus.PhoneNumber = p.PhoneNumber;
                    if (!string.IsNullOrEmpty(p.Logo))
                    {
                        cus.Logo = p.Logo;
                    }
                    cus.Name = p.Name;
                    cus.CityCode = p.CityCode;
                    if (await setupBL.UpdateServiceProvider(cus))
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
        [HttpGet("DeleteServiceProvider/{id}")]
        public async Task<ActionResult<bool>> DeleteServiceProvider(string id)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(id))
            {
                if (await setupBL.DeleteServiceProvider(id))
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
        [HttpGet("GetAllMeals")]
        public async Task<ActionResult<List<MealVM>>> GetAllMeals()
        {
            var cus = await setupBL.GetAllMeals();
            return cus.Select(x => new MealVM
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryId != null ? setupBL.GetCategoryByID(x.CategoryId).Result.Name : "",
                Description = x.Description,
                MealId = x.MealId,
                Name = x.Name,
                Picture = x.Picture,
                PrepareTime = x.PrepareTime,
                Price = x.Price,
                ProviderId = x.ProviderId,
                ProviderName = x.ProviderId != null ? setupBL.GetServiceProviderByID(x.ProviderId).Result.Name : ""
            }).ToList();
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMeal/{id}")]
        public async Task<ActionResult<MealVM>> GetMeal(int id)
        {
            MealVM p = new MealVM();
            if (id > 0)
            {
                var x = await setupBL.GetMealByID(id);
                if (x != null)
                {
                    p = new MealVM
                    {
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryId != null ? setupBL.GetCategoryByID(x.CategoryId).Result.Name : "",
                        Description = x.Description,
                        MealId = x.MealId,
                        Name = x.Name,
                        Picture = x.Picture,
                        PrepareTime = x.PrepareTime,
                        Price = x.Price,
                        ProviderId = x.ProviderId,
                        ProviderName = x.ProviderId != null ? setupBL.GetServiceProviderByID(x.ProviderId).Result.Name : ""
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
        [HttpGet("UpdateMeal/{id}")]
        public async Task<ActionResult<bool>> UpdateMeal(MealVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await setupBL.GetMealByID(p.MealId);
                if (cus != null)
                {
                    cus.Price = p.Price;
                    cus.PrepareTime = p.PrepareTime;
                    cus.Description = p.Description;
                    cus.Name = p.Name;
                    cus.CategoryId = p.CategoryId;
                    if (await setupBL.UpdateMeal(cus))
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
        [HttpGet("DeleteMeal/{id}")]
        public async Task<ActionResult<bool>> DeleteMeal(int id)
        {
            bool result = false;
            if (id > 0)
            {
                if (await setupBL.DeleteMeal(id))
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
        [HttpGet("GetAllCitys")]
        public ActionResult<List<CityVM>> GetAllCitys ()
        {
            string contentRootPath = hostingEnvironment.ContentRootPath;
            var JSON = System.IO.File.ReadAllText(contentRootPath + "/json/ng.json");

            List<CityVM> cities = JsonSerializer.Deserialize<List<CityVM>>(JSON);
            return cities;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetAllCities")]
        public async Task<ActionResult<List<CityVM>>> GetAllCities()
        {
            var cus = await setupBL.GetAllCitiesAsync();
            var vm = cus.Select(x => new CityVM
            {
                CityCode = x.CityCode,
                Name = x.Name,
                ShiipingCost = x.ShiipingCost
            }).ToList();
            return vm;
        }

        /// <summary>
        /// This method creates a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetCity/{id}")]
        public async Task<ActionResult<CityVM>> GetCity(int id)
        {
            CityVM p = new CityVM();
            if (id > 0)
            {
                var x = await setupBL.GetCityByIDAsync(id);
                if (x != null)
                {
                    p = new CityVM
                    {
                        CityCode = x.CityCode,
                        Name = x.Name,
                        ShiipingCost = x.ShiipingCost
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
        [HttpGet("UpdateCity/{id}")]
        public async Task<ActionResult<bool>> UpdateCity(CityVM p)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                var cus = await setupBL.GetCityByIDAsync(p.CityCode);
                if (cus != null)
                {
                    cus.Name = p.Name;
                    cus.ShiipingCost = p.ShiipingCost;
                    if (await setupBL.UpdateCityAsync(cus))
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
        [HttpGet("DeleteCity/{id}")]
        public async Task<ActionResult<bool>> DeleteCity(int id)
        {
            bool result = false;
            if (id > 0)
            {
                if (await setupBL.DeleteCityAsync(id))
                {
                    result = true;
                }
            }
            return result;
        }

    }
}
