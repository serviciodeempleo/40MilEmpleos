using IdentitySample.Models;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using MilEmpleos;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System;
using System.Net.Mail;



namespace IdentitySample
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
 
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute()); // this line is the culprit
        }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //force json
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            

         }

        void Application_Error(object sender, EventArgs e)
        {

            try
            {
                //Obtenemos el error
                Exception ex = Server.GetLastError();

                //Formateamos el correo con los datos del error
                string bodyMail = "Error en Aplicación<br/>";
                bodyMail += "<br/>Fecha: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                bodyMail += "<br/>Descripción: " + ex.Message;
                bodyMail += "<br/>Origen: " + ex.Source;
                if (ex.InnerException != null)
                {
                    bodyMail += "<br/> <br/> Inner Exception Type: ";
                    bodyMail +=ex.InnerException.GetType().ToString();
                    bodyMail += "<br/><br/> Inner Exception: ";
                    bodyMail +=ex.InnerException.Message;
                    bodyMail += "<br/><br/> Inner Source: ";
                    bodyMail += ex.InnerException.Source;
                    if (ex.InnerException.StackTrace != null)
                    {
                        bodyMail += "<br/><br/> Inner Stack Trace: ";
                        bodyMail += ex.InnerException.StackTrace;
                    }
                }
                bodyMail += ("<br/><br/> Exception Type: ");
                bodyMail += (ex.GetType().ToString());
                bodyMail += ("<br/><br/> Exception: " + ex.Message);
                bodyMail += ("<br/><br/> Source: " + ex.Source);
                
                if (ex.StackTrace != null)
                {
                    bodyMail += ("<br/><br/> Stack Trace:");
                    bodyMail += (ex.StackTrace);
                }
                if (ex.TargetSite != null)
                {
                    bodyMail += ("<br/><br/> Target Site: ");
                    bodyMail += (ex.TargetSite);
                }
                if (ex.HelpLink != null)
                {
                    bodyMail += ("<br/><br/> Help Link: <br/>");
                    bodyMail += (ex.HelpLink);
                }
                if (ex.GetBaseException() != null)
                {
                    bodyMail += ("<br/><br/> Base Exception: <br/>");
                    bodyMail += (ex.GetBaseException());
                }
                
                System.Net.Mail.MailMessage oMsg = new System.Net.Mail.MailMessage();
                oMsg.From = new System.Net.Mail.MailAddress("erro40milEmpleos@serviciodeempleo.gov.co");
                oMsg.To.Add("iperilla.ext@serviciodeempleo.gov.co");
                oMsg.Subject = "Error en Aplicación 40MilEmpleos ";
                oMsg.Body = bodyMail;
                oMsg.IsBodyHtml = true;

                System.Net.Mail.SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                };
                smtp.Credentials = new System.Net.NetworkCredential("iperilla.ext@serviciodeempleo.gov.co", "miesencia51525");

                smtp.Send(oMsg);

                //Limpiamos el error 
                Server.ClearError();
            }
            catch (Exception errorEnEnvioDeError)
            {
                //En caso de fallar el envío de error podíamos probar un método alternativo como guardar "bodyMail" en un fichero de texto o en base de datos
                string msg = "Mail cannot be sent:";
                msg += errorEnEnvioDeError.Message;               
            }
        }
    }
}
