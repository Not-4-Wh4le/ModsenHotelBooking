using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetHotelsBooking
{
    public class GetHotelsBookingQueryValidator : AbstractValidator<GetHotelsBookingQuery>
    {
        public GetHotelsBookingQueryValidator()
        {
            RuleFor(q => q.HotelId)
                .NotEmpty().WithMessage("Hotel Id is required");

            RuleFor(q => q.Page)
               .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(50).WithMessage("Page size cannot exceed 50 items per page");
        }
    }
}
