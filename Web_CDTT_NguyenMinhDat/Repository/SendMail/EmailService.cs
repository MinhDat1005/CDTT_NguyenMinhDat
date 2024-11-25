using System.Net;
using System.Net.Mail;
using Web_CDTT_NguyenMinhDat.Repository.SendMail;
namespace Web_CDTT_NguyenMinhDat.Repository
{
    public class EmailService : IEmailSender
    {
        public async Task SendEmailAsync(string to, string subject, string body, string fromName, string fromEmail)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("datminh100504@gmail.com", "zdzvaeldwmeogajm") 
            };

            // Set the sender's name and email
            var fromAddress = new MailAddress(fromEmail, fromName);
            var toAddress = new MailAddress(to);

            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true 
            };

            // Send the email
            await client.SendMailAsync(mailMessage);
        }

    }
}



//namespace WebCDTT_NguyenMinhDat.Repository
//{
//    public class EmailService : IEmailSender
//    {
//        public Task SendEmailAysnc(string email, string subject, string message)
//        {
//            var client = new SmtpClient("smtp.gmail.com", 587);
//            {
//                client.EnableSsl = true;
//                client.UseDefaultCredentials = false;
//                client.Credentials = new NetworkCredential("datminh100504@gmail.com", "zdzvaeldwmeogajm");

//            };
//            return client.SendMailAsync(
//                new MailMessage(from: "datminh100504@gmail.com",
//                to: email,
//                subject,
//                message

//                ));
//        }
//    }
//}

