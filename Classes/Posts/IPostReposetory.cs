using System;

namespace My_SocNet_Win.Classes.Posts;

public interface IPostReposetory<T> where T : BasePost
{
    Task<T> GetPost(Guid id);
    Task<List<T>> GetPosts(int count);
    Task<T> CreatePost(T post);
    Task<T> UpdatePost(T post);
    Task<T> DeletePost(Guid id);
}
