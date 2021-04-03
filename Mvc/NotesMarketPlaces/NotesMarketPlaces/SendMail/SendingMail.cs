using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace NotesMarketPlaces.Send_Mail
{
    public class SendingMail
    {
        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("rajdaslaniya7@gmail.com", "Raj@12345");
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}