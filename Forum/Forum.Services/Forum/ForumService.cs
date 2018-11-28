﻿namespace Forum.Services.Forum
{
    using global::Forum.Models;
    using global::Forum.Services.Category.Contracts;
    using global::Forum.Services.Db;
    using global::Forum.Services.Forum.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;

    public class ForumService : IForumService
    {
        private readonly DbService dbService;
        private readonly ICategoryService categoryService;

        public ForumService(DbService dbService, ICategoryService categoryService)
        {
            this.dbService = dbService;
            this.categoryService = categoryService;
        }

        public SubForum GetForum(string id)
        {
            SubForum forum = this.dbService
                .DbContext
                .Forums
                .Include(f => f.Category)
                .Include(f => f.Posts)
                .ThenInclude(f => f.Author)
                .FirstOrDefault(f => f.Id == id);

            return forum;
        }

        public void Add(SubForum model, string categoryName)
        {
            Category category = this.categoryService.GetCategory(categoryName);

            model.CreatedOn = DateTime.UtcNow;
            model.CategoryId = category.Id;
            model.Category = category;
            
            this.dbService.DbContext.Forums.Add(model);
            this.dbService.DbContext.SaveChanges();
        }

        public SubForum GetPostsByForum(string id)
        {
            SubForum forum = this.GetForum(id);

            return forum;
        }
    }
}