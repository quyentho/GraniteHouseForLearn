﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }
        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductsViewModel()
            {
                Products = new Models.Products(),
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList()
            };
        }
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }
        public IActionResult Create()
        {
            return View(ProductsVM);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }
            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productFromDb = _db.Products.Find(ProductsVM.Products.Id);
            if (files.Count != 0)
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                productFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".jpg");
                productFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".jpg";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).FirstOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
                return NotFound();

            return View(ProductsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            if(ModelState.IsValid)
            {
                var webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                var productFromDb = _db.Products.FirstOrDefault(m => m.Id == id);
                if(files.Count != 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_old = Path.GetExtension(productFromDb.Image);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    if(System.IO.File.Exists(Path.Combine(uploads,productFromDb.Id+extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, productFromDb.Id + extension_old));
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    ProductsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension_new;
                }
                
                //if(ProductsVM.Products.Image != null)
                //{
                //    productFromDb.Image = ProductsVM.Products.Image;
                //}
                productFromDb.Image = ProductsVM.Products.Image;
                productFromDb.ProductName = ProductsVM.Products.ProductName;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productFromDb.SpecialTagID = ProductsVM.Products.SpecialTagID;
                productFromDb.Available = ProductsVM.Products.Available;
                productFromDb.ShadeColor = ProductsVM.Products.ShadeColor;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            }

            return View(ProductsVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).FirstOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
                return NotFound();

            return View(ProductsVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).FirstOrDefaultAsync(m => m.Id == id);

            if (ProductsVM.Products == null)
                return NotFound();

            return View(ProductsVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;
            var product = _db.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var uploads = Path.Combine(webRootPath, SD.ImageFolder);
            var extension = Path.GetExtension(product.Image);

            if(System.IO.File.Exists(Path.Combine(uploads,product.Id+extension)))
            {
                System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
    }
}