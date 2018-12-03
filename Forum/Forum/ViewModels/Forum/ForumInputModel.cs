﻿namespace Forum.Web.ViewModels.Forum
{
    using global::Forum.MapConfiguration.Contracts;
    using global::Forum.Models;
    using System.ComponentModel.DataAnnotations;

    public class ForumInputModel : IMapFrom<SubForum>
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z_\-0-9]*$", ErrorMessage = "{0} is allowed to contain only lowercase/uppercase characters, digits and '_', '-'")]
        [StringLength(50, ErrorMessage = "{0} length must be between {1} and {2} characters.", MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z _\/\-0-9!.?()&]*$", ErrorMessage = "{0} is allowed to contain only lowercase/uppercase characters, digits and '_', '-', '(', ')', '&', '.', '/', '?', '!'")]
        [StringLength(500, ErrorMessage = "{0} length must be between {1} and {2} characters.", MinimumLength = 5)]
        public string Description { get; set; }
        
        //TODO: should have Id maybe
        public string Category { get; set; }
    }
}