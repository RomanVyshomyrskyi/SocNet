using System;

namespace My_SocNet_Win.Classes.User;

public class User
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public List<int> Friends { get; set; } = new List<int>();
    public DateTime DateOfCreation { get; set; }
    public DateTime LastLogin { get; set; }

}
