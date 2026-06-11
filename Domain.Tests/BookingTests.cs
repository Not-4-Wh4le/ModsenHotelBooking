using Domain.Entities;
using Domain.Enums;

namespace Domain.Tests
{
    public class BookingTests
    {
        private Booking CreateValidTestBooking(DateTimeOffset checkIn, DateTimeOffset checkOut, decimal pricePerNight)
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var totalPrice = Booking.CalculateTotalPrice(pricePerNight, checkIn, checkOut);

            return new Booking(id, userId, roomId, checkIn, checkOut, totalPrice, discountAmount: 10m);
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly_WhenDataIsValid()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var roomId = Guid.NewGuid();
            var checkIn = DateTimeOffset.Now;
            var checkOut = checkIn.AddDays(3);
            var totalPrice = 300m;
            var discount = 30m;
            var booking = new Booking(id, userId, roomId, checkIn, checkOut, totalPrice, discount);

            Assert.Equal(id, booking.Id);
            Assert.Equal(userId, booking.UserId);
            Assert.Equal(roomId, booking.RoomId);
            Assert.Equal(BookingStatus.Created, booking.BookingStatus);
            Assert.False(booking.IsDeleted);
            Assert.Equal(270m, booking.FinalPrice);
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentException_WhenCheckInIsAfterCheckOut()
        {
            var checkIn = DateTimeOffset.Now.AddDays(5);
            var checkOut = DateTimeOffset.Now.AddDays(2);

            var exception = Assert.Throws<ArgumentException>(() =>
                new Booking(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), checkIn, checkOut, 100m, 0m)
            );
            Assert.Contains("Date cannot be more than check out date", exception.Message);
        }

        [Fact]
        public void CompleteBooking_ShouldThrowException_WhenBookingIsNotConfirmed()
        {
            var booking = CreateValidTestBooking(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(2), 100m);
            Assert.Throws<InvalidOperationException>(() =>
                booking.CompleteBooking()
            );
        }

        [Fact]
        public void ShortenStay_ShouldRecalculatePriceAndResetDiscount_WhenCalled()
        {
            var checkIn = DateTimeOffset.Now;
            var checkOut = checkIn.AddDays(4); 
            var pricePerNight = 50m;           
            var booking = CreateValidTestBooking(checkIn, checkOut, pricePerNight);
            var newCheckOut = DateTimeOffset.Now.AddDays(2);
            booking.ShortenStay(newCheckOut, pricePerNight, out int refundOfPoints);
            Assert.Equal(newCheckOut, booking.CheckOutDate);
            Assert.Equal(100m, booking.FinalPrice);
            Assert.Equal(0m, booking.DiscountAmount);
            Assert.Equal(10, refundOfPoints);
        }
    }
}
