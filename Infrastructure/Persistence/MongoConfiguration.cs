using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
    public static class MongoConfiguration
    {
        private static bool isConfigured;
        public static void Configure()
        {
            if (isConfigured) return;

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),     
            new EnumRepresentationConvention(BsonType.String), 
            new IgnoreExtraElementsConvention(true) 
        };

            ConventionRegistry.Register(
                "ApplicationConventions",
                conventionPack,
                t => true); 

            isConfigured = true;
        }
    }
}
