﻿using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class CatalogSettingsModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductSku")]
        public bool ShowProductSku { get; set; }
        public bool ShowProductSku_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowManufacturerPartNumber")]
        public bool ShowManufacturerPartNumber { get; set; }
        public bool ShowManufacturerPartNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowGtin")]
        public bool ShowGtin { get; set; }
        public bool ShowGtin_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowProductSorting")]
        public bool AllowProductSorting { get; set; }
        public bool AllowProductSorting_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowProductViewModeChanging")]
        public bool AllowProductViewModeChanging { get; set; }
        public bool AllowProductViewModeChanging_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductsFromSubcategories")]
        public bool ShowProductsFromSubcategories { get; set; }
        public bool ShowProductsFromSubcategories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryProductNumber")]
        public bool ShowCategoryProductNumber { get; set; }
        public bool ShowCategoryProductNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowCategoryProductNumberIncludingSubcategories")]
        public bool ShowCategoryProductNumberIncludingSubcategories { get; set; }
        public bool ShowCategoryProductNumberIncludingSubcategories_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CategoryBreadcrumbEnabled")]
        public bool CategoryBreadcrumbEnabled { get; set; }
        public bool CategoryBreadcrumbEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowShareButton")]
        public bool ShowShareButton { get; set; }
        public bool ShowShareButton_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductReviewsMustBeApproved")]
        public bool ProductReviewsMustBeApproved { get; set; }
        public bool ProductReviewsMustBeApproved_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToReviewProduct")]
        public bool AllowAnonymousUsersToReviewProduct { get; set; }
        public bool AllowAnonymousUsersToReviewProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NotifyStoreOwnerAboutNewProductReviews")]
        public bool NotifyStoreOwnerAboutNewProductReviews { get; set; }
        public bool NotifyStoreOwnerAboutNewProductReviews_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EmailAFriendEnabled")]
        public bool EmailAFriendEnabled { get; set; }
        public bool EmailAFriendEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.AllowAnonymousUsersToEmailAFriend")]
        public bool AllowAnonymousUsersToEmailAFriend { get; set; }
        public bool AllowAnonymousUsersToEmailAFriend_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedProductsNumber")]
        public int RecentlyViewedProductsNumber { get; set; }
        public bool RecentlyViewedProductsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyViewedProductsEnabled")]
        public bool RecentlyViewedProductsEnabled { get; set; }
        public bool RecentlyViewedProductsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyAddedProductsNumber")]
        public int RecentlyAddedProductsNumber { get; set; }
        public bool RecentlyAddedProductsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.RecentlyAddedProductsEnabled")]
        public bool RecentlyAddedProductsEnabled { get; set; }
        public bool RecentlyAddedProductsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.CompareProductsEnabled")]
        public bool CompareProductsEnabled { get; set; }
        public bool CompareProductsEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowBestsellersOnHomepage")]
        public bool ShowBestsellersOnHomepage { get; set; }
        public bool ShowBestsellersOnHomepage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfBestsellersOnHomepage")]
        public int NumberOfBestsellersOnHomepage { get; set; }
        public bool NumberOfBestsellersOnHomepage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.SearchPageProductsPerPage")]
        public int SearchPageProductsPerPage { get; set; }
        public bool SearchPageProductsPerPage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteEnabled")]
        public bool ProductSearchAutoCompleteEnabled { get; set; }
        public bool ProductSearchAutoCompleteEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductSearchAutoCompleteNumberOfProducts")]
        public int ProductSearchAutoCompleteNumberOfProducts { get; set; }
        public bool ProductSearchAutoCompleteNumberOfProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ShowProductImagesInSearchAutoComplete")]
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public bool ShowProductImagesInSearchAutoComplete_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsAlsoPurchasedEnabled")]
        public bool ProductsAlsoPurchasedEnabled { get; set; }
        public bool ProductsAlsoPurchasedEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsAlsoPurchasedNumber")]
        public int ProductsAlsoPurchasedNumber { get; set; }
        public bool ProductsAlsoPurchasedNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.EnableDynamicPriceUpdate")]
        public bool EnableDynamicPriceUpdate { get; set; }
        public bool EnableDynamicPriceUpdate_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.NumberOfProductTags")]
        public int NumberOfProductTags { get; set; }
        public bool NumberOfProductTags_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPageSize")]
        public int ProductsByTagPageSize { get; set; }
        public bool ProductsByTagPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagAllowCustomersToSelectPageSize")]
        public bool ProductsByTagAllowCustomersToSelectPageSize { get; set; }
        public bool ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ProductsByTagPageSizeOptions")]
        public string ProductsByTagPageSizeOptions { get; set; }
        public bool ProductsByTagPageSizeOptions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeShortDescriptionInCompareProducts")]
        public bool IncludeShortDescriptionInCompareProducts { get; set; }
        public bool IncludeShortDescriptionInCompareProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IncludeFullDescriptionInCompareProducts")]
        public bool IncludeFullDescriptionInCompareProducts { get; set; }
        public bool IncludeFullDescriptionInCompareProducts_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreDiscounts")]
        public bool IgnoreDiscounts { get; set; }
        public bool IgnoreDiscounts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.IgnoreFeaturedProducts")]
        public bool IgnoreFeaturedProducts { get; set; }
        public bool IgnoreFeaturedProducts_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Catalog.ManufacturersBlockItemsToDisplay")]
        public int ManufacturersBlockItemsToDisplay { get; set; }
        public bool ManufacturersBlockItemsToDisplay_OverrideForStore { get; set; }
    }
}