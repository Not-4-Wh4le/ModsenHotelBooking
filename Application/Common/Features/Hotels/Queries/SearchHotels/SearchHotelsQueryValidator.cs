using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Hotels.Queries.SearchHotels
{
    public class SearchHotelsQueryValidator : AbstractValidator<SearchHotelsQuery>
    {
        private static readonly string[] AllowedSortFields = ["name", "rating"];
        public SearchHotelsQueryValidator()
        {
            RuleFor(q => q.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(50).WithMessage("Page size cannot exceed 50 items per page");

            RuleFor(q => q.MinRating)
                .InclusiveBetween(0.0, 5.0)
                .When(q => q.MinRating.HasValue)    
                .WithMessage("Minimal rating must be between 0.0 and 5.0");

            RuleFor(q => q.SortBy)
                .NotEmpty().WithMessage("Sort field is required")
                .Must(s => AllowedSortFields.Contains(s.ToLower()))
                .WithMessage($"You can only sort by {string.Join(", ", AllowedSortFields)}"); 
        }
    }
}
