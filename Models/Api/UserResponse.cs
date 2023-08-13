using AdrianTube2.Models.UserModels;

namespace AdrianTube2.Models.Api
{
    public class UserResponse : Response
    {
        public UserWithStringId? User { get; set; }
    }
}