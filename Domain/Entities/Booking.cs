using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public User? User { get; private set; }
        public Guid RoomId { get; init; }
        public Room? Room { get; private set; }
        public DateTimeOffset CheckInDate { get; private set; }
        public DateTimeOffset CheckOutDate { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal FinalPrice => TotalPrice - DiscountAmount;
        public decimal DiscountAmount { get; private set; }
        public BookingStatus BookingStatus { get; private set; } = BookingStatus.Created;
        public bool IsDeleted { get; private set; } = false;

        public Booking(
            Guid id, 
            Guid userId,
            Guid roomId,
            DateTimeOffset checkInDate,
            DateTimeOffset checkOutDate,
            decimal totalPrice,
            decimal discountAmount

            )
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");
            
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id cannot be empty");

            if (roomId == Guid.Empty)
                throw new ArgumentException("Room Id cannot be empty");

            if (checkInDate > checkOutDate)
                throw new ArgumentException("Date cannot be more than check out date");


            Id = id;
            UserId = userId;
            RoomId = roomId;
            CheckOutDate = checkOutDate;
            CheckInDate = checkInDate;
            TotalPrice = totalPrice;
            DiscountAmount = discountAmount;  
        }

        public void ExtendStay(DateTimeOffset newCheckOutDate, decimal pricePerNight)
        {
            if (newCheckOutDate <= CheckOutDate)
                throw new ArgumentException("");

            if (pricePerNight <= 0)
                throw new ArgumentException("Price must be positive");

            CheckOutDate = newCheckOutDate;
            TotalPrice = CalculateTotalPrice(pricePerNight, CheckInDate, CheckOutDate);

           
        }

        public void ShortenStay(DateTimeOffset newCheckOutDate, decimal pricePerNight, out int refundOfPoints)
        {
            refundOfPoints = 0;
            if (newCheckOutDate <= CheckInDate)
                throw new ArgumentException("");

            if (pricePerNight <= 0)
                throw new ArgumentException("Price must be positive");
            
            CheckOutDate = newCheckOutDate;
            TotalPrice = CalculateTotalPrice(pricePerNight, CheckInDate, CheckOutDate);
            refundOfPoints = (int)DiscountAmount;
            DiscountAmount = 0;
        }

        public static decimal CalculateTotalPrice(
            decimal pricePerNight, 
            DateTimeOffset checkInDate,
            DateTimeOffset checkOutDate)
        {
            if (pricePerNight <= 0)
                throw new ArgumentException("Price must be positive");

            int nights = (checkOutDate - checkInDate).Days;
            if (nights == 0)
                nights = 1;
            return Math.Round(nights * pricePerNight, 2);

        }

        public void DeleteBooking()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Booking has already been deleted");
            IsDeleted = true;
        }

        public void CompleteBooking()
        {
            if (BookingStatus != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be completed.");

            BookingStatus = BookingStatus.Completed;
        }
        public void CancelBooking()
        {
            if (BookingStatus == BookingStatus.Completed)
                throw new InvalidOperationException("Cannot cancel an already completed booking");

            BookingStatus = BookingStatus.Cancelled;
        }

        public void ConfirmBooking()
        {
            if (BookingStatus != BookingStatus.Created)
                throw new InvalidOperationException("Can only confirm created bookings");

            BookingStatus = BookingStatus.Confirmed;
        }
    }
}
