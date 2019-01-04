﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Forum.Web.Services.Account.Contracts;
using Forum.Services.Interfaces.Category;
using Forum.Services.Interfaces.Forum;
using Forum.ViewModels.Forum;
using Forum.Web.Common;
using Forum.Web.Attributes.CustomAuthorizeAttributes;
using Forum.Services.Interfaces.Db;
using System.Net;

namespace Forum.Web.Controllers.Forum
{
    [AuthorizeRoles(Role.Administrator, Role.Owner)]
    public class ForumController : BaseController
    {
        private readonly IDbService dbService;
        private readonly ICategoryService categoryService;
        private readonly IForumService forumService;

        public ForumController(IDbService dbService, IAccountService accountService, ICategoryService categoryService, IForumService forumService)
            : base(accountService)
        {
            this.dbService = dbService;
            this.categoryService = categoryService;
            this.forumService = forumService;
        }

        public IActionResult Create()
        {
            var names = this.categoryService.GetAllCategories().GetAwaiter().GetResult();

            var namesList =
                names
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Name
                })
                .ToList();


            ForumFormInputModel model = new ForumFormInputModel
            {
                Categories = namesList
            };

            return this.View(model);
        }

        [HttpPost]
        public IActionResult Create(ForumFormInputModel model)
        {
            if (ModelState.IsValid)
            {
                this.forumService.Add(model, model.ForumModel.Category);

                return this.Redirect("/");
            }
            else
            {
                var result = this.View("Error", ModelState);
                result.StatusCode = (int)HttpStatusCode.BadRequest;

                return result;
            }
        }

        [AllowAnonymous]
        public IActionResult Posts(string id, int start)
        {
            var forum = this.forumService.GetForum(id);
            var posts = this.forumService.GetPostsByForum(id, start);

            this.ViewData["PostsIds"] = this.forumService.GetForumPostsIds(id);

            var model = new ForumPostsInputModel
            {
                Forum = forum,
                Posts = posts,
                PagesCount = this.forumService.GetPagesCount(forum.Posts.Count())
            };

            return this.View(model);
        }

        public IActionResult Edit(string id)
        {
            var forum = this.forumService.GetForum(id);

            var model = (ForumFormInputModel)this.forumService.GetMappedForumModel(forum);

            this.ViewData["ForumId"] = forum.Id;

            return this.View(model);
        }

        [HttpPost]
        public IActionResult Edit(ForumFormInputModel model, string forumId)
        {
            var forum = this.forumService.GetForum(forumId);

            this.forumService.Edit(model.ForumModel, forumId);

            return this.Redirect("/");
        }

        public IActionResult Delete(string id)
        {
            var forum = this.forumService.GetForum(id);
            //TODO: Validate

            this.forumService.Delete(forum);

            return this.Redirect("/");
        }
    }
}