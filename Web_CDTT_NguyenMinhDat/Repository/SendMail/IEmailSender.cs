namespace Web_CDTT_NguyenMinhDat.Repository.SendMail
{
    public interface IEmailSender
    {
        public  Task SendEmailAsync(string to, string subject, string body, string fromName, string fromEmail);
    }
}