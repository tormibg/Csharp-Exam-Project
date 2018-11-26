﻿namespace Forum.Web.Controllers.Post
{
    using Forum.Web.Services.Contracts;
    using Forum.Web.ViewModels.Post;
    using Microsoft.AspNetCore.Mvc;

    public class PostController : BaseController
    {
        public PostController(IAccountService accountService) 
            : base(accountService)
        {
        }

        public IActionResult Create()
        {
            //TODO: Create a proper view for creating posts
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(PostInputModel model)
        {
            //TODO: Finish
            return this.Redirect("/");
        }
    }
}