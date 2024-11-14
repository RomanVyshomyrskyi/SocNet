using System;

namespace My_SocNet_Win.Classes.Comment;

public interface ICommentRepository<T> where T : BaseComment
{
    Task<T> GetComment(int id);
    Task<List<T>> GetComments(int count);
    Task<T> CreateComment(T comment);
    Task<T> UpdateComment(T comment);
    Task<T> DeleteComment(int id);
}
