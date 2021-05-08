using System;
using System.Diagnostics;
using System.Net.Mail;

namespace NotesMarketPlaces.SendMail
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
            client.Credentials = new System.Net.NetworkCredential("**********@gmail.com", "********");
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