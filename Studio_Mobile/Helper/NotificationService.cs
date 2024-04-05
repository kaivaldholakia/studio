using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Studio_Mobile.Helper
{
    public static  class NotificationService
    {
      
        public static  void SendCode(string UserName, string Password, string SenderID, string msg, string mobno, string msgtyp, string entityID, string templateID)
        {
            string result = "";
            WebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                string smsurl = "http://piosys.co.in/SendSMS.aspx?UserName=tpiodemo&Password=tpiodemo&SenderId=COMMSG&Message=Dear Customer, Verification code : "+UserName+" For Studio Registration PS&MobileNo="+mobno.ToString()+"&MsgTyp=T&EntityId=1701158046859603415&TemplateId=1707162080497255983";
                //string smsurl0 = "http://piosys.co.in/SendSMS.aspx?UserName=tpiodemo&Password=tpiodemo&SenderId=COMMSG&Message="+msg+"PS&MobileNo="+mobno.ToString()+"&MsgTyp=T&EntityId=1701158046859603415&TemplateId=1707162080497255983";

                //string smsurl1 = "http://piosys.co.in/SendSMS.aspx?UserName=" + UserName + "&Password=" + Password + "&SenderId=" + SenderID + "&Message=" + msg + "&MobileNo=" + mobno.ToString() + "&MsgTyp=" + msgtyp.ToString() + "&EntityId=1701158046859603415&TemplateId=1707162080497255983";
                
                request = WebRequest.Create(smsurl);
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader reader = new System.IO.StreamReader(stream, ec);
                result = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            catch (Exception exp)
            {
                result = exp.ToString();
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

        public static  void SendMail(string msg, string email)
        {



            // Create an instance of MailMessage and SmtpClient
            MailMessage mymailmsg = new MailMessage();
            SmtpClient Smtp = new SmtpClient();

            // Set the sender's email address and credentials
            string senderEmail = "email2pioneer@gmail.com";
            string senderPassword = "bswslprzydbebgsq"; // Update with your actual password

            // Set up basic authentication information
            NetworkCredential basicAuthenticationInfo = new NetworkCredential(senderEmail, senderPassword);

            // Set up the email message
            mymailmsg.From = new MailAddress(senderEmail);
            mymailmsg.To.Add(new MailAddress(email));
            mymailmsg.Subject = "Forgot Password";
            mymailmsg.IsBodyHtml = true;
            mymailmsg.Body = msg;

            // Configure the SMTP client
            Smtp.Host = "smtp.gmail.com";
            Smtp.Port = 587;
            Smtp.EnableSsl = true;
            Smtp.Credentials = basicAuthenticationInfo;

            try
            {
                // Send the email
                Smtp.Send(mymailmsg);
                // Email sent successfully
            }
            catch (SmtpException ex)
            {
                // Handle any errors
                Console.WriteLine("Error sending email: " + ex.Message);
            }

        }

        public static async Task<string> SendNotification(PushNotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            string query = "";
            try
            {
                try
                {
                    var res = NotifyAsync(notificationModel.DeviceId, notificationModel.Title, notificationModel.Body);

                }
                catch (Exception ex)
                {
                    // _logger.LogError($"Exception thrown in Notify Service: {ex}");
                }

                return query;
            }
            catch (Exception ex)
            {
                query = ex.Message;
                return query;
            }
        }

        public static async Task<bool> NotifyAsync(string to, string title, string body)
        {
            try
            {
                // Get the server key from FCM console
                var serverKey = string.Format("key={0}", "AAAALJbGiN0:APA91bHA8Pt6aM9XcsdBa1F8OvhxLIVgXAbhEeLexkxzjq_P5Ou8jREvfjU8PJoQqNxFMRStiA4qDMq8qGxpgLWMD6B51Fw0jbSLbX5cAg5w0U2_sGaEwEjCnFAPOydxS8p8v-LF3w0f");

                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", "191508154589");



                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body }
                };

                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient())
                    {
                        var result = await httpClient.SendAsync(httpRequest);

                        if (result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError($"Exception thrown in Notify Service: {ex}");
            }

            return false;
        }

    }
}
