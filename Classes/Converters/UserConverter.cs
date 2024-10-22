using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Classes.Converters
{
    public static class UserConverter
    {
        public static ConcreteUser ConvertToBaseUser<TUser>(TUser user) where TUser : BaseUsers
        {
            return new ConcreteUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                Friends = user.Friends,
                DateOfCreation = user.DateOfCreation,
                LastLogin = user.LastLogin,
                Roles = user.Roles,
                ImgBinary = user.ImgBinary
            };
        }
    }
}