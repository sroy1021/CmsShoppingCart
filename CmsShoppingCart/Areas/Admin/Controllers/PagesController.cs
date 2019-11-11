using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels;
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
            //Declare list of PageVM
            List<PageVM> pages;


            //Init the List

            using (Db db = new Db())
            {
                pages = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Return view with list


            return View(pages);
        }

        //GET : Admin/Pages/AddPage
       [HttpGet]
        public ActionResult AddPage()
        {

            return View();
        }


        //POST : Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {

            //Check model state

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {


                //Declare slug
                string slug;

                //Init PageDTO
                PageDTO pageDTO = new PageDTO();
                //DTO Title
                pageDTO.Title = model.Title;
                //Check for and set slug if need
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();

                }
                //Make sure title and slug are unique

                if (db.Pages.Any(x=> x.Title == model.Title) || db.Pages.Any(x=>x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "The title or slug already exist.");
                    return View(model);
                }

                //DTO  the rest
                pageDTO.Slug = slug;
                pageDTO.Body = model.Body;
                pageDTO.HasSideBar = model.HasSideBar;
                pageDTO.Sorting = 100;
                //Save DTO

                db.Pages.Add(pageDTO);
                db.SaveChanges();

            }
            //Set Template Message
            TempData["SM"] = "You have added a new page!";

            //Redirect
            return RedirectToAction("AddPage");

           
        }



        //GET : Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage( int id = 0)
        {
            //Declare pageVM

            PageVM model;

            using (Db db = new Db())
            {

                //Get the page

                PageDTO page = db.Pages.Find(id);



                //Confirm page exists
                if (page == null)
                {
                    return Content("Page not exist");
                }
                //Init pageVM

                model = new PageVM(page);


            }
                       
            //Return view with mode
            return View(model);
        }


        //POST : Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Declare slug
                string slug = "home";
                //Get page id
                int id = model.Id;
                //Get the page
                PageDTO page = db.Pages.Find(id);
                //DTO the title
                page.Title = model.Title;
                //Check for slug if need be

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Make sure title and slug are unique

                if (db.Pages.Where(x=>x.Id != id).Any(x=>x.Title == model.Title)
                   || db.Pages.Where(x=>x.Id != id).Any(x=>x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "The title or slug already exist.");
                    return View(model);
                }

                //DTO the rest
                page.Slug = slug;
                page.Body = model.Body;
                page.HasSideBar = model.HasSideBar;

                //Save the DTO
                db.SaveChanges();
                //Set template message
                TempData["SM"] = "Page Edited successfully.";
            }


            //Redirect to view

            return RedirectToAction("EditPage");
        }

    }
}