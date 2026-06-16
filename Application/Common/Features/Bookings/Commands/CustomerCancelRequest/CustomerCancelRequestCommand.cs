using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CustomerCancelRequest
{
    public record CustomerCancelRequestCommand(
        Guid BookingId)
        : IRequest<Result<Guid>>;
}
