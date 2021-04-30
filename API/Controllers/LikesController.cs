using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        /// <summary>
        /// 使用者按喜歡另一個使用者
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUSer = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
            
            if(likedUSer == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("發生錯誤!!使用者不能對自己按喜歡");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUSer.Id);
            if(userLike != null) return BadRequest("您已經按了喜歡");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUSer.Id
            };
            
            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("對使用者按喜歡發生錯誤");
        }

        /// <summary>
        /// 取得使用者喜歡的所有人
        /// </summary>
        /// <param name="predicate">查詢參數(liked：按喜歡的人，likedBy：喜歡你的人)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage);

            return Ok(users);
        }        
    }
}