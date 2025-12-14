using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifySystem.Core.Domain.SharedKernel.Base;

public abstract class Entity : IAggregateRoot
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    protected Entity(long id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }

    protected void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}