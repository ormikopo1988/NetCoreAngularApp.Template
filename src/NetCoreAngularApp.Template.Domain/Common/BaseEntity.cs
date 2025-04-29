using System;

namespace NetCoreAngularApp.Template.Domain.Common;

public abstract class BaseEntity
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic Guid types for simplicity
    public Guid Id { get; set; } = Guid.NewGuid();
}
