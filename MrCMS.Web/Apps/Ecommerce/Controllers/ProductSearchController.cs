﻿using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductSearchController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductSearchService _productSearchService;

        public ProductSearchController(ICategoryService categoryService, IProductOptionManager productOptionManager, IProductSearchService productSearchService)
        {
            _categoryService = categoryService;
            _productOptionManager = productOptionManager;
            _productSearchService = productSearchService;
        }

        public ViewResult Show(ProductSearch page, string q = null)
        {
            ViewBag.ProductOptions = _productOptionManager.GetAllAttributeOptions();
            ViewBag.ProductSpecifications = _productOptionManager.ListSpecificationAttributes();
            ViewBag.ProductPriceRangeMin = 0;
            ViewBag.ProductPriceRangeMax = 5000;
            ViewBag.Categories = _categoryService.GetAll().Where(x => x.Parent != null && x.Parent.Parent == null && x.Products.Any()).ToList();
            ViewBag.SearchTerm = q;
            return View(page);
        }

        [HttpGet]
        public PartialViewResult Results(string searchTerm,string sortBy,string options,string specifications, decimal productPriceRangeMin = 0, decimal productPriceRangeMax = 0, int pageNo = 0, int pageSize=0)
        {
            var specs = new List<string>();
            if (!String.IsNullOrWhiteSpace(specifications))
            {
                try
                {
                    string[] rawSpecs = specifications.Split(',');
                    foreach (var item in rawSpecs)
                    {
                        if(item!=String.Empty)
                            specs.Add(item);
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            var ops = new List<string>();
            if (!String.IsNullOrWhiteSpace(options))
            {
                ops.Add(options);
            }

            var products = new ProductPagedList(_productSearchService.SearchProducts(
                searchTerm,
                sortBy,
                ops, 
                specs, 
                productPriceRangeMin, 
                productPriceRangeMax, 
                pageNo == 0 ? 1 : pageNo,
                pageSize==0 ? Int32.Parse(MrCMSApplication.Get<EcommerceSettings>().CategoryProductsPerPage.Split(',').First()) : pageSize), null);

            return PartialView(products);
        }
    }
}
