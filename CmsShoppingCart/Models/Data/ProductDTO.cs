using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblProducts")]
    public class ProductDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string slug { get; set; }
        [Required]
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoriesDTO Category { get; set; }

    }
}