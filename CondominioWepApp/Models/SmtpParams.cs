namespace CondominioWepApp.Models
{
    public class SmtpParams
    {
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? SenderEmail { get; set; }
        public string? ReceiverEmail { get; set; }
        public string? ClientName { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Password { get; set; }
    }
}
