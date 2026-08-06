using Application.Common.Interfaces.Repositories;
using Application.DiscountFactory;
using Application.DiscountFactory.Interfaces;
using Domain.Entities;
using Domain.Strategies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler(
        IRoomRepository roomRepository,
        IDiscountStrategyResolver strategyResolver,
        ILoyaltyRepository loyaltyRepository,
        IBookingRepository bookingRepository)
        : IRequestHandler<CreateBookingCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if(room == null)
                return Result<Guid>.Failure("Room is not available");

            decimal totalPrice = Booking.CalculateTotalPrice(room.PricePerNight, request.CheckInDate, request.CheckOutDate);

            var selectedDiscountType = DiscountType.NoDiscount;

            if (!string.IsNullOrEmpty(request.PromoCode))
                selectedDiscountType = DiscountType.PromoCode;
            else if (request.UseLoyaltyPoints)
                selectedDiscountType = DiscountType.LoyaltyProgram;

            var env = new BookingDiscountEnvironment(
                request.UserId, 
                request.RoomId, 
                request.CheckInDate, 
                request.PromoCode);

            IDiscountStrategy dicsountStrategy = await strategyResolver.ResolveAsync(env, selectedDiscountType);
            var loyaltyProfile = await loyaltyRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            int userCurrentPoints = loyaltyProfile?.CurrentPoints ?? 0;

            var discountContext = new DiscountContext(totalPrice, userCurrentPoints, request.PromoCode);
            var discountResult = dicsountStrategy.CalculateDiscount(discountContext);

            if (selectedDiscountType == DiscountType.LoyaltyProgram && discountResult.PointsToDeduct > 0 && loyaltyProfile != null)
            {
                loyaltyProfile.SpendPoints(discountResult.PointsToDeduct);
                await loyaltyRepository.CreateOrUpdateAsync(loyaltyProfile, cancellationToken);
            }

            Booking booking;
            try
            {
                booking = new Booking(
                    Guid.NewGuid(),
                    request.UserId,
                    request.RoomId,
                    request.CheckInDate,
                    request.CheckOutDate,
                    totalPrice,
                    discountResult.DiscountAmount,
                    discountResult.PointsToDeduct);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await bookingRepository.AddAsync(booking, cancellationToken);
            return Result<Guid>.Success(booking.Id);
        }
    }
}
