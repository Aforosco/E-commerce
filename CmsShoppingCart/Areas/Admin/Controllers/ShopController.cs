using CmsShoppingCart.Models.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModel;
using System.IO;
using System.Web.Helpers;
using PagedList;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            List<CategoriesVM> CategoriesList;

            using (Db db = new Db())
            {
                CategoriesList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoriesVM(x)).ToList();

            }
            return View(CategoriesList);
        }
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }

                CategoriesDTO dto = new CategoriesDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", ".").ToLower();
                dto.Sorting = 100;
                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString();
                return id;
            }
    
        }

        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;

                CategoriesDTO dto;

                foreach (var catid in id)
                {
                    dto = db.Categories.Find(catid);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }

            }
        }

        public ActionResult DeleteCategory(int id)
        {

            using (Db db = new Db())
            {
                CategoriesDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);
                db.SaveChanges();
            }
            return RedirectToAction("categories");
        }

        [HttpPost]
        public string RenameCategory( string newCatName, int id)
        {
            using (Db db = new Models.Data.Db())
            {
               if(db.Categories.Any(x=>x.Name== newCatName))
                {
                    return "titletaken";
                }

                CategoriesDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Id = id;
                dto.Slug = newCatName.Replace(" ", ".").ToLower();
                db.SaveChanges();

                return "ok";
            }
        }
        public ActionResult AddProduct()
        {
            var model = new ProductVM();
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                    return View(model);
                }

            }
            using (Db db = new Db())
            {
                if(db.products.Any(x=>x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The Product Name already exist");
                    return View(model);
                }
            }
            int id;

            using (Db db = new Db())
            {
                ProductDTO products = new ProductDTO();
                products.Name = model.Name;
                products.Price = model.Price;
                products.slug = model.Name.Replace(" ", ".").ToLower();
                products.CategoryId = model.CategoryId;
                products.Description = model.Description;

                CategoriesDTO CatDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);

                products.CategoryName = CatDto.Name;

                db.products.Add(products);
                db.SaveChanges();
                id = products.Id;  

            }

            TempData["SM"] = "You have Just Added a product!";

            var OriginalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads",Server.MapPath(@"\")));
            var PathString1 = Path.Combine(OriginalDirectory.ToString(), "Products");
            var PathString2 = Path.Combine(OriginalDirectory.ToString(), "Products\\", id.ToString());
            var PathString3 = Path.Combine(OriginalDirectory.ToString(), "Products\\", id.ToString()+"\\Thumbs");
            var PathString4 = Path.Combine(OriginalDirectory.ToString(), "Products\\", id.ToString() + "\\Gallery");
            var PathString5 = Path.Combine(OriginalDirectory.ToString(), "Products\\", id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(PathString1))
            {
                Directory.CreateDirectory(PathString1);
            }

            if (!Directory.Exists(PathString2))
            {
                Directory.CreateDirectory(PathString2);
            }

            if (!Directory.Exists(PathString3))
            {
                Directory.CreateDirectory(PathString3);
            }
            if (!Directory.Exists(PathString4))
            {
                Directory.CreateDirectory(PathString4);
            }
            if (!Directory.Exists(PathString5))
            {
                Directory.CreateDirectory(PathString5);
            }

            if( file!=null && file.ContentLength > 2)
            {
                string ext = file.ContentType.ToLower();

                if(ext != "image/jpg"&& 
                    ext != "image/jpeg" && 
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded wrong image format use jpg or png only");
                        return View(model);
                    }
                  
                }
                string imageName = file.FileName;

                using(Db db = new Db())
                {
                    ProductDTO dto = db.products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                var path = string.Format("{0}\\{1}", PathString2, imageName);
                var path2 = string.Format("{0}\\{1}", PathString3, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            return RedirectToAction("Addproduct");
            
        }
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> listOfProductVM;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                listOfProductVM = db.products.ToArray().Where(x => catId == null || catId == 0
                || x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.Selected = catId.ToString();
                var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            return View(listOfProductVM);
        }

        public ActionResult EditProduct(int id)
        {
            ProductVM model;
            using (Db db = new Db())
            {
                ProductDTO dto = db.products.Find(id);
                if (dto == null)
                {
                    return Content("Product not found");
                }

                model = new ProductVM(dto);
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.GalaryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                    .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Get product Id

            int Id = model.Id;

            //Populate Categories Select List and Gallery Images

            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
               
            }
            model.GalaryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + Id + "/Gallery/Thumbs"))
                   .Select(fn => Path.GetFileName(fn));

            // Check Model State

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Make sure the product name is unique
            using (Db db = new Db())
            {
                if(db.products.Where(x=>x.Id != Id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "The product Name already exist");
                    return View(model);
                }
            }

            // Update Product
            using (Db db = new Db())
            {
                ProductDTO dto = db.products.Find(Id);

                dto.Name = model.Name;
                dto.slug = model.Name.Replace(" ", ".");
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;
                CategoriesDTO Catdto = db.Categories.FirstOrDefault(x => x.Id.Equals(model.CategoryId));
                dto.CategoryName = Catdto.Name;
                db.SaveChanges();
            }

            //Set Temp Data Message

            TempData["SM"] = "You have succcessfully edited this product";
            #region ImageUpload

            // check for file upload

            if (file != null && file.ContentLength > 2)
            {

                //Get extension
                string ext = file.ContentType.ToLower();
                //Verify extension

                if (ext != "image/jpg" &&
                       ext != "image/jpeg" &&
                       ext != "image/pjpeg" &&
                       ext != "image/gif" &&
                       ext != "image/x-png" &&
                       ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded wrong image format use jpg or png only");
                        return View(model);
                    }

                }

                // set upload directory path
                var OriginalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                var PathString1 = Path.Combine(OriginalDirectory.ToString(), "Products\\", Id.ToString());
                var PathString2 = Path.Combine(OriginalDirectory.ToString(), "Products\\", Id.ToString() + "\\Thumbs");


                //delete file from directory

                DirectoryInfo di1 = new DirectoryInfo(PathString1);
                DirectoryInfo di2 = new DirectoryInfo(PathString2);

                foreach(FileInfo file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (FileInfo file3 in di2.GetFiles())
                {
                    file3.Delete();
                }
                //save image name

                string ImageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO Dto = db.products.Find(Id);
                    Dto.ImageName = ImageName;
                    db.SaveChanges();
                }

                // save Original and thumb images
                var path = string.Format("{0}\\{1}", PathString1, ImageName);
                var path2 = string.Format("{0}\\{1}", PathString2, ImageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }
            #endregion

            //Redirect

            return RedirectToAction("EditProduct");
        }

        public ActionResult DeleteProduct(int id)
        {

            // Delete Product from DB
            using (Db db = new Db())
            {
                ProductDTO dto = db.products.Find(id);
                db.products.Remove(dto);
                db.SaveChanges();
            }

            //Delete Product Folder


            var OriginalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var PathString = Path.Combine(OriginalDirectory.ToString(), "Products\\", id.ToString());
            if (!Directory.Exists(PathString))
            {
                Directory.Delete(PathString,true);
            }

            //Redirect 
            return RedirectToAction("Products");
        }

        [HttpPost]
        public void GalleryImages (int Id)
        {
            //loop throgh the files
            foreach(string filename in Request.Files)
            {
                //init the file

                HttpPostedFileBase file = Request.Files[filename];

                //check if it is not null
                if (file != null && file.ContentLength > 0)
                {

                    //set directory path
                    var OriginalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                    var PathString1 = Path.Combine(OriginalDirectory.ToString(), "Products\\", Id.ToString()+"\\Gallery");
                    var PathString2 = Path.Combine(OriginalDirectory.ToString(), "Products\\", Id.ToString() + "\\Gallery\\Thumbs");


                    //set image path
                    var path = string.Format("{0}\\{1}", PathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", PathString2, file.FileName);

                    //save original and thumb
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);

                }

            }

        }

        [HttpPost]
        public void DeleteImage(int Id, string imageName)
        {
            string fullpath1 = Request.MapPath("~/Images/Uploads/Products/"+ Id.ToString()+ "/Gallery/"+ imageName);
            string fullpath2 = Request.MapPath("~/Images/Uploads/Products/" + Id.ToString() + "/Gallery/Thumbs/" + imageName);
            if (System.IO.File.Exists(fullpath1))
                System.IO.File.Delete(fullpath1);
            if (System.IO.File.Exists(fullpath2))
                System.IO.File.Delete(fullpath2);

        }
    }
}