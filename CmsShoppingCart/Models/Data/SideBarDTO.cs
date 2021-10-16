using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblsidebar")]
    public class SideBarDTO
    {
        [Key]
        public int id { get; set; }


        public string  body { get; set; }
    }
}