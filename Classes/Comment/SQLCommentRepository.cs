using System;
using My_SocNet_Win.Classes.DB.MSSQL;

namespace My_SocNet_Win.Classes.Comment;

public class SQLCommentRepository : ICommentRepository<BaseComment>
{

private readonly MssqlService _mssqlService;

    public SQLCommentRepository(MssqlService mssqlService)
    {
        _mssqlService = mssqlService;
    }

    public async Task<BaseComment> CreateComment(BaseComment comment)
    {
        using (var connection = _mssqlService.GetConnection())
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO Comments (PostID, CreatorID, Text, IsDeleted, LastCreatorPostID, DateOfCreation, Likes, Dislikes)
                        VALUES (@PostID, @CreatorID, @Text, @IsDeleted, @LastCreatorPostID, @DateOfCreation, @Likes, @Dislikes);
                        SELECT SCOPE_IDENTITY();";
                    command.Parameters.AddWithValue("@PostID", comment.PostID);
                    command.Parameters.AddWithValue("@CreatorID", comment.CreatorID);
                    command.Parameters.AddWithValue("@Text", comment.Text);
                    command.Parameters.AddWithValue("@IsDeleted", comment.IsDeleted);
                    command.Parameters.AddWithValue("@LastCreatorPostID", comment.LastCreatorPostID);
                    command.Parameters.AddWithValue("@DateOfCreation", comment.DateOfCreation);
                    command.Parameters.AddWithValue("@Likes", comment.Likes);
                    command.Parameters.AddWithValue("@Dislikes", comment.Dislikes);

                    comment.ID = Convert.ToInt32(await command.ExecuteScalarAsync());

                    transaction.Commit();
                    return comment;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public async Task<BaseComment> DeleteComment(int id)
    {
        using (var connection = _mssqlService.GetConnection())
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Comments
                SET IsDeleted = 1
                WHERE ID = @ID;";
            command.Parameters.AddWithValue("@ID", id);

            await command.ExecuteNonQueryAsync();
            return await GetComment(id);
        }
    }

    public async Task<BaseComment> GetComment(int id)
    {
        using (var connection = _mssqlService.GetConnection())
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT ID, PostID, CreatorID, Text, IsDeleted, LastCreatorPostID, DateOfCreation, Likes, Dislikes
                FROM Comments
                WHERE ID = @ID;";
            command.Parameters.AddWithValue("@ID", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Comment
                    {
                        ID = reader.GetInt32(0),
                        PostID = reader.GetInt32(1),
                        CreatorID = reader.GetInt32(2),
                        Text = reader.GetString(3),
                        IsDeleted = reader.GetBoolean(4),
                        LastCreatorPostID = reader.GetInt32(5),
                        DateOfCreation = reader.GetDateTime(6),
                        Likes = reader.GetInt32(7),
                        Dislikes = reader.GetInt32(8)
                    };
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public async Task<List<BaseComment>> GetComments(int count)
    {
        using (var connection = _mssqlService.GetConnection())
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT TOP (@Count) ID, PostID, CreatorID, Text, IsDeleted, LastCreatorPostID, DateOfCreation, Likes, Dislikes
                FROM Comments
                WHERE IsDeleted = 0
                ORDER BY ID DESC;";
            command.Parameters.AddWithValue("@Count", count);

            var comments = new List<BaseComment>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    comments.Add(new Comment
                    {
                        ID = reader.GetInt32(0),
                        PostID = reader.GetInt32(1),
                        CreatorID = reader.GetInt32(2),
                        Text = reader.GetString(3),
                        IsDeleted = reader.GetBoolean(4),
                        LastCreatorPostID = reader.GetInt32(5),
                        DateOfCreation = reader.GetDateTime(6),
                        Likes = reader.GetInt32(7),
                        Dislikes = reader.GetInt32(8)
                    });
                }
            }

            return comments;
        }
    }

    public async Task<BaseComment> UpdateComment(BaseComment comment)
    {
        using (var connection = _mssqlService.GetConnection())
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"
                        UPDATE Comments
                        SET PostID = @PostID, Text = @Text, IsDeleted = @IsDeleted, LastCreatorPostID = @LastCreatorPostID, DateOfCreation = @DateOfCreation, Likes = @Likes, Dislikes = @Dislikes
                        WHERE ID = @ID;";
                    command.Parameters.AddWithValue("@ID", comment.ID);
                    command.Parameters.AddWithValue("@PostID", comment.PostID);
                    command.Parameters.AddWithValue("@Text", comment.Text);
                    command.Parameters.AddWithValue("@IsDeleted", comment.IsDeleted);
                    command.Parameters.AddWithValue("@LastCreatorPostID", comment.LastCreatorPostID);
                    command.Parameters.AddWithValue("@DateOfCreation", comment.DateOfCreation);
                    command.Parameters.AddWithValue("@Likes", comment.Likes);
                    command.Parameters.AddWithValue("@Dislikes", comment.Dislikes);

                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                    return comment;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
