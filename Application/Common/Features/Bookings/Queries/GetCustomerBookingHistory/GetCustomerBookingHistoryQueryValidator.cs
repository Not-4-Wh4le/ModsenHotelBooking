using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Queries.GetCustomerBookingHistory
{
    public class GetCustomerBookingHistoryQueryValidator : AbstractValidator<GetCustomerBookingHistoryQuery>
    {
        public GetCustomerBookingHistoryQueryValidator()
        {
            RuleFor(q => q.CustomerId)
                .NotEmpty().WithMessage("Customer Id is required");

            RuleFor(q => q.Page)
               .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(50).WithMessage("Page size cannot exceed 50 items per page");

        }
    }
}
