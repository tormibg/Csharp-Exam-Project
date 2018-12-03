﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Forum.Web.Services.Account.Contracts;
using Forum.Services.Interfaces.Category;
using Forum.Services.Interfaces.Forum;
using Forum.ViewModels.Forum;

namespace Forum.Web.Controllers.Forum
{
    [Authorize("Admin")]
    public class ForumController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IForumService forumService;

        public ForumController(IAccountService accountService, ICategoryService categoryService, IForumService forumService)
            : base(accountService)
        {
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
                    Text = $"{x.Name} ({x.ForumsCount})"
                })
                .ToArray();
                

            ForumFormInputModel model = new ForumFormInputModel
            {
                Categories = namesList
            };
            
            return this.View(model);
        }

        [HttpPost]
        public IActionResult Create(ForumFormInputModel model)
        {
            if(ModelState.IsValid)
            {
                this.forumService.Add(model, model.ForumModel.Category);

                return this.Redirect("/");
            }
            else
            {
                var names = this.categoryService.GetAllCategories().GetAwaiter().GetResult();

                var namesList =
                    names
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = $"{x.Name} ({x.ForumsCount})"
                    })
                    .ToArray();

                ForumFormInputModel viewModel = new ForumFormInputModel
                {
                    ForumModel = model.ForumModel,
                    Categories = namesList
                };

                return this.View(viewModel);
            }
        }

        [AllowAnonymous]
        public IActionResult Posts(string id)
        {
            var forum = this.forumService.GetPostsByForum(id).GetAwaiter().GetResult();

            var model = new ForumPostsInputModel
            {
                Forum = forum,
                Posts = forum.Posts
            };
            
            return this.View(model);
        }
    }
}