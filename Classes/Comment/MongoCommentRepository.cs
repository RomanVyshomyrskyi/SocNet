using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace My_SocNet_Win.Classes.Comment
{
    public class MongoCommentRepository : ICommentRepository<BaseComment>
    {
        private readonly IMongoCollection<BaseComment> _commentsCollection;
        private readonly IMongoCollection<Counter> _counterCollection;

        public MongoCommentRepository(IMongoDatabase database)
        {
            _commentsCollection = database.GetCollection<BaseComment>("Comments");
            _counterCollection = database.GetCollection<Counter>("Counters");
        }

        public async Task<BaseComment> CreateComment(BaseComment comment)
        {
            comment.DateOfCreation = DateTime.UtcNow;
            comment.ID = await GetNextSequenceValueAsync("CommentId");

            // Get the last comment ID created by the user
            var lastComment = await _commentsCollection
                .Find(c => c.CreatorID == comment.CreatorID)
                .SortByDescending(c => c.DateOfCreation)
                .FirstOrDefaultAsync();

            comment.LastCreatorPostID = lastComment?.ID ?? 0;

            await _commentsCollection.InsertOneAsync(comment);
            return comment;
        }

        public async Task<BaseComment> DeleteComment(int id)
        {
            var filter = Builders<BaseComment>.Filter.Eq(c => c.ID, id);
            var update = Builders<BaseComment>.Update.Set(c => c.IsDeleted, true);
            var result = await _commentsCollection.FindOneAndUpdateAsync(filter, update);
            return result;
        }

        public async Task<BaseComment> GetComment(int id)
        {
            return await _commentsCollection.Find(c => c.ID == id).FirstOrDefaultAsync();
        }

        public async Task<List<BaseComment>> GetComments(int count)
        {
            return await _commentsCollection
                .Find(c => !c.IsDeleted)
                .SortByDescending(c => c.DateOfCreation)
                .Limit(count)
                .ToListAsync();
        }

        public async Task<BaseComment> UpdateComment(BaseComment comment)
        {
            var filter = Builders<BaseComment>.Filter.Eq(c => c.ID, comment.ID);
            var update = Builders<BaseComment>.Update
                .Set(c => c.PostID, comment.PostID)
                .Set(c => c.Text, comment.Text)
                .Set(c => c.IsDeleted, comment.IsDeleted)
                .Set(c => c.LastCreatorPostID, comment.LastCreatorPostID)
                .Set(c => c.DateOfCreation, comment.DateOfCreation)
                .Set(c => c.Likes, comment.Likes)
                .Set(c => c.Dislikes, comment.Dislikes);

            var result = await _commentsCollection.FindOneAndUpdateAsync(filter, update);
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
}
