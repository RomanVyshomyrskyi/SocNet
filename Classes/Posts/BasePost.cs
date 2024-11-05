using System;

namespace My_SocNet_Win.Classes.Posts
{
    public abstract class BasePost
    {   
        public int ID { get; set; }
        public int CreatorID { get; set; }
        public string Text { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<byte[]> Images { get; set; } = new List<byte[]>();
    }
}
