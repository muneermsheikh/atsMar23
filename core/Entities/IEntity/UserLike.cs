using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.Identity;

namespace core.Entities.Identity
{
    
    [NotMapped]
    public class UserLike
    {
          public UserLike(AppUser sourceUser, AppUser likedUser) 
        {
             this.SourceUser = sourceUser;
     this.LikedUser = likedUser;
   
        }
                public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}