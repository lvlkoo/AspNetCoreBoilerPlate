using System;

namespace Boilerplate.DAL.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}