namespace EmailService.DTOs
{
    public class SendEmailNotificationRequest
    {
        public long NotificationId { get; set; }
        public long RecipientId { get; set; }
        public string? RecipientEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? Message { get; set; }
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
