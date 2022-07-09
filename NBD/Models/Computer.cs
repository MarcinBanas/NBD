﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NBD.Models
{
    public class Computer
    {
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [Display(Name = "Computer name")]

        public string Name { get; set; }

        [Display(Name = "Creation year")]

        public int Year { get; set; }

        public string ImageId { get; set; }

        public bool HasImage() => !string.IsNullOrWhiteSpace(ImageId);
    }
}
