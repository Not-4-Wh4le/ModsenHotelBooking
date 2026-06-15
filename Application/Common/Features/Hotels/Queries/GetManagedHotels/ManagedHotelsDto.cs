using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.GetManagedHotels
{
    public record ManagedHotelsDto(
        Guid Id,
        string Name,
        string City,
        string Country,
        string Address,
        double Rating,
        Guid ManagerId,
        string ManagerUsername);
}
