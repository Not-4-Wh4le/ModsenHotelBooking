using Domain.Enums;
using Domain.Strategies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Domain.Entities
{
    public class LoyaltyProgram
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public int TotalPoints { get; private set; } = 0;
        public int CurrentPoints { get; private set; } = 0;
        public LoyaltyTier Tier { get; private set; } = LoyaltyTier.Bronze;
        public decimal TotalSpent { get; private set; } = 0m;
        public DateTimeOffset JoinedAt { get; } = DateTimeOffset.Now;

        public LoyaltyProgram(Guid id, Guid userId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");

            if (userId == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");

            Id = id;
            UserId = userId;
        }
        public decimal GetTierDiscountPercent() => Tier switch
        {
            LoyaltyTier.Bronze => 0.00m,
            LoyaltyTier.Silver => 0.05m,
            LoyaltyTier.Gold => 0.10m,
            LoyaltyTier.Platinum => 0.20m,
            _ => 0.00m
        };

        public decimal CalculateDiscount(
            decimal bookingTotalPrice, 
            bool usePoints,
            IDiscountStrategy discountStrategy,
            out int pointsToDeduct)  
            => discountStrategy.CalculateDiscount(bookingTotalPrice, CurrentPoints, out pointsToDeduct);
        

        public void SpendPoints(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative");

            if (points > CurrentPoints)
                throw new InvalidOperationException("Not enough points available");

            CurrentPoints -= points;
        }

        public void RewardPointsForBooking(Booking booking)
        {
            if (booking.BookingStatus != BookingStatus.Completed)
                throw new InvalidOperationException("Booking must be completed");

            TotalSpent += booking.TotalPrice;
            var earnedPoint = (int)(booking.TotalPrice * 0.1m);
            TotalPoints += earnedPoint;
            CurrentPoints += earnedPoint;

            UpdateTier();
        }

        private void UpdateTier()
        {
            Tier = TotalPoints switch
            {
                >= 10000 => LoyaltyTier.Platinum,
                >= 5000 => LoyaltyTier.Gold,
                >= 1000 => LoyaltyTier.Silver,
                _ => LoyaltyTier.Bronze

            };
        }
    }
}
