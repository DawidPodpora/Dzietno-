using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;


namespace Dzietność { 

public class Uzytkownik
    {
        [BsonId]
        [JsonProperty("nameid")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("role")]
        public string password { get; set; }
    }
}