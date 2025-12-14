using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.SharedKernel.Base;
using NotifySystem.Core.Domain.ValueObjects;

namespace NotifySystem.Core.Domain.Entities
{
    public class Recipient : Entity
    {
        public string Name { get; set; }
        public ContactInfo ContactInfo { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        
        private Recipient() { }

        public Recipient(string name, ContactInfo contactInfo)
        {
            Name = name;
            ContactInfo = contactInfo;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public string GetContact(NotificationType type)
        {
            return ContactInfo.GetContact(type);
        }

        public void UpdateContactInfo(ContactInfo contact)
        {
            ContactInfo = contact;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
