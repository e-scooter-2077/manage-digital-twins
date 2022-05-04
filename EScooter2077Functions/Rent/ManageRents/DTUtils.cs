using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;

namespace EScooter.Rent.ManageRents
{
    public static class DTUtils
    {
        private const string RelationshipName = "is_riding";

        public static async Task CreateRentRelationship(Guid rentId, Guid customerId, Guid scooterId, DateTime timeStamp, DigitalTwinsClient digitalTwinsClient)
        {
            var relationship = new BasicRelationship
            {
                TargetId = scooterId.ToString(),
                Name = RelationshipName,
                Properties = new Dictionary<string, object>()
                {
                    { "start", timeStamp }
                }
            };
            await digitalTwinsClient.CreateOrReplaceRelationshipAsync(customerId.ToString(), rentId.ToString(), relationship);
        }

        public static async Task RemoveRentRelationship(Guid rentId, Guid customerId, DigitalTwinsClient digitalTwinsClient)
        {
            await digitalTwinsClient.DeleteRelationshipAsync(customerId.ToString(), rentId.ToString());
        }
    }
}
