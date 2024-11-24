using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My_SocNet_Win.Classes.Posts
{
    public interface IPostRepository<T> where T : BasePost
    {
        Task<T> GetPost(int id);
        Task<List<T>> GetPosts(int count);
        Task<T> CreatePost(T post);
        Task<T> UpdatePost(T post);
        Task<T> DeletePost(int id);
    }
}
