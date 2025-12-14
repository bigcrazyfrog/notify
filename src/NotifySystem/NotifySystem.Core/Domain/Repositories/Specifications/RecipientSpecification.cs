using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.SharedKernel.Specification;

namespace NotifySystem.Core.Domain.Repositories.Specifications
{
    public static class RecipientSpecification
    {
        public static ISpecification<Recipient> ByName(string name)
        {
            return Specification<Recipient>.Create(x => x.Name.Contains(name));
        }

        public static ISpecification<Recipient> ByEmail(string email)
        {
            return Specification<Recipient>.Create(x => x.ContactInfo.Email.Contains(email));
        }

        public static ISpecification<Recipient> ByPhone(string phone)
        {
            return Specification<Recipient>.Create(x => x.ContactInfo.PhoneNumber.Contains(phone));
        }

        public static ISpecification<Recipient> ByDeviceToken(string deviceToken)
        {
            return Specification<Recipient>.Create(x => x.ContactInfo.DeviceToken.Contains(deviceToken));
        }

        internal static ISpecification<Recipient> ByNameAndEmail(string name, string email)
        {
            return ByEmail(email).And(Specification<Recipient>.Create(x => EF.Functions.Like(nameof(x.Name), $"{name}")));
        }
        
        internal static ISpecification<Recipient> ByNameAndPhone(string name, string phone)
        {
            return ByPhone(phone).And(Specification<Recipient>.Create(x => EF.Functions.Like(nameof(x.Name), $"{name}")));
        }
        
        internal static ISpecification<Recipient> ByNameAndDeviceToken(string name, string deviceToken)
        {
            return ByDeviceToken(deviceToken).And(Specification<Recipient>.Create(x => EF.Functions.Like(nameof(x.Name), $"{name}")));
        }

        public static ISpecification<Recipient> IsActive()
        {
            return Specification<Recipient>.Create(x => x.IsActive);
        }

        public static ISpecification<Recipient> IsInactive()
        {
            return Specification<Recipient>.Create(x => !x.IsActive);
        }

        public static ISpecification<Recipient> CreatedAfter(DateTime date)
        {
            return Specification<Recipient>.Create(x => x.CreatedAt > date);
        }

        public static ISpecification<Recipient> CreatedBefore(DateTime date)
        {
            return Specification<Recipient>.Create(x => x.CreatedAt < date);
        }

        public static ISpecification<Recipient> HasEmail()
        {
            return Specification<Recipient>.Create(x => !string.IsNullOrEmpty(x.ContactInfo.Email));
        }

        public static ISpecification<Recipient> HasPhone()
        {
            return Specification<Recipient>.Create(x => !string.IsNullOrEmpty(x.ContactInfo.PhoneNumber));
        }

        public static ISpecification<Recipient> HasDeviceToken()
        {
            return Specification<Recipient>.Create(x => !string.IsNullOrEmpty(x.ContactInfo.DeviceToken));
        }
    }
}