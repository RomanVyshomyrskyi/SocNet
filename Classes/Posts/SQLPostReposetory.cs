using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using My_SocNet_Win.Classes.DB.MSSQL;

namespace My_SocNet_Win.Classes.Posts
{
    public class SQLPostReposetory : IPostRepository<BasePost>
    {
        private readonly MssqlService _mssqlService;
        public SQLPostReposetory(MssqlService mssqlService)
        {
            _mssqlService = mssqlService;
        }

        public async Task<BasePost> CreatePost(BasePost post)
        {
            using (var connection = _mssqlService.GetConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get the last post ID created by the user
                        var lastPostCommand = connection.CreateCommand();
                        lastPostCommand.Transaction = transaction;
                        lastPostCommand.CommandText = @"
                            SELECT TOP 1 ID
                            FROM BasePosts
                            WHERE CreatorID = @CreatorID
                            ORDER BY DateOfCreation DESC";
                        lastPostCommand.Parameters.AddWithValue("@CreatorID", post.CreatorID);

                        var lastPostId = await lastPostCommand.ExecuteScalarAsync();
                        post.LastCreatorPostID = lastPostId != null ? Convert.ToInt32(lastPostId) : 0;

                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO BasePosts (CreatorID, Text, IsDeleted, LastCreatorPostID, DateOfCreation, Likes, Dislikes)
                            VALUES (@CreatorID, @Text, @IsDeleted, @LastCreatorPostID, @DateOfCreation, @Likes, @Dislikes);
                            SELECT SCOPE_IDENTITY();";
                        command.Parameters.AddWithValue("@CreatorID", post.CreatorID);
                        command.Parameters.AddWithValue("@Text", post.Text);
                        command.Parameters.AddWithValue("@IsDeleted", post.IsDeleted);
                        command.Parameters.AddWithValue("@LastCreatorPostID", post.LastCreatorPostID);
                        command.Parameters.AddWithValue("@DateOfCreation", post.DateOfCreation);
                        command.Parameters.AddWithValue("@Likes", post.Likes);
                        command.Parameters.AddWithValue("@Dislikes", post.Dislikes);

                        post.ID = Convert.ToInt32(await command.ExecuteScalarAsync());

                        foreach (var image in post.Images)
                        {
                            var imageCommand = connection.CreateCommand();
                            imageCommand.Transaction = transaction;
                            imageCommand.CommandText = @"
                                INSERT INTO PostImages (PostID, Image)
                                VALUES (@PostID, @Image);";
                            imageCommand.Parameters.AddWithValue("@PostID", post.ID);
                            imageCommand.Parameters.AddWithValue("@Image", image);
                            await imageCommand.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return post;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<BasePost> DeletePost(Guid id)
        {
            using (var connection = _mssqlService.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE BasePosts
                    SET IsDeleted = 1
                    WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", id);

                await command.ExecuteNonQueryAsync();
                return await GetPost(id);
            }
        }

        public async Task<BasePost> GetPost(Guid id)
        {
            using (var connection = _mssqlService.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT ID, CreatorID, Text, IsDeleted, LastCreatorPostID, DateOfCreation, Likes, Dislikes
                    FROM BasePosts
                    WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var post = new Post
                        {
                            ID = reader.GetInt32(0),
                            CreatorID = reader.GetInt32(1),
                            Text = reader.GetString(2),
                            IsDeleted = reader.GetBoolean(3),
                            LastCreatorPostID = reader.GetInt32(4),
                            DateOfCreation = reader.GetDateTime(5),
                            Likes = reader.GetInt32(6),
                            Dislikes = reader.GetInt32(7),
                            Images = new List<byte[]>()
                        };

                        var imageCommand = connection.CreateCommand();
                        imageCommand.CommandText = @"
                            SELECT Image
                            FROM PostImages
                            WHERE PostID = @PostID;";
                        imageCommand.Parameters.AddWithValue("@PostID", post.ID);

                        using (var imageReader = await imageCommand.ExecuteReaderAsync())
                        {
                            while (await imageReader.ReadAsync())
                            {
                                post.Images.Add((byte[])imageReader["Image"]);
                            }
                        }

                        return post;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public async Task<List<BasePost>> GetPosts(int count)
        {
            using (var connection = _mssqlService.GetConnection())
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP (@Count) ID, CreatorID, Text, IsDeleted, LastCreatorPostID, DateOfCreation, Likes, Dislikes
                    FROM BasePosts
                    WHERE IsDeleted = 0
                    ORDER BY ID DESC;";
                command.Parameters.AddWithValue("@Count", count);

                var posts = new List<BasePost>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var post = new Post
                        {
                            ID = reader.GetInt32(0),
                            CreatorID = reader.GetInt32(1),
                            Text = reader.GetString(2),
                            IsDeleted = reader.GetBoolean(3),
                            LastCreatorPostID = reader.GetInt32(4),
                            DateOfCreation = reader.GetDateTime(5),
                            Likes = reader.GetInt32(6),
                            Dislikes = reader.GetInt32(7),
                            Images = new List<byte[]>()
                        };

                        var imageCommand = connection.CreateCommand();
                        imageCommand.CommandText = @"
                            SELECT Image
                            FROM PostImages
                            WHERE PostID = @PostID;";
                        imageCommand.Parameters.AddWithValue("@PostID", post.ID);

                        using (var imageReader = await imageCommand.ExecuteReaderAsync())
                        {
                            while (await imageReader.ReadAsync())
                            {
                                post.Images.Add((byte[])imageReader["Image"]);
                            }
                        }

                        posts.Add(post);
                    }
                }

                return posts;
            }
        }

        public async Task<BasePost> UpdatePost(BasePost post)
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
                            UPDATE BasePosts
                            SET Text = @Text, IsDeleted = @IsDeleted, LastCreatorPostID = @LastCreatorPostID, DateOfCreation = @DateOfCreation, Likes = @Likes, Dislikes = @Dislikes
                            WHERE ID = @ID;";
                        command.Parameters.AddWithValue("@ID", post.ID);
                        command.Parameters.AddWithValue("@Text", post.Text);
                        command.Parameters.AddWithValue("@IsDeleted", post.IsDeleted);
                        command.Parameters.AddWithValue("@LastCreatorPostID", post.LastCreatorPostID);
                        command.Parameters.AddWithValue("@DateOfCreation", post.DateOfCreation);
                        command.Parameters.AddWithValue("@Likes", post.Likes);
                        command.Parameters.AddWithValue("@Dislikes", post.Dislikes);

                        await command.ExecuteNonQueryAsync();

                        var deleteImagesCommand = connection.CreateCommand();
                        deleteImagesCommand.Transaction = transaction;
                        deleteImagesCommand.CommandText = @"
                            DELETE FROM PostImages
                            WHERE PostID = @PostID;";
                        deleteImagesCommand.Parameters.AddWithValue("@PostID", post.ID);
                        await deleteImagesCommand.ExecuteNonQueryAsync();

                        foreach (var image in post.Images)
                        {
                            var imageCommand = connection.CreateCommand();
                            imageCommand.Transaction = transaction;
                            imageCommand.CommandText = @"
                                INSERT INTO PostImages (PostID, Image)
                                VALUES (@PostID, @Image);";
                            imageCommand.Parameters.AddWithValue("@PostID", post.ID);
                            imageCommand.Parameters.AddWithValue("@Image", image);
                            await imageCommand.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        return post;
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

    public interface IPostRepository<T>
    {
    }
}
