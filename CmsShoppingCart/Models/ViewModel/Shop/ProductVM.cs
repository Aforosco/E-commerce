using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.ViewModel
{
    public class ProductVM
    {
        public ProductVM()
        {

        }

        public ProductVM(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            slug = row.slug;
            Description = row.Description;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
            Price = row.Price;

        }

      
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string slug { get; set; }
        [Required]
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>Categories { get; set; }
        public IEnumerable<string> GalaryImages { get; set; }


    }
}