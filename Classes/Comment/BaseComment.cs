using System;
using My_SocNet_Win.Classes.Posts;

namespace My_SocNet_Win.Classes.Comment;

public abstract class BaseComment : BasePost
{
    public int PostID { get; set; }
}
