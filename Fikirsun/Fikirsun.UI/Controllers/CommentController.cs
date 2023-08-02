using Fikirsun.DAL.Context;
using Fikirsun.Entities;
using Fikirsun.Tools;
using Fikirsun.UI.Models;
using Fikirsun.UI.Models.CommentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Fikirsun.UI.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly FikirsunContext _db;
        public CommentController(FikirsunContext context)
        {
            _db = context;
        }
        
        #region comment

        [HttpPost]
        public async Task<JsonResult> CreateComment([FromBody] CreateCommentModel model)
        {
            if (!ModelState.IsValid)
            {
                Json(false);
            }
            var spams = _db.SpamWords.ToArray();
            var haveSpam = SpamHelper.Invoke(spams, model.Content);

            if (!string.IsNullOrEmpty(haveSpam)) // geriye bir mesaj dönmüş spam kelime var demek
            {
                return Json(haveSpam);
            }
            var user = _db.Users
                .Include(x => x.likedComments)
                .FirstOrDefault(x => x.UserName == User.Identity!.Name);

            var post = _db.Posts
                .Include(x => x.user)
                .FirstOrDefault(x => x.Id == model.postId);
            if (post == null)
            {
                return Json("Soru bulunamadı , sayfayı yenileyiniz");

            }
            var comment = new Comment()
            {
                Content = model.Content,
                postId = model.postId,
                userId = user!.Id
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            if (post.userId != user.Id) //kişi kendisine bildirim göndersin istemeyiz :)
            {
                _db.Notifications.Add(new Notification
                {
                    Content = $"{user.UserName} sorunuza yorum attı ! '{comment.Content.Substring(0, comment.Content.Length / 3)}..'",
                    returnUrl = $"/Post/Index/{comment.postId}#comment{comment.Id}",
                    userId = post.user!.Id,
                    NotifyType = (byte)NotificationType.Info,
                    signDate = DateTime.Now
                });
                _db.SaveChanges();
            }

            return Json(comment.Id);

        }
        [HttpPost]
        public async Task<JsonResult> DeleteComment([FromBody] int commentId)
        {
            var deletedComment = _db.Comments.FirstOrDefault(x => x.Id == commentId);
            if (deletedComment == null)
            {
                return Json(false);
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (User.IsInRole("Member"))
            {
                if (deletedComment.userId != userId)
                {
                    return Json(false);
                }
            }

            _db.Comments.Remove(deletedComment);
            await _db.SaveChangesAsync();
            return Json(true);
        }

        [HttpPost]
        public JsonResult LikeComment([FromBody] int commentId)
        {
            var comment = _db.Comments.Include(x => x.user).FirstOrDefault(x => x.Id == commentId);
            if (comment == null)
            {
                return Json("404");
            }
            var user = _db.Users
                .Include(x => x.likedComments)
                .FirstOrDefault(x => x.UserName == User.Identity!.Name);

            var isLikedBefore = user!.likedComments.FirstOrDefault(x => x.commentId == commentId);

            if (isLikedBefore != null)
            {
                return Json("Bu yorumu öncesinde beğenmişsiniz");
            }
            var likedComment = new LikedComment()
            {
                commentId = commentId,
                userId = user!.Id
            };
            _db.LikedComments.Add(likedComment);
            comment.likeCount += 1;
            _db.SaveChanges();
            //bildirim gönder
            if (comment.userId != user.Id)
            {
                _db.Notifications.Add(new Notification
                {
                    Content = $"{user.UserName} yorumunuzu beğendi !",
                    returnUrl = $"/Post/Index/{comment.postId}#comment{comment.Id}",
                    userId = comment.user!.Id,
                    NotifyType = (byte)NotificationType.Like,
                    signDate = DateTime.Now
                });
                _db.SaveChanges();
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult DislikeComment([FromBody] int commentId)
        {
            var comment = _db.Comments.FirstOrDefault(x => x.Id == commentId);
            if (comment == null)
            {
                return Json("404");
            }
            var user = _db.Users
                .Include(x => x.likedComments)
                .FirstOrDefault(x => x.UserName == User.Identity!.Name);


            var likedComment = user!.likedComments.FirstOrDefault(x => x.commentId == commentId);
            if (likedComment != null)
            {
                _db.LikedComments.Remove(likedComment);
                comment.likeCount -= 1;
                _db.SaveChanges();

            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult AnswerComment([FromBody] AnswerCommentModel model)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var comment = _db
                .Comments
                .Include(x => x.user)
                .Include(x => x.Post)
                .ThenInclude(x => x.user)
                .FirstOrDefault(x => x.Id == model.commentId && x.postId == model.postId);

            if (comment == null)
            {
                return Json("404");

            }
            var post = _db.Posts
                .Include(x => x.comments)?
                .FirstOrDefault(x => x.Id == model.postId);
            if (post == null)
            {
                return Json("404");
            }
            if (post.userId != userId)
            {
                return Json("Bu işlem için yetkiniz yok");
            }
            var isAnsweredBefore = post!
                .comments
                .FirstOrDefault(x => x.isAnswer == true);

            if (isAnsweredBefore != null && isAnsweredBefore?.Id != model.commentId)
            //bu postun altında daha önce cevaplanmış yorum varmı ve bu yorum bizim gönderdiğimiz yorum mu?
            {
                isAnsweredBefore!.isAnswer = false; //varsa onu değiştirir
            }

            comment.isAnswer = !comment.isAnswer;
            _db.SaveChanges();
            if (comment.isAnswer)
            {
                //bildirim gönder
                if (comment.userId != userId)
                {
                    _db.Notifications.Add(new Notification
                    {
                        Content = $"{comment.Post.user!.UserName} yorumunuzu onayladı !",
                        returnUrl = $"/Post/Index/{comment.postId}#comment{comment.Id}",
                        userId = comment.user!.Id,
                        NotifyType = (byte)NotificationType.Success,
                        signDate = DateTime.Now
                    });
                    _db.SaveChanges();
                }

                return Json("answered");
            }
            else
            {
                return Json("unanswered");
            }
        }
        #endregion

        #region reply

        [HttpPost]
        public async Task<JsonResult> CreateReply([FromBody] CreateReplyModel model)
        {
            if (!ModelState.IsValid)
            {
                Json(false);
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var user = _db.Users.FirstOrDefault(x => x.Id == userId);

            var reply = new Reply()
            {
                Content = model.Content,
                commentId = model.commentId,
                userId = userId

            };

            var comment = _db.Comments
                            .Include(x => x.user)
                            .FirstOrDefault(x => x.Id == model.commentId);
            if (comment == null)
            {
                return Json(false);
            }
            var repliedUser = _db.Users.FirstOrDefault(x => x.Id == model.replyUserId);
            if (repliedUser != null)
            {
                reply.repliedUserId = repliedUser.Id;
            }

            _db.Replies.Add(reply);
            await _db.SaveChangesAsync();

            //bildirim gönder

            if (comment!.userId != userId)
            {
                _db.Notifications.Add(new Notification
                {
                    Content = $"{user!.UserName} yorumunuza bir yanıt attı ! '{comment.Content.Substring(0, comment.Content.Length / 3)}..'",
                    returnUrl = $"/Post/Index/{comment.postId}#comment{comment.Id}",
                    userId = comment.user!.Id,
                    NotifyType = (byte)NotificationType.Info,
                    signDate = DateTime.Now
                });
                _db.SaveChanges();
            }
            if (repliedUser != null && repliedUser?.Id != userId)
            {
                _db.Notifications.Add(new Notification
                {
                    Content = $"{user!.UserName} sizi yanıtladı ! '" +
                    $"{reply.Content.Substring(0, reply.Content.Length / 3)}..'",
                    returnUrl = $"/Post/Index/{comment.postId}#comment{comment.Id}",
                    userId = repliedUser!.Id,
                    NotifyType = (byte)NotificationType.Info,
                    signDate = DateTime.Now
                });
                _db.SaveChanges();

            }
            return Json(reply.Id);

        }
        [HttpPost]
        public async Task<JsonResult> DeleteReply([FromBody] int replyId)
        {
            var deletedReply = _db.Replies.FirstOrDefault(x => x.Id == replyId);
            if (deletedReply == null)
            {
                return Json(false);
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (User.IsInRole("Member"))
            {
                if (deletedReply.userId != userId)
                {
                    return Json(false);
                }
            }

            _db.Replies.Remove(deletedReply);
            await _db.SaveChangesAsync();
            return Json(true);
        }
        #endregion
    }
}
