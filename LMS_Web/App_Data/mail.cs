using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace LMS_Web
{
    public class mail
    {
        public void send(StringBuilder body, string to, string from, string subject)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(to);
            mail.From = new MailAddress(from);
            mail.Subject = subject;
            string Body = body.ToString();
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("abhishekkpr9@gmail.com", "kapoor@123"); // Enter seders User name and password   
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}