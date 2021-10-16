using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> PageList;

            using (Db db = new Db())
            {
                PageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            return View(PageList);
        }

        public ActionResult Addpage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Addpage(PageVM  model)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            using (Db db = new Db())
            {
                string slug;

                PageDTO dto = new PageDTO();
                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", ".").ToLower();

                }

                else
                {

                    slug = model.Slug.Replace(" ", ".").ToLower();

                }

                if(db.Pages.Any(x=>x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
               {
                    ModelState.AddModelError("", "The Title or slug already exist");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();

            }

            TempData["SM"] = "You have added a new Data";

            return RedirectToAction("Addpage");
            
        }

        public ActionResult EditPage( int Id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(Id);

                if (dto == null)
                {
                    return Content("Page not found");
                }
                model = new PageVM(dto);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (! ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                int id = model.Id;
                string slug = "home";

                PageDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                if(model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", ".").ToLower();
                    }

                    else
                    {
                        slug = model.Slug.Replace(" ", ".").ToLower();
                    }
                }

                if(db.Pages.Where(x=>x.Id!=id).Any(x=> x.Title== model.Title)||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;

                db.SaveChanges();

         
            }

            TempData["SM"] = "Your Value has been Updated";
            return RedirectToAction("EditPage");
        }

        public ActionResult PageDetails(int id)
        {

            PageVM model;

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("The page requested is not found");
                }

                model = new PageVM(dto);

            }
            return View(model);
        }

        public ActionResult DeletePage(int id)
        {

            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                db.Pages.Remove(dto);
                db.SaveChanges();
            }
            return RedirectToAction("index");
        }
        public void Reorderpages(int [] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                PageDTO dto;

                foreach(var pageid in id)
                {
                    dto = db.Pages.Find(pageid);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }

            }
        }

        public ActionResult EditSideBar()
        {
            SideBarVM model;

            using (Db db = new Db())
            {
                SideBarDTO dto = db.Sidebars.Find(1);
                model = new SideBarVM(dto);
            }
            return View(model);
        }

        [HttpPost]

        public ActionResult EditSideBar(SideBarVM model)
        {
            using (Db db = new Db())
            {
                SideBarDTO dto = db.Sidebars.Find(1);

                dto.body = model.body;
                db.SaveChanges();

            }

            TempData["SM"] = "You Information have been saved";
            return RedirectToAction("EditSideBar");
        }
    }
}