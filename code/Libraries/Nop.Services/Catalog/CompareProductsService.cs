using System;
using System.Collections.Generic;
using System.Web;
using Nop.Core.Domain.Catalog;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Compare products service
    /// </summary>
    public partial class CompareProductsService : ICompareProductsService
    {
        #region Constants

        /// <summary>
        /// Compare products cookie name
        /// </summary>
        private const string COMPARE_PRODUCTS_COOKIE_NAME = "moderncart.cmp";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="productService">Product service</param>
        /// <param name="catalogSettings">Catalog settings</param>
        public CompareProductsService(HttpContextBase httpContext, IProductService productService,
            ICategoryService _categoryService, CatalogSettings catalogSettings)
        {
            this._httpContext = httpContext;
            this._productService = productService;
            this._categoryService = _categoryService;
            this._catalogSettings = catalogSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a "compare products" identifier list
        /// </summary>
        /// <returns>"compare products" identifier list</returns>
        protected virtual List<int> GetComparedProductIds()
        {
            var productIds = new List<int>();
            HttpCookie compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if ((compareCookie == null) || (compareCookie.Values == null))
                return productIds;
            string[] values = compareCookie.Values.GetValues("cmpids");
            if (values == null)
                return productIds;
            foreach (string productId in values)
            {
                int prodId = int.Parse(productId);
                if (!productIds.Contains(prodId))
                    productIds.Add(prodId);
            }

            return productIds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public virtual void ClearCompareProducts()
        {
            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (compareCookie != null)
            {
                compareCookie.Values.Clear();
                compareCookie.Expires = DateTime.Now.AddYears(-1);
                _httpContext.Response.Cookies.Set(compareCookie);
            }
        }

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        public virtual IList<Product> GetComparedProducts()
        {
            var products = new List<Product>();
            var productIds = GetComparedProductIds();
            foreach (int productId in productIds)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.Published && !product.Deleted)
                    products.Add(product);
            }
            return products;
        }

        public virtual string GenerateCompare(IList<Product> products)
        {
            if (products.Count > 0)
            {
                var productCategories = _categoryService.GetProductCategoriesByProductId(products[0].Id);
                string template = string.Empty;
                if (productCategories.Count > 0)
                    template = _categoryService.GetCategoryById(productCategories[0].CategoryId).HtmlCompare;

                if (string.IsNullOrEmpty(template))
                    return string.Empty;
                string regRow = @"(\[row([^\]])*\])";
                MatchCollection mcRow = Regex.Matches(template, regRow);
                if (mcRow.Count == 0)
                    return string.Empty;
                StringBuilder sb = new StringBuilder();
                
                var flag = false;

                // Table content
                int i = 0;
                string regParam = @" (?<name>[^=]+)=""(?<val>[^""]+)";
                foreach (var item in mcRow)
                {
                    sb.Append("<tr class=\"short-description\">");
                    MatchCollection mcRowParam = Regex.Matches(item.ToString(), regParam);
                    var tmp2 = GenerateCompareTableRow(products, mcRowParam, i);
                    if (!string.IsNullOrEmpty(tmp2))
                    {
                        sb.Append(tmp2);
                        flag = true;
                    }
                    i++;
                    sb.Append("</tr>");
                }

                if (!flag)
                    return string.Empty;
                return sb.ToString();
            }
            else
                return string.Empty;
        }

        private string GenerateCompareTableRow(IList<Product> products, MatchCollection param, int introw)
        {
            try
            {
                if (param == null || param.Count == 0)
                    return string.Empty;
                string title = param[0].Groups["val"].ToString();
                string propIds = param[1].Groups["val"].ToString();

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(propIds))
                    return string.Empty;

                var arrPropIds = propIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrPropIds == null || arrPropIds.Length == 0)
                    return string.Empty;

                string strclass = introw % 2 == 0 ? " bg_gray" : "";
                StringBuilder sb = new StringBuilder();
                sb.Append("<td class=\"border-right" + strclass +"\">");
                sb.Append(title);
                sb.Append("</td>");
                int i = 0;
                foreach (var product in products)
                {
                    sb.Append("<td class=\"border-left" + strclass + "\">");
                    var tmp = _productService.GetProductDetailByProductId(product.Id);
                    if (tmp != null && tmp.Count > 0)
                    {
                        var props = (from propId in arrPropIds
                                     join prop in tmp
                                     on Convert.ToInt32(propId) equals prop.PropertyId
                                     select new { prop }
                              ).ToList();

                        var propValue = new List<string>();
                        foreach (var p in props)
                        {
                            if(!string.IsNullOrEmpty(p.prop.Value))
                            {
                                if (p.prop.PropertyType == 0)
                                    propValue.Add(_productService.GetProductPropertyValue(Convert.ToInt32(p.prop.Value)).Name);
                                else if (p.prop.PropertyType == 1)
                                {
                                    string[] lstProp = p.prop.Value.Split(',');
                                    foreach (var str in lstProp)
                                        propValue.Add(_productService.GetProductPropertyValue(Convert.ToInt32(str)).Name);
                                }
                                else
                                    propValue.Add(p.prop.Value);
                            }
                        }
                        string value = string.Join(", ", propValue.ToArray());
                        sb.Append(value);
                    }
                    else
                        sb.Append("Không có");
                    i++;
                    sb.Append("</td>");
                }
                return sb.ToString();
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void RemoveProductFromCompareList(int productId)
        {
            var oldProductIds = GetComparedProductIds();
            var newProductIds = new List<int>();
            newProductIds.AddRange(oldProductIds);
            newProductIds.Remove(productId);

            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if ((compareCookie == null) || (compareCookie.Values == null))
                return;
            compareCookie.Values.Clear();
            foreach (int newProductId in newProductIds)
                compareCookie.Values.Add("cmpids", newProductId.ToString());
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(compareCookie);
        }

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        public virtual void AddProductToCompareList(int productId)
        {
            var oldProductIds = GetComparedProductIds();
            var newProductIds = new List<int>();
            newProductIds.Add(productId);
            foreach (int oldProductId in oldProductIds)
                if (oldProductId != productId)
                    newProductIds.Add(oldProductId);

            var compareCookie = _httpContext.Request.Cookies.Get(COMPARE_PRODUCTS_COOKIE_NAME);
            if (compareCookie == null)
            {
                compareCookie = new HttpCookie(COMPARE_PRODUCTS_COOKIE_NAME);
                compareCookie.HttpOnly = true;
            }
            compareCookie.Values.Clear();
            int i = 1;
            foreach (int newProductId in newProductIds)
            {
                compareCookie.Values.Add("cmpids", newProductId.ToString());
                if (i == _catalogSettings.CompareProductsNumber)
                    break;
                i++;
            }
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            _httpContext.Response.Cookies.Set(compareCookie);
        }

        #endregion
    }
}
