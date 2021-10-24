using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.ViewModel.Shop
{
    public class CategoriesVM
    {
        public CategoriesVM()
        {

        }

        public CategoriesVM(CategoriesDTO row)
        {
            Id = row.Id;
            Slug = row.Slug;
            Name = row.Name;
            Sorting = row.Sorting;
        }
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public int Sorting { get; set; }
    }
}