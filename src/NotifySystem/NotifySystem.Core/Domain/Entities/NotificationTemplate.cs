using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.Entities
{
    public class NotificationTemplate : Entity
    {
        public string Name { get; private set; }         
        public string MessageTemplate { get; private set; } 
        public NotificationType Type { get; private set; }  
        public bool IsActive { get; private set; }

        public NotificationTemplate(string name, string messageTemplate, NotificationType type)
        {
            Name = name;
            MessageTemplate = messageTemplate;
            Type = type;
            IsActive = true;
        }

        public void UpdateTemplate(string content)
        {
            MessageTemplate = content;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
