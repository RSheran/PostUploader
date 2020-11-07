using PostUploader.common;
using PostUploader.Common;
using PostUploader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace PostUploader.Controllers
{
    public class LoginController : Controller
    {
        public string currentFile = String.Empty, methodName = String.Empty;
        string apiStartURL = System.Configuration.ConfigurationManager.AppSettings["APIStartURL"].ToString();
        // GET: Login
        public ActionResult Index()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now.AddSeconds(-1);
                Response.Cookies.Add(authCookie);
            }
            return View();
        }

        public ActionResult Authenticate(LoginViewModel model)
        {
            try
            {
                if ((model.Username != null && model.Password != null))
                {
                    if (LoginAuth(model.Username, model.Password))
                    {                                               
                        return RedirectToAction("Index", "PostUploader");                    
                                                
                    }
                    else
                    {

                        //ModelState.AddModelError("", "The user name or password provided is incorrect.");

                        //return View(model);
                        this.HttpContext.Session["ErrorMsg"] = "LoginErr";
                        //eturn RedirectToAction("Action2");
                        //modified by ranga 9/9/2015               
                        //   return RedirectToAction("Index", "LoginError");
                        //return Content("<script language='javascript' type='text/javascript'>alert('Invalid username or password.');window.location.href='../Login/'</script>");
                        //  return ActionResult()
                        TempData["notice"] = "Username-Pasword Combination  Is Incorrect.";
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    TempData["notice"] = "Username and Password are required.";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                currentFile = this.ControllerContext.RouteData.Values["controller"].ToString(); // System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(0);
                methodName = sf.GetMethod().Name;
                ErrorLogHelper.UpdatingErrorLog(currentFile + "-" + methodName, "N/A", ex);
                return Json("");
            }

        }


        private bool LoginAuth(string UserName, string Password)
        {
            try
            {
                var isExistingUser = IsExistingUser(UserName, Password);
                if (true)
                {

                    var authTicket = new FormsAuthenticationTicket(1,
                         UserName,
                         DateTime.Now,
                         DateTime.Now.AddMinutes(20),
                         false,
                         "test"
                         );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);


                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        class UserValidationMessage
        {
            public string ValidationMessage { get; set; }
        }

        private async Task<bool> IsExistingUser(string UserName, string Password)
        {
            try
            {
             
                string checkPwd = "",resultMsg=String.Empty;
                JavaScriptSerializer _serializer = new JavaScriptSerializer();

                using (MD5 md5Hash = MD5.Create())
                {
                    HashingModule hm = new HashingModule();
                    checkPwd = hm.GetMd5Hash(md5Hash, Password);
                }

                using (HttpClient client = new HttpClient())
                {
                    
                    string apiCallURL = apiStartURL + "IsExistingUser?username="+UserName+"&password="+checkPwd;
                    var response = await client.GetAsync(apiCallURL);
                    if (response != null)
                    {

                        resultMsg = await response.Content.ReadAsStringAsync();
                        var jsonString = await response.Content.ReadAsStringAsync();
                        resultMsg = _serializer.Deserialize<UserValidationMessage>(jsonString).ValidationMessage;

                    }
                    else
                    {
                        return false;
                    }
                }

                if (resultMsg!="" && resultMsg.Length>0)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Check whether username is existing
        public async Task<JsonResult> IsUsernameExisting(string chkUserName)
        {
            try
            {
                string resultMsg = String.Empty;
                JavaScriptSerializer _serializer = new JavaScriptSerializer();

                using (HttpClient client = new HttpClient())
                {

                    string apiCallURL = apiStartURL + "IsUserNameExisting?username=" + chkUserName;
                    var response = await client.GetAsync(apiCallURL);
                    if (response != null)
                    {

                        resultMsg = await response.Content.ReadAsStringAsync();
                        var jsonString = await response.Content.ReadAsStringAsync();
                        resultMsg = _serializer.Deserialize<UserValidationMessage>(jsonString).ValidationMessage;

                    }
                    else
                    {
                        return Json(false);
                    }
                }

                if (resultMsg != "" && resultMsg.Length > 0)
                {
                    return Json(true);

                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                return Json(false);
            }

        }
    }
}