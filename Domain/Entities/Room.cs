using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Room
    {
        public Guid Id { get; init; }
        public int Number { get; private set; }
        public Guid HotelId { get; init; }
        public Hotel? Hotel { get; private set; }
        public RoomType RoomType { get; private set; }
        public decimal PricePerNight { get; private set;}
        public int Capacity { get; private set; }
        public string Description { get; private set; } = String.Empty;
        public string Amenities { get; private set; } = String.Empty;
        public int Area { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        public Room(
            Guid id, 
            int number,
            Guid hotelId, 
            RoomType roomType, 
            decimal pricePerNight, 
            int capacity,
            string description,
            string amenities,
            int area
            )
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");
            
            if (hotelId == Guid.Empty)
                throw new ArgumentException("Hotel Id cannot be empty");

            if (area <= 0)
                throw new ArgumentException("Area must be positive");

            Id = id;
            HotelId = hotelId;
            Area = area;
            ChangeRoomType( roomType );
            ChangeNumber(number);
            ChangeCapacity(capacity);
            ChangePricePerNight(pricePerNight);
            ChangeAmenities(amenities);
            ChangeDescription(description);
            
        }

        public void ChangePricePerNight(decimal pricePerNight)
        {
            if (pricePerNight <= 0)
                throw new ArgumentException("Price must be positive");
            PricePerNight = pricePerNight;
        }

        public void ChangeCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be positive");

            Capacity = capacity;
        }

        public void ChangeNumber(int number)
        {
            if (number <= 0)
                throw new ArgumentException("Numder must be positive");

            Number = number;
        }

        public void ChangeDescription(string description)
        {
            var trimDescription = description?.Trim() ?? string.Empty;

            if (trimDescription.Length > 500)
                throw new ArgumentException("Description too long");
            
            Description = trimDescription;
        }

        public void ChangeAmenities(string amenities)
        {
            var trimAmenities = amenities?.Trim() ?? string.Empty;
            if (trimAmenities.Length > 250)
                throw new ArgumentException("Amenities too long");
            Amenities = trimAmenities;
        }

        public void DeleteRoom()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Room has already been deleted");
            IsDeleted = true;
        }

        public void ChangeRoomType(RoomType roomType) => RoomType = roomType;
    }
}
