using PostUploader.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Configuration;

namespace PostUploader.Controllers
{
    public class PostUploaderController : Controller
    {
        // GET: PostUploader
        public string _currentFile = String.Empty, _methodName = String.Empty;
        string _apiStartURL = System.Configuration.ConfigurationManager.AppSettings["APIStartURL"].ToString();
        string _applicationClientId = System.Configuration.ConfigurationManager.AppSettings["ApplicationClientId"].ToString();
        string _apiToken = System.Configuration.ConfigurationManager.AppSettings["APIToken"].ToString();

        public ActionResult Index()
        {
            ViewBag.MaxContentLength = System.Configuration.ConfigurationManager.AppSettings["MaxContentLength"];
            return View();          
        }

        //class to deserialize
        class SentimentDeserializer
        {
            public string PhraseSentiment { get; set; }
        }

        public async Task<JsonResult>  PredictSentimentForPhrase(string phraseToUpload)
        {
            try
            {

                var resultString = String.Empty;
                var _serializer = new JavaScriptSerializer();                               

                using (var client = new HttpClient())
                 {
                                        

                      var payloadJson = new
                      {
                          apiToken = _apiToken,
                          applicationId= _applicationClientId,
                          phraseToUpload = phraseToUpload,                        
                        };


                     var payloadJsonStr = _serializer.Serialize(payloadJson);                     
                     var content = new StringContent(payloadJsonStr, Encoding.UTF8, "application/json");
                     var apiCallURL = _apiStartURL + "PredictSentimentForPhrase";
                     var response = await client.PostAsync(apiCallURL, content);
                     if (response != null)
                     {
                         var jsonString = await response.Content.ReadAsStringAsync();
                         var  deserializedString = _serializer.Deserialize<SentimentDeserializer>(jsonString);
                         return Json(deserializedString.PhraseSentiment);
                     }
                     else
                     {
                         return Json("");
                     }
                 }            
            }
            catch (Exception ex)
            {
                _currentFile = this.ControllerContext.RouteData.Values["controller"].ToString(); // System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(0);
                _methodName = sf.GetMethod().Name;
                ErrorLogHelper.UpdatingErrorLog(_currentFile + "-" +_methodName,"N/A", ex);
                return Json("");
            }
        }
    }
}