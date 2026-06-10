using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Hotel
    {
        public Guid Id { get; init; }
        public String Name { get; private set; } = String.Empty;
        public string City { get; init; }
        public string Country { get; init; }
        public string Address { get; init; }
        public double Rating { get; private set; } = 0.0;
        public Guid ManagerId { get; private set; }
        public User? Manager { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        /* private readonly List<Room> rooms = new();
         public IReadOnlyCollection<Room> Rooms => rooms.AsReadOnly();*/


        public Hotel(
            Guid id,
            string name,
            string city,
            string country,
            string address,
            double rating,
            Guid managerId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");

            if (String.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty");

            if (String.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty");

            ChangeName(name);
            ChangeManager(managerId);
            ChangeRating(rating);  
            Id = id;
            Country = country;
            City = city;
            Address = address;
        }

        public void DeleteHotel()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Hotel has already been deleted");
            IsDeleted = true;
        }

        public void ChangeManager(Guid managerId)
        {
            if (managerId == Guid.Empty)
                throw new ArgumentException("Manager Id cannot be empty");
            ManagerId = managerId;
        }

        public void ChangeName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hotel name cannot be empty");

            if (name.Length < 6)
                throw new ArgumentException("Hotel name cannot be shorter then 6 chatacters");

            Name = name;
        }

        public void ChangeRating(double rating)
        {
            if (rating < 0.0 || rating > 5.0)
                throw new ArgumentException("Rating must be between 0 and 5");

            Rating = Math.Round(rating, 2);
        }
    }
}
