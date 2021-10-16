using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.ViewModel.Pages
{
    public class SideBarVM
    {
       
            public SideBarVM()
            {

            }

            public SideBarVM(SideBarDTO row)
            {
                id = row.id;
                body = row.body;
            }
            [Key]
            public int id { get; set; }

                [AllowHtml]
            public string body { get; set; }
        

    }
}