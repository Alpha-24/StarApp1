using System.Net;
using System.Net.Mail;


namespace StarApp1.Services
{
    public interface ITriggerMail
    {
        void SendMail(string msg,string sub,string email,string name);
    }
    public class TriggerMail:ITriggerMail
    {
        public TriggerMail()
        {

        }

        void ITriggerMail.SendMail(string msg, string sub, string email, string name)
        {


            MailMessage mail = new MailMessage();
            // you need to enter your mail address
            mail.From = new MailAddress("paritosh.sharma@incedoinc.com");

            //To Email Address - your need to enter your to email address
            mail.To.Add(email);

            mail.Subject = sub;

            mail.CC.Add("ankit.kumar3@incedoinc.com");

            mail.IsBodyHtml = true;

            string content = "Hello ," + name;
            content += "<br/>" + msg;
            content += "<br/><br/> Regards , <br/> StarApp Team";
            mail.Body = content;


            //create SMTP instant

            //you need to pass mail server address and you can also specify the port number if you required
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com");

            //Create nerwork credential and you need to give from email address and password
            NetworkCredential networkCredential = new NetworkCredential("paritosh.sharma@incedoinc.com", "Welcome@2021");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = networkCredential;
            smtpClient.Port = 587; // this is default port number - you can also change this

            smtpClient.Send(mail);

        }
            
    }

}
