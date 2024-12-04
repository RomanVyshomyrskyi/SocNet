using System;
using System.Collections.Generic;

namespace My_SocNet_Win.Classes.Posts
{
    public class BasePost
    {
        public int ID { get; set; }
        public int CreatorID { get; set; }
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public List<byte[]> Images { get; set; } = new List<byte[]>();
        public int LastCreatorPostID { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
    }
}
