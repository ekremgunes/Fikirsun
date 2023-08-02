using Fikirsun.Entities;

namespace Fikirsun.Tools
{
    public static class Popularity
    {
        public enum Priority
        {
            Time = 0,
            Like = 1
        }
        public static float Invoke(Post post, Priority priority)
        {
            var postLikeCount = post.likeCount + .003f;
            var postViewCount = post.viewCount + .03f;

            if (priority == Priority.Like)
            {
                float popularity = (float)
                (

                    (postLikeCount / postViewCount) * (postLikeCount / 1.11f)
                    + post.comments.Count * .0133f
                    + post.comments.Sum(c => c.replies.Count) * .015f
                    + post.comments.Sum(c => c.likeCount) * .554f
                    + (post.createdDate.Subtract(DateTime.Now).TotalHours * .055f)
                );

                return popularity;
            }
            else
            {

                float popularity = (float)
                (

                    (postLikeCount / postViewCount) * (postLikeCount / 1.13f)
                    + post.comments.Count * .0133f
                    + post.comments.Sum(c => c.replies.Count) * .015f
                    + post.comments.Sum(c => c.likeCount) * .554f
                    + (post.createdDate.Subtract(DateTime.Now).TotalSeconds * .00133f)
                );

                return popularity;

            }

        }
    }
}
