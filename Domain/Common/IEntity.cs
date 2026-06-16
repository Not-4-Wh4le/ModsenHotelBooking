using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public interface IEntity
    {
        Guid Id { get; init; }
    }
}
