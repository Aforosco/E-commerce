using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<SideBarDTO> Sidebars { get; set; }
        public DbSet<CategoriesDTO> Categories { get; set; }
        public DbSet<ProductDTO> products { get; set; }

    }
}