using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryApp.API.Helpers.Interfaces
{
    public interface IUtilities
    {
        [AllowAnonymous]
        Task<string> ConvertToPdf(int id, string action, string controller);
        string EmailTemplate(string urlLink = null, string text = null, string emailview = null, string name = null);
        string EnsureFileName(string fileName);
        string GenerateGuid();
        int GetMonthIntegreEquivalent(string month);
        bool GetNextThreeMonthsStatus(DateTime date, string month);
        string GetPath(string dir, string fileName);
        string GetPathDirectory(string dir);
        bool SendMail(string Receiver = null, string body = null, string attachmentUrl = null, string subject = null);
    }
}
