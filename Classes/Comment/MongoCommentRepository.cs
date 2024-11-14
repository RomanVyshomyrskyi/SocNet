using System;
using MongoDB.Driver;

namespace My_SocNet_Win.Classes.Comment;

public class MongoCommentRepository : ICommentRepository<BaseComment>
{
    private readonly IMongoCollection<BaseComment> _commentsCollection;

    public MongoCommentRepository(IMongoDatabase database)
    {
        _commentsCollection = database.GetCollection<BaseComment>("Comments");
    }

    public async Task<BaseComment> CreateComment(BaseComment comment)
    {
        comment.DateOfCreation = DateTime.UtcNow;

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

}
