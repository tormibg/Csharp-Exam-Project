﻿using Forum.Models;
using Forum.ViewModels.Interfaces.Forum;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Security.Claims;

namespace Forum.Services.Interfaces.Forum
{
    public interface IForumService
    {
        bool ForumExists(string name);

        void Add(IForumFormInputModel model, string category);

        IEnumerable<Models.Post> GetPostsByForum(string Id, int start);

        SubForum GetForum(string Id, ModelStateDictionary modelState);

        void Edit(IForumInputModel model, string forumId);

        IForumFormInputModel GetMappedForumModel(SubForum forum);

        void Delete(SubForum forum);

        IEnumerable<SubForum> GetAllForums(ClaimsPrincipal principal);

        IEnumerable<string> GetAllForumsIds(ClaimsPrincipal principal, ModelStateDictionary modelState, string forumId);

        IEnumerable<string> GetForumPostsIds(string id);
    }
}