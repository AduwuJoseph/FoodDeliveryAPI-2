using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryApp.API.Helpers
{
    public static class Helper
    {
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
    public class ItemMultiple
    {
        public string sid1 { get; set; }
        public string sid2 { get; set; }
        public int iid1 { get; set; }
        public int iid2 { get; set; }
        public int iid3 { get; set; }
        public bool bid1 { get; set; }
        public bool bid2 { get; set; }
        public bool bid3 { get; set; }
        public bool bid4 { get; set; }
        public string did1 { get; set; }
        public List<string> sList1 { get; set; }

    }
    /// <summary>
    /// This class is used to define days so that we can have it set in the dropdown list
    /// </summary>
    public class Day
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// This class is used to set dropdown.
    /// It is used in other to limit the amount of data passed into a dropdown.
    /// </summary>
    public class Item
    {
        
        public object Id { get; set; }

        public object Name { get; set; }
    }

    /// <summary>
    /// This class sets and returns the actual days to be returned in the dropdown
    /// </summary>
    public static class Days
    {
        public static List<Day> GetMonths
        {
            get
            {
                return new List<Day>
                {
                    new Day{ Id = "January", Name="January"},
                    new Day{ Id = "February", Name="February"},
                    new Day{ Id = "March", Name="March"},
                    new Day{ Id = "April", Name="April"},
                    new Day{ Id = "May", Name="May"},
                    new Day{ Id = "June", Name="June"},
                    new Day{ Id = "July", Name="July"},
                    new Day{ Id = "August", Name="August"},
                    new Day{ Id = "September", Name="September"},
                    new Day{ Id = "October", Name="October"},
                    new Day{ Id = "November", Name="November"},
                    new Day{ Id = "December", Name="December"}
                };
            }
        }

        public static List<Day> GetDays
        {
            get
            {
                return new List<Day> 
                {
                    new Day{ Id="Mondays", Name="Mondays"},
                    new Day{ Id="Tuesdays", Name="Tuesdays"},
                    new Day{ Id="Wednesdays", Name="Wednesdays"},
                    new Day{ Id="Thursdays", Name="Thursdays"},
                    new Day{ Id="Fridays", Name="Fridays"},
                    new Day{ Id="Saturdays", Name="Saturdays"},
                    new Day{ Id="Sundays", Name="Sundays"},
                    new Day{ Id="After 2 Days", Name="After 2 Days"},
                    new Day{ Id="After 3 Days", Name="After 3 Days"},
                    new Day{ Id="After 4 Days", Name="After 4 Days"},
                    new Day{ Id="After 5 Days", Name="After 5 Days"},
                    new Day{ Id="After 6 Days", Name="After 6 Days"},
                    new Day{ Id="After 7 Days", Name="After 7 Days"},
                    new Day{ Id="After 8 Days", Name="After 8 Days"},
                    new Day{ Id="After 9 Days", Name="After 9 Days"}
                };
            }
        }

        public static List<Day> GetAllStatus
        {
            get
            {
                return new List<Day>
                {
                    new Day{Id = "Small", Name="Small"},
                    new Day{Id = "Average", Name="Average"},
                    new Day{Id = "Large", Name="Large"}
                };
            }
        }

        public static List<Day> GetAccountTypes
        {
            get
            {
                return new List<Day>
                {
                    new Day{Id = "Current", Name="Current"},
                    new Day{Id = "Savings", Name="Savings"}
                };
            }
        }
    }

    /// <summary>
    /// This static class returns the different status of a user
    /// </summary>
    public static class Status
    {
        public static string Active { get { return "Active"; } }
        public static string Inactive { get { return "Inactive"; } }

    }
    /// <summary>
    /// <see cref="IUrlHelper"/> extension methods.
    /// </summary>
    public static class UrlHelperExtended
    {
        /// <summary>
        /// Generates a fully qualified URL to an action method by using the specified action name, controller name and
        /// route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object routeValues = null)
        {
            return url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified content by using the specified content path. Converts a
        /// virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteContent(
            this IUrlHelper url,
            string contentPath)
        {
            HttpRequest request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified route by using the route name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public static string AbsoluteRouteUrl(
            this IUrlHelper url,
            string routeName,
            object routeValues = null)
        {
            return url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }
    }
}
