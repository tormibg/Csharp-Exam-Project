﻿using AutoMapper;
using Forum.Services.Common;
using Forum.ViewModels.Common;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Forum.Services.Post
{
    public class PostService : BaseService, Interfaces.Post.IPostService
    {
        private readonly Interfaces.Quote.IQuoteService quoteService;
        private readonly Interfaces.Forum.IForumService forumService;

        public PostService(IMapper mapper, Interfaces.Quote.IQuoteService quoteService, Interfaces.Db.IDbService dbService, Interfaces.Forum.IForumService forumService)
            : base(mapper, dbService)
        {
            this.quoteService = quoteService;
            this.forumService = forumService;
        }

        public async Task<int> AddPost(ViewModels.Interfaces.Post.IPostInputModel model, Models.ForumUser user, string forumId)
        {
            var post = this.mapper.Map<Models.Post>(model);

            var forum = this.dbService.DbContext.Forums.FirstOrDefault(f => f.Id == forumId);

            post.StartedOn = DateTime.UtcNow;
            post.Description = this.ParseDescription(post.Description);
            post.Views = 0;
            post.Author = user;
            post.AuthorId = user.Id;

            await this.dbService.DbContext.Posts.AddAsync(post);
            return await this.dbService.DbContext.SaveChangesAsync();
        }

        public bool DoesPostExist(string Id)
        {
            var result = this.dbService.DbContext.Posts.Any(p => p.Id == Id);
            return result;
        }

        public int Edit(ViewModels.Interfaces.Post.IEditPostInputModel model)
        {
            var post = this.dbService.DbContext.Posts.Where(p => p.Id == model.Id).FirstOrDefault();

            post.Name = model.Name;
            post.ForumId = model.ForumId;
            post.Description = model.Description;

            return this.dbService.DbContext.SaveChanges();
        }

        public ViewModels.Interfaces.Post.IEditPostInputModel GetEditPostModel(string Id, ClaimsPrincipal principal)
        {
            var post = this.dbService.DbContext.Posts.Include(p => p.Forum).Where(p => p.Id == Id).FirstOrDefault();

            var model = this.mapper.Map<ViewModels.Post.EditPostInputModel>(post);

            model.AllForums = this.forumService.GetAllForums(principal);

            return model;
        }

        public IEnumerable<ViewModels.Interfaces.Post.ILatestPostViewModel> GetLatestPosts()
        {
            var latestPosts =
                this.dbService
                .DbContext
                .Posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.StartedOn)
                .Take(3)
                .Select(p => this.mapper.Map<ViewModels.Post.LatestPostViewModel>(p))
                .ToList();

            return latestPosts;
        }

        public IEnumerable<ViewModels.Interfaces.Post.IPopularPostViewModel> GetPopularPosts()
        {
            var popularPosts =
                this.dbService
                .DbContext
                .Posts
                .OrderByDescending(p => p.Views)
                .Take(3)
                .Select(p => this.mapper.Map<ViewModels.Post.PopularPostViewModel>(p))
                .ToList();

            return popularPosts;
        }

        public ViewModels.Interfaces.Post.IPostViewModel GetPost(string id, int start, ModelStateDictionary modelState)
        {
            Models.Post post =
                this.dbService
                .DbContext
                .Posts
                .Include(p => p.Author)
                .ThenInclude(p => p.Posts)
                .Include(p => p.Replies)
                .ThenInclude(p => p.Author)
                .ThenInclude(p => p.Posts)
                .Include(p => p.Replies)
                .ThenInclude(p => p.Quotes)
                .Include(p => p.Replies)
                .ThenInclude(p => p.Quotes)
                .ThenInclude(p => p.Author)
                .ThenInclude(p => p.Posts)
                .FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                modelState.AddModelError("error", ErrorConstants.InvalidPostIdError);

                return null;
            }

            ViewModels.Post.PostViewModel viewModel = this.mapper.Map<ViewModels.Post.PostViewModel>(post);

            viewModel.Replies = viewModel.Replies.Skip(start).OrderBy(r => r.RepliedOn).Take(5).ToList();

            return viewModel;
        }

        public string ParseDescription(string description)
        {
            string[] inputArray =
                description
                .Split(Environment.NewLine)
                .ToArray();

            var sb = new StringBuilder();

            string pattern = ServicesConstants.ParseTagsRegex;
            Regex tagsRegex = new Regex(pattern);

            ParseTags(inputArray, sb, tagsRegex);

            //Valdiating the allowed tags
            var sanitizer = new HtmlSanitizer(Common.ServicesConstants.AllowedTags);

            var result = sanitizer.Sanitize(sb.ToString().TrimEnd());

            return result;
        }

        private static void ParseTags(string[] inputArray, StringBuilder sb, Regex tagsRegex)
        {
            for (int index = 0; index < inputArray.Length; index++)
            {
                var match = tagsRegex.Match(inputArray[index]);

                if (match.Success)
                {
                    while (match.Success)
                    {
                        match = tagsRegex.Match(inputArray[index]);

                        int lineLength = inputArray[index].Length;
                        if (lineLength < 0)
                        {
                            lineLength = 0;
                        }

                        //getting the text before the match
                        var stringBeggining = inputArray[index].Substring(0, match.Index);

                        //the match
                        //opening tag
                        var openingTag = match.Groups[1].Value;
                        openingTag = openingTag.Replace(']', '>');
                        openingTag = openingTag.Replace('[', '<');

                        //middle text
                        var text = match.Groups[3].Value;

                        //closing tag
                        var closingTag = match.Groups[4].Value;
                        closingTag = closingTag.Replace(']', '>');
                        closingTag = closingTag.Replace('[', '<');

                        int lastMatchIndex = (match.Length + match.Index);
                        if (lastMatchIndex < 0)
                        {
                            lastMatchIndex = 0;
                        }
                        //getting the text after the match
                        var restOfString = inputArray[index].Substring(lastMatchIndex, lineLength - lastMatchIndex);

                        inputArray[index] = stringBeggining + openingTag + text + closingTag + restOfString;
                    }
                    sb.AppendLine(inputArray[index]);
                }
                else
                {
                    sb.AppendLine(inputArray[index]);
                }
            }
        }

        public int ViewPost(string id)
        {
            var post = this.dbService.DbContext.Posts.Where(p => p.Id == id).FirstOrDefault();

            post.Views++;

            this.dbService.DbContext.SaveChanges();

            return post.Views;
        }

        public int GetTotalPostsCount()
        {
            int totalPostsCount = this.dbService.DbContext.Posts.Count();

            return totalPostsCount;
        }
    }
}