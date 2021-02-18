using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using FoodDeliveryApp.API.Helpers.Interfaces;
using FoodDeliveryApp.API.Helpers;

namespace FoodDeliveryApp.API.Helpers
{
    public class Utilities : IUtilities
    {

        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUrlHelper urlHelper;
        //private readonly IProvidersBL providersBL;
        public Utilities(IWebHostEnvironment _hostingEnvironment,
            IHttpContextAccessor _httpContextAccessor,
            //IProvidersBL _providersBL,
            IUrlHelper _urlHelper)
        {
            urlHelper = _urlHelper;
            hostingEnvironment = _hostingEnvironment;
            httpContextAccessor = _httpContextAccessor;
            //providersBL = _providersBL;
        }

        /// <summary>
        /// This method gets the path to save an ItemList,
        /// it takes the folder name to save the ItemList in without
        /// backward slashes and the file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetPath(string dir, string fileName)
        {
            string path = hostingEnvironment.WebRootPath + "\\" + dir + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string npath = path + fileName;
            return npath;
        }

        /// <summary>
        /// This method gets the path to save an ItemList,
        /// it takes the folder name to save the ItemList in without
        /// backward slashes and the file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetPathDirectory(string dir)
        {
            string path = hostingEnvironment.WebRootPath + "\\" + dir + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// This method ensures that file name is correct
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string EnsureFileName(string fileName)
        {
            if (fileName.Contains("\\"))
            {
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            }
            return fileName;
        }

        /// <summary>
        /// This Method returns the month equivalent of the month from selected date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetMonthIntegreEquivalent(string month)
        {
            int intMonth = 0;
            switch (month)
            {
                case "December":
                    intMonth = 12;
                    break;
                case "January":
                    intMonth = 1;
                    break;
                case "February":
                    intMonth = 2;
                    break;
                case "March":
                    intMonth = 3;
                    break;
                case "April":
                    intMonth = 4;
                    break;
                case "May":
                    intMonth = 5;
                    break;
                case "June":
                    intMonth = 6;
                    break;
                case "July":
                    intMonth = 7;
                    break;
                case "August":
                    intMonth = 8;
                    break;
                case "September":
                    intMonth = 9;
                    break;
                case "October":
                    intMonth = 10;
                    break;
                case "November":
                    intMonth = 11;
                    break;
                default:
                    intMonth = 0;
                    break;
            }
            return intMonth;
        }

        /// <summary>
        /// This Method returns the status true if the given date month is greater than the selected month
        /// used for selection in the GeneratePayroll method
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool GetNextThreeMonthsStatus(DateTime date, string month)
        {
            bool status = false;
            if (month != null && month == "December")
            {
                if (date.Month <= 2)
                    return true;
            }
            else
            {
                if (date.Month >= GetMonthIntegreEquivalent(month))
                    return true;
            }
            return status;
        }
        /// <summary>
        /// This method sends an email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SendMail(string Receiver = null, string body = null, string attachmentUrl = null, string subject = null)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(Receiver) && !string.IsNullOrEmpty(body))
            {
                string From = "info@medicsetal.com"; //example:- sourabh9303@gmail.com
                string PW = "Pa$$w00rd";
                try
                {
                    using (MailMessage mail = new MailMessage(From, Receiver))
                    {
                        mail.Subject = subject;

                        StringBuilder sb = new StringBuilder();
                        sb.Append(body);
                        mail.Body = sb.ToString();
                        if (!string.IsNullOrEmpty(attachmentUrl))
                        {
                            string fileName = Path.GetFileName(attachmentUrl);
                            mail.Attachments.Add(new Attachment(attachmentUrl));
                        }

                        mail.IsBodyHtml = true;

                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Host = "5.77.48.66";
                        smtpClient.Port = 25;
                        smtpClient.EnableSsl = false;
                        smtpClient.Credentials = new NetworkCredential(From, PW);

                        ServicePointManager.ServerCertificateValidationCallback =
                        delegate (object s, X509Certificate certificate,
                            X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        { return true; };
                        try
                        {
                            smtpClient?.Send(mail);
                            result = true;
                            //client.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// This method builts an email template
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string EmailTemplate(string urlLink = null, string text = null, string emailview = null, string name = null)
        {
            try
            {
                string mxg = null;
                if (!string.IsNullOrEmpty(emailview))
                {
                    string body = System.IO.File.ReadAllText(Path.Combine("EmailTemplate/") + emailview + ".cshtml");
                    if (!string.IsNullOrEmpty(urlLink))
                    {
                        var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                        var logoUrl = Path.Combine(currentUrl, "images", "printheader.png");

                        body = body.Replace("@ViewBag.Url", name).Replace("@ViewBag.Text", text).Replace("@ViewBag.Name", urlLink);
                    }
                    body = body.ToString();
                    mxg = body;
                }
                return mxg;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
		/// This method converts the patient report to pdf and sends it to the referrer
		/// </summary>
		/// <param name="url"></param>
		/// 
		[AllowAnonymous]
        public async Task<string> ConvertToPdf(int id, string action, string controller)
        {
            string targetPath = "";
            //if (id > 0)
            //{
            //    try
            //    {
            //        var p = await providersBL.GetProviderByProviderId(id);
            //        string Name = p.Firstname + "_" + p.Lastname;
            //        string fileName = GenerateGuid();

            //        // instantiate the html to pdf converter
            //        HtmlToPdf converter = new HtmlToPdf();

            //        converter.Options.PdfPageSize = PdfPageSize.Legal;
            //        converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            //        converter.Options.MarginBottom = 1;
            //        converter.Options.MarginLeft = 1;
            //        converter.Options.MarginRight = 1;
            //        converter.Options.MarginTop = 1;
            //        // convert the url to pdf

            //        string urlBuilder = UrlHelperExtended.AbsoluteAction(urlHelper, action, controller, new { id = id });

            //        string url = urlBuilder.ToString();
            //        url = url.Replace("%3F", "?");
            //        PdfDocument doc = converter.ConvertUrl(url);
            //        // save pdf document
            //        string targetFolder = GetPathDirectory("uploads/forms");
            //        targetPath = Path.Combine(targetFolder, fileName + ".pdf");
            //        //Read and Write permission check
            //        var permission = new FileIOPermission(FileIOPermissionAccess.Write, targetFolder);
            //        var permissionSet = new PermissionSet(PermissionState.None);
            //        permissionSet.AddPermission(permission);
            //        if (!permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
            //        {
            //            // You have write permission for the given folder
            //            doc.Save(targetPath);
            //        }

            //        // close pdf document
            //        doc.Close();
            //        return targetPath;
            //    }
            //    catch (Exception ex)
            //    {
            //        //return targetPath;
            //        throw ex;
            //    }
            //}
            return targetPath;
        }
        /// <summary>
        /// This method generates a new guid
        /// </summary>
        /// <returns></returns>
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
