﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDbCRUD.Repository
{
    public class BaseRepository<T>: IRepository<T> where T : BaseEntity
    {
        private IMongoClient MongoClient;

        private IMongoDatabase Database;

        private IMongoCollection<T> Collection;

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public BaseRepository()
        {
            MongoClient = new MongoClient(Program.MainConnectionString);
            Database = MongoClient.GetDatabase(Program.MainDatabaseName);
            Collection = Database.GetCollection<T>(GetCollectionName(typeof(T)));
        }

        public async void Create(T entity)
        {
            
            await Collection.InsertOneAsync(entity);
        }

        public async Task<IList<T>> GetAll()
        {            
            return await Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<T> GetById(string id)
        {            
            var filter = Builders<T>.Filter;
            var condition = filter.Eq(x => x.Id, id);
            
            return await Collection.Find(condition).FirstOrDefaultAsync();
        }

        public async void UpdateById(T entity)
        {
            var filter = Builders<T>.Filter;
            var condition = filter.Eq(x => x.Id, entity.Id);
            
            await Collection.ReplaceOneAsync(condition, entity);
        }
        public async void DeleteById(string id)
        {
            var filter = Builders<T>.Filter;
            var condition = filter.Eq(x => x.Id, id);

            await Collection.DeleteOneAsync(condition);
        }
    }
}
