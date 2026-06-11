using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Tests
{
    public class LoyaltyProgramTests
    {
        [Theory]
        [InlineData(1000, 100, LoyaltyTier.Bronze)]
        [InlineData(10000, 1000, LoyaltyTier.Silver)]
        [InlineData(50000, 5000, LoyaltyTier.Gold)]
        [InlineData(100000, 10000, LoyaltyTier.Platinum)]
        public void RewardPointsForBooking_ShouldIncreasePointsAndUpgradeTierCorrectly(
            decimal totalPrice, int expectedPoints, LoyaltyTier expectedTier)
        {
            var loyalty = new LoyaltyProgram(id: Guid.NewGuid(), userId: Guid.NewGuid());
            var booking = new Booking(
                Guid.NewGuid(),
                loyalty.UserId,
                Guid.NewGuid(),
                DateTimeOffset.Now,
                DateTimeOffset.Now.AddDays(1),
                totalPrice,
                0m);

            booking.ConfirmBooking();
            booking.CompleteBooking();
            loyalty.RewardPointsForBooking(booking);

            Assert.Equal(expectedTier, loyalty.Tier);
            Assert.Equal(expectedPoints, loyalty.TotalPoints);
            Assert.Equal(expectedPoints, loyalty.CurrentPoints);
            Assert.Equal(totalPrice, loyalty.TotalSpent);
        }
    }

}
