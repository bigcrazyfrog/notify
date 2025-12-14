namespace EmailService.Configuration
{
    public class EmailSettings
    {
        public bool Enabled { get; set; } = true;
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public bool UseSsl { get; set; }

        public void Validate()
        {
            if (!Enabled)
                return;
                
            if (string.IsNullOrWhiteSpace(SmtpServer))
                throw new InvalidOperationException("SMTP Server is required");
            
            if (SmtpPort <= 0)
                throw new InvalidOperationException("SMTP Port must be greater than 0");
            
            if (string.IsNullOrWhiteSpace(SenderEmail))
                throw new InvalidOperationException("Sender Email is required");
            
            if (string.IsNullOrWhiteSpace(SenderPassword))
                throw new InvalidOperationException("Sender Password is required");
        }
    }
}
