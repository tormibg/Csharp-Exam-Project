﻿using Forum.MapConfiguration.Contracts;
using System.ComponentModel.DataAnnotations;
using Forum.ViewModels.Interfaces.Category;
using Forum.Models.Enums;
using Forum.ViewModels.Common;

namespace Forum.ViewModels.Category
{

    public class CategoryInputModel : ICategoryInputModel, IMapTo<Models.Category>
    {
        [Required(ErrorMessage = ErrorConstants.RequiredError)]
        [StringLength(ErrorConstants.MaximumNamesLength, ErrorMessage = ErrorConstants.StringLengthErrorMessage, MinimumLength = ErrorConstants.MinimumNamesLength)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorConstants.RequiredError)]
        [EnumDataType(typeof(CategoryType), ErrorMessage = ErrorConstants.MustEnterValidValueError)]
        public CategoryType Type { get; set; }
    }
}