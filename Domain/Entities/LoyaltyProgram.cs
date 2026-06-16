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
        public const int SilverPointsThreshold = 1000;
        public const int GoldPointsThreshold = 5000;
        public const int PlatinumPointsThreshold = 10000;

        public const decimal BronzeDiscountPercent = 0.00m;
        public const decimal SilverDiscountPercent = 0.05m;
        public const decimal GoldDiscountPercent = 0.10m;
        public const decimal PlatinumDiscountPercent = 0.20m;

        public const decimal BookingFinalPriceToPointsPercent = 0.10m;
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
            LoyaltyTier.Bronze => BronzeDiscountPercent,
            LoyaltyTier.Silver => SilverDiscountPercent,
            LoyaltyTier.Gold => GoldDiscountPercent,
            LoyaltyTier.Platinum => PlatinumDiscountPercent,
            _ => BronzeDiscountPercent
        };
        
        public void SpendPoints(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative");

            if (points > CurrentPoints)
                throw new InvalidOperationException("Not enough points available");

            CurrentPoints -= points;
        }

        public void RewardPointsForBooking(decimal bookingFinalPrice)
        {
            TotalSpent += bookingFinalPrice;
            var earnedPoint = (int)(bookingFinalPrice * BookingFinalPriceToPointsPercent);
            TotalPoints += earnedPoint;
            CurrentPoints += earnedPoint;

            UpdateTier();
        }

        private void UpdateTier()
        {
            Tier = TotalPoints switch
            {
                >= PlatinumPointsThreshold => LoyaltyTier.Platinum,
                >= GoldPointsThreshold => LoyaltyTier.Gold,
                >= SilverPointsThreshold=> LoyaltyTier.Silver,
                _ => LoyaltyTier.Bronze
            };
        }
    }
}
