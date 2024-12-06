using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace My_SocNet_Win.Classes.Posts
{
    public class MongoPostRepository : IPostRepository<BasePost>
    {
        private readonly IMongoCollection<BasePost> _postsCollection;
        private readonly IMongoCollection<Counter> _counterCollection;

        static MongoPostRepository()
        {
            // Register the class map for BasePost
            if (!BsonClassMap.IsClassMapRegistered(typeof(BasePost)))
            {
                BsonClassMap.RegisterClassMap<BasePost>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.ID).SetSerializer(new Int32Serializer(BsonType.Int32));
                });
            }
        }

        public MongoPostRepository(IMongoDatabase database)
        {
            _postsCollection = database.GetCollection<BasePost>("Posts");
            _counterCollection = database.GetCollection<Counter>("Counters");
        }

        public async Task<BasePost> CreatePost(BasePost post)
        {
            post.DateOfCreation = DateTime.UtcNow;
            post.ID = await GetNextSequenceValueAsync("PostId");

            // Get the last post ID created by the user
            var lastPost = await _postsCollection
                .Find(p => p.CreatorID == post.CreatorID)
                .SortByDescending(p => p.DateOfCreation)
                .FirstOrDefaultAsync();

            post.LastCreatorPostID = lastPost?.ID ?? 0;

            await _postsCollection.InsertOneAsync(post);
            return post;
        }

        public async Task<BasePost> DeletePost(int id)
        {
            var filter = Builders<BasePost>.Filter.Eq(nameof(BasePost.ID), id);
            var update = Builders<BasePost>.Update.Set(p => p.IsDeleted, true);
            var result = await _postsCollection.FindOneAndUpdateAsync(filter, update);
            return result;
        }

        public async Task<BasePost> GetPost(int id)
        {
            return await _postsCollection.Find(p => p.ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<BasePost>> GetPosts(int count)
        {
            return await _postsCollection
                .Find(p => !p.IsDeleted)
                .SortByDescending(p => p.DateOfCreation)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<BasePost> UpdatePost(BasePost post)
        {
            var filter = Builders<BasePost>.Filter.Eq(p => p.ID, post.ID);
            var update = Builders<BasePost>.Update
                .Set(p => p.Title, post.Title)
                .Set(p => p.Text, post.Text)
                .Set(p => p.IsDeleted, post.IsDeleted)
                .Set(p => p.LastCreatorPostID, post.LastCreatorPostID)
                .Set(p => p.DateOfCreation, post.DateOfCreation)
                .Set(p => p.Likes, post.Likes)
                .Set(p => p.Dislikes, post.Dislikes)
                .Set(p => p.Images, post.Images);

            var options = new FindOneAndUpdateOptions<BasePost>
            {
                ReturnDocument = ReturnDocument.After
            };

            var result = await _postsCollection.FindOneAndUpdateAsync(filter, update, options);
            return result;
        }

        private async Task<int> GetNextSequenceValueAsync(string sequenceName)
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, sequenceName);
            var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);
            var options = new FindOneAndUpdateOptions<Counter>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var counter = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
            return counter.SequenceValue;
        }
    }

    public class Counter
    {
        public string Id { get; set; }
        public int SequenceValue { get; set; }
    }
}
