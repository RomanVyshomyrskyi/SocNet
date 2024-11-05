using System;

namespace My_SocNet_Win.Classes.Posts
{
    public abstract class BasePost
    {
        public Guid ID { get; set; }
        public Guid CreatorID { get; set; }
        public byte[] Images { get; set; }
        public string Text { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
