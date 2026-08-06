using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Rooms.Queries.SearchRooms
{
    public class SearchRoomsQueryValidator : AbstractValidator<SearchRoomsQuery>
    {
        private static string[] AllowedSortFields = ["price", "capacity"];
        public SearchRoomsQueryValidator()
        {
            
            
            RuleFor(q => q.Page)
               .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(50).WithMessage("Page size cannot exceed 50 items per page");
           
            RuleFor(q => q.SortBy)
                .NotEmpty().WithMessage("Sort field is required")
                .Must(role => AllowedSortFields.Contains(role.ToLower()))
                .WithMessage($"You can only sort by {string.Join(", ", AllowedSortFields)}");

            RuleFor(q => q.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Min price cannot be negative");

            RuleFor(q => q.MaxPrice)
                .GreaterThanOrEqualTo(q => q.MinPrice).WithMessage("Max price cannot be less than min")
                .When(q => q.MaxPrice.HasValue);

            RuleFor(q => q.Capacity)
                .GreaterThan(0)
                .WithMessage("Capacity must be greater than 0")
                .When(q => q.Capacity.HasValue);

            RuleFor(q => q.CheckIn)
                .GreaterThanOrEqualTo(DateTimeOffset.UtcNow.Date)
                .WithMessage("Check-in date cannot be in the past")
                .When(q => q.CheckIn.HasValue);

            RuleFor(q => q.CheckOut)
                .GreaterThan(q => q.CheckIn)
                .WithMessage("Check-out date msut be after Check-in date")
                .When(q => q.CheckIn.HasValue && q.CheckOut.HasValue);

            RuleFor(q => q.CheckIn)
                .NotEmpty()
                .WithMessage("Check-in date is required if check-out date is specified")
                .When(q => q.CheckOut.HasValue && !q.CheckIn.HasValue);

        }
    }
}
