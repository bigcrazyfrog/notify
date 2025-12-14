namespace EmailService.DTOs
{
    public class EmailResult
    {
        public bool IsSuccess { get; private set; }
        public string? MessageId { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? Details { get; private set; }

        private EmailResult(bool isSuccess, string? messageId, string? errorMessage, string? details)
        {
            IsSuccess = isSuccess;
            MessageId = messageId;
            ErrorMessage = errorMessage;
            Details = details;
        }

        public static EmailResult Success(string messageId, string details)
        {
            return new EmailResult(true, messageId, null, details);
        }

        public static EmailResult Failure(string errorMessage)
        {
            return new EmailResult(false, null, errorMessage, null);
        }
    }
}