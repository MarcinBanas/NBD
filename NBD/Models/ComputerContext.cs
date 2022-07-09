
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;

namespace NBD.Models
{
    public class ComputerContext
    {
        IMongoDatabase database;
        IGridFSBucket gridFS;

        public ComputerContext()
        {

            string connectionString = "mongodb://localhost:27017/computerstore";
            var connection = new MongoUrl(connectionString);

            var client = new MongoClient(connectionString);

            database = client.GetDatabase(connection.DatabaseName);

            gridFS = new GridFSBucket(database);
        }

        public IMongoCollection<Computer> Computers
        {
            get { return database.GetCollection<Computer>("Computers"); }
        }

        public async Task<IEnumerable<Computer>> GetComputers(int? year, string name)
        {

            var builder = new FilterDefinitionBuilder<Computer>();
            var filter = builder.Empty;

            if (!String.IsNullOrWhiteSpace(name))
            {
                filter = filter & builder.Regex("Name", new BsonRegularExpression(name));
            }
            if (year.HasValue)
            {
                filter = filter & builder.Eq("Year", year.Value);
            }

            return await Computers.Find(filter).ToListAsync();
        }


        public async Task<Computer> GetComputer(string id)
        {
            return await Computers.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }

        public async Task Create(Computer c)
        {
            await Computers.InsertOneAsync(c);
        }

        public async Task Update(Computer c)
        {
            await Computers.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(c.Id)), c);
        }

        public async Task Remove(string id)
        {
            await Computers.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }

        public async Task<byte[]> GetImage(string id)
        {
            return await gridFS.DownloadAsBytesAsync(new ObjectId(id));
        }

        public async Task StoreImage(string id, Stream imageStream, string imageName)
        {
            var computer = await GetComputer(id);
            if (computer.HasImage())
            {

                await gridFS.DeleteAsync(new ObjectId(computer.ImageId));
            }

            var imageId = await gridFS.UploadFromStreamAsync(imageName, imageStream);

            computer.ImageId = imageId.ToString();
            var filter = Builders<Computer>.Filter.Eq("_id", new ObjectId(computer.Id));
            var update = Builders<Computer>.Update.Set("ImageId", computer.ImageId);
            await Computers.UpdateOneAsync(filter, update);
        }
    }
}
