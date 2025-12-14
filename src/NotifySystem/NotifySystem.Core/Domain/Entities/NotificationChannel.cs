using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.Entities
{
    public class NotificationChannel : Entity
    {
        public string Name { get; private set; }
        public bool IsEnabled { get; private set; }
        public TimeSpan RetryInterval { get; private set; }
        public int MaxRetryCount { get; private set; }

        public NotificationChannel(string name, TimeSpan retryInterval, int maxRetryCount)
        {
            if (maxRetryCount < 0)
                throw new Exception("MaxRetryCount must be >= 0");

            Name = name;
            RetryInterval = retryInterval;
            MaxRetryCount = maxRetryCount;
            IsEnabled = true;
        }

        public bool CanRetry(int retryCount)
            => retryCount < MaxRetryCount;

        public void UpdateSettings(TimeSpan retryInterval, int maxRetryCount)
        {
            RetryInterval = retryInterval;
            MaxRetryCount = maxRetryCount;
        }

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
    }
}
