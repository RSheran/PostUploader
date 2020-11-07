using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PostUploader.Common
{
    public class ErrorLogHelper
    {
             
        public static void UpdatingErrorLog(string schema, string uName, Exception ex)
        {
            var logFilePath = System.Configuration.ConfigurationSettings.AppSettings["ErrorLogPath"];

            string exString = String.Empty;

            if (ex.InnerException == null)
            {
                exString = ex.Message.ToString();

            }
            else if (ex.InnerException.InnerException != null)
            {
                exString = ex.InnerException.InnerException.GetBaseException().ToString();
            }
            else
            {
                exString = ex.InnerException.ToString();
            }


            if (Directory.Exists(logFilePath))
            {
                var FileName = "HS_API_ErrorLog" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".txt";
                var fullPath = logFilePath + FileName;
                string errorLogContent = "-----------------------------------------------------------------------" + Environment.NewLine +
                                            "Time Stamp     : " + DateTime.Now.ToLongTimeString() + Environment.NewLine +
                                            "Schema         : " + schema + Environment.NewLine +
                                            "Username       : " + uName + Environment.NewLine +
                                            "Exception/Error: " + exString + Environment.NewLine +
                                            "---------------------------------------------------------------------" + Environment.NewLine;

                //if (File.Exists(fullPath))
                //{

                File.AppendAllText(fullPath, errorLogContent);

                //}
                //else
                //{

                //    UpdatingErrorLog(schema,uName,exception);
                //}
            }

               

        }

       

    }
}
