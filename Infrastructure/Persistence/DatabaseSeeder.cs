using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
    public class DatabaseSeeder(MongoDbContext dbContext, IPasswordHasher passwordHasher)
    {
        public async Task SeedAsync()
        {
            await SeedUsersAsync();
            await SeedHotelsAsync();
            await SeedRoomsAsync();
            await SeedBookingsAsync();
            await SeedLoyaltyProgramsAsync();
        }

        private async Task SeedUsersAsync()
        {
            var userCollection = dbContext.GetCollection<User>(MongoCollectionNames.Users);

            var totalUsers = await userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);

            if (totalUsers > 0)
            {
                return;
            }

            string defaultPasswordHash = passwordHasher.HashPassword("19051906123");

            var admin = new User(Guid.NewGuid(), "admin", "admin@modsen.com", defaultPasswordHash, UserRole.Admin);
            var manager = new User(Guid.NewGuid(), "hotel_manager", "manager@modsen.com", defaultPasswordHash, UserRole.HotelManager);
            var manager2 = new User(Guid.NewGuid(), "hotel_manager2", "manager2@modsen.com", defaultPasswordHash, UserRole.HotelManager);

            var alex = new User(Guid.NewGuid(), "alex_customer", "alex@gmail.com", defaultPasswordHash, UserRole.Customer);
            var dmitry = new User(Guid.NewGuid(), "dmitry_customer", "dma@gmail.com", defaultPasswordHash, UserRole.Customer);

            var anna = new User(Guid.NewGuid(), "anna_customer", "anna@gmail.com", defaultPasswordHash, UserRole.Customer);
            anna.DeleteUser();

            var seedUsers = new List<User>
            {
                admin,
                manager,
                manager2,
                alex,
                dmitry,
                anna
            };

            await userCollection.InsertManyAsync(seedUsers);
        }

        private async Task SeedHotelsAsync()
        {
            var hotelCollection = dbContext.GetCollection<Hotel>(MongoCollectionNames.Hotels);

            var totalHotels = await hotelCollection.CountDocumentsAsync(FilterDefinition<Hotel>.Empty);
            if (totalHotels > 0)
            {
                return;
            }

            var userCollection = dbContext.GetCollection<User>(MongoCollectionNames.Users);
            var manager = await userCollection.Find(u => u.Username == "hotel_manager").FirstOrDefaultAsync();
            var manager2 = await userCollection.Find(u => u.Username == "hotel_manager2").FirstOrDefaultAsync();

            Guid managerId = manager?.Id ?? Guid.NewGuid();
            Guid managerId2 = manager2?.Id ?? Guid.NewGuid();

            var hotel1 = new Hotel(Guid.NewGuid(), "Grand Plaza Hotel", "Minsk", "Belarus", "Lenina St. 12", managerId2, 4.5);
            var hotel2 = new Hotel(Guid.NewGuid(), "Renaissance Minsk", "Minsk", "Belarus", "Dzerzhinskogo Ave. 15", managerId, 4.8);
            var hotel3 = new Hotel(Guid.NewGuid(), "Eco Stay Resort", "Brest", "Belarus", "Kosmonavtov St. 24", managerId2, 3.9);

            var deletedHotel = new Hotel(Guid.NewGuid(), "Old Broken Inn", "Gomel", "Belarus", "Pervomayskaya St. 8", managerId, 1.2);
            deletedHotel.DeleteHotel();

            var seedHotels = new List<Hotel> { hotel1, hotel2, hotel3, deletedHotel };
            await hotelCollection.InsertManyAsync(seedHotels);
        }

        private async Task SeedRoomsAsync()
        {
            var roomCollection = dbContext.GetCollection<Room>(MongoCollectionNames.Rooms);
            var totalRooms = await roomCollection.CountDocumentsAsync(FilterDefinition<Room>.Empty);

            if (totalRooms > 0) return;

            var hotelCollection = dbContext.GetCollection<Hotel>(MongoCollectionNames.Hotels);
            var grandPlaza = await hotelCollection.Find(h => h.Name == "Grand Plaza Hotel").FirstOrDefaultAsync();
            var renaissance = await hotelCollection.Find(h => h.Name == "Renaissance Minsk").FirstOrDefaultAsync();

            if (grandPlaza == null || renaissance == null) return;

            var seedRooms = new List<Room>
            {
                new Room(Guid.NewGuid(), 101, grandPlaza.Id, RoomType.Double, 120.00m, 2, "Уютный номер.", "Wi-Fi, TV", 25),
                new Room(Guid.NewGuid(), 102, grandPlaza.Id, RoomType.Double, 120.00m, 2, "Уютный номер.", "Wi-Fi, TV", 25),
                new Room(Guid.NewGuid(), 201, grandPlaza.Id, RoomType.Suite, 250.00m, 4, "Люкс с видом.", "Wi-Fi, Джакузи", 55),
                new Room(Guid.NewGuid(), 404, renaissance.Id, RoomType.Deluxe, 180.00m, 3, "Улучшенный номер.", "Wi-Fi, Кофемашина", 38),

                ConvertToDeleteRoom(new Room(Guid.NewGuid(), 666, renaissance.Id, RoomType.Single, 50.00m, 1, "На ремонте.", "Нет", 12))
            };

            await roomCollection.InsertManyAsync(seedRooms);
        }
        private static Room ConvertToDeleteRoom(Room room)
        {
            room.DeleteRoom();
            return room;
        }

        private async Task SeedBookingsAsync()
        {
            var bookingCollection = dbContext.GetCollection<Booking>(MongoCollectionNames.Bookings);

            var totalBookings = await bookingCollection.CountDocumentsAsync(FilterDefinition<Booking>.Empty);
            if (totalBookings > 0) return;

            var userCollection = dbContext.GetCollection<User>(MongoCollectionNames.Users);
            var alex = await userCollection.Find(u => u.Username == "alex_customer").FirstOrDefaultAsync();
            var dmitry = await userCollection.Find(u => u.Username == "dmitry_customer").FirstOrDefaultAsync();

            var roomCollection = dbContext.GetCollection<Room>(MongoCollectionNames.Rooms);
            var room101 = await roomCollection.Find(r => r.Number == 101).FirstOrDefaultAsync();
            var room201 = await roomCollection.Find(r => r.Number == 201).FirstOrDefaultAsync();

            if (alex == null || dmitry == null || room101 == null || room201 == null) return;

            var now = DateTimeOffset.Now;

            var checkIn1 = now.AddDays(3);
            var checkOut1 = now.AddDays(7);
            var price1 = Booking.CalculateTotalPrice(room101.PricePerNight, checkIn1, checkOut1);

            var bookingConfirmed = new Booking(Guid.NewGuid(), alex.Id, room101.Id, checkIn1, checkOut1, price1, 0.0m, 0);
            bookingConfirmed.ConfirmBooking();

            var checkIn2 = now.AddDays(-10);
            var checkOut2 = now.AddDays(-5);
            var price2 = Booking.CalculateTotalPrice(room201.PricePerNight, checkIn2, checkOut2);

            var bookingCompleted = new Booking(Guid.NewGuid(), dmitry.Id, room201.Id, checkIn2, checkOut2, price2, 20.0m, 200);
            bookingCompleted.ConfirmBooking();
            bookingCompleted.CompleteBooking();


            var checkIn3 = now.AddDays(15);
            var checkOut3 = now.AddDays(20);
            var price3 = Booking.CalculateTotalPrice(room101.PricePerNight, checkIn3, checkOut3);

            var bookingWithCancelRequest = new Booking(Guid.NewGuid(), alex.Id, room101.Id, checkIn3, checkOut3, price3, 0.0m, 0);
            bookingWithCancelRequest.RequestCancelation();

            var checkIn4 = now.AddDays(30);
            var checkOut4 = now.AddDays(35);
            var price4 = Booking.CalculateTotalPrice(room101.PricePerNight, checkIn4, checkOut4);

            var bookingDeleted = new Booking(Guid.NewGuid(), dmitry.Id, room101.Id, checkIn4, checkOut4, price4, 0.0m, 0);
            bookingDeleted.DeleteBooking();

            var seedBookings = new List<Booking>
            {
                bookingConfirmed,
                bookingCompleted,
                bookingWithCancelRequest,
                bookingDeleted
            };

            await bookingCollection.InsertManyAsync(seedBookings);
        }

        private async Task SeedLoyaltyProgramsAsync()
        {
            var loyaltyCollection = dbContext.GetCollection<LoyaltyProgram>(MongoCollectionNames.LoyaltyPrograms);

            var totalPrograms = await loyaltyCollection.CountDocumentsAsync(FilterDefinition<LoyaltyProgram>.Empty);
            if (totalPrograms > 0) return;

            var userCollection = dbContext.GetCollection<User>(MongoCollectionNames.Users);
            var alex = await userCollection.Find(u => u.Username == "alex_customer").FirstOrDefaultAsync();
            var dmitry = await userCollection.Find(u => u.Username == "dmitry_customer").FirstOrDefaultAsync();

            if (alex == null || dmitry == null) return;

            var alexLoyalty = new LoyaltyProgram(Guid.NewGuid(), alex.Id);
            var dmitryLoyalty = new LoyaltyProgram(Guid.NewGuid(), dmitry.Id);


            alexLoyalty.RewardPointsForBooking(12500.00m);

            dmitryLoyalty.RewardPointsForBooking(55000.00m);
            dmitryLoyalty.SpendPoints(500);

            var seedLoyalty = new List<LoyaltyProgram> { alexLoyalty, dmitryLoyalty };
            await loyaltyCollection.InsertManyAsync(seedLoyalty);
        }
    }
}
