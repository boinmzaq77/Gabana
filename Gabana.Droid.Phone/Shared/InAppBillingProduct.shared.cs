﻿
using System;

namespace Plugin.InAppBilling
{

    /// <summary>
    /// Product being offered
    /// </summary>
    [Preserve(AllMembers = true)]
    public class InAppBillingProduct
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Trial period, specified in ISO 8601 format.
        /// </summary>
        public string FreeTrialPeriod { get; set; }

        /// <summary>
        /// Icon of the product if present
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Product ID or sku
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Localized Price (not including tax)
        /// </summary>
        public string LocalizedPrice { get; set; }

        /// <summary>
        /// ISO 4217 currency code for price. For example, if price is specified in British pounds sterling is "GBP".
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Price in micro-units, where 1,000,000 micro-units equal one unit of the 
        /// currency. For example, if price is "€7.99", price_amount_micros is "7990000". 
        /// This value represents the localized, rounded price for a particular currency.
        /// </summary>
        public Int64 MicrosPrice { get; set; }

        /// <summary>
        /// Gets or sets the localized introductory price.
        /// </summary>
        /// <value>The localized introductory price.</value>
        public string LocalizedIntroductoryPrice { get; set; }

        /// <summary>
        /// Number of subscription billing periods for which the user will be given the introductory price, such as 3
        /// </summary>
        public int IntroductoryPriceCycles { get; set; }

        /// <summary>
        /// Billing period of the introductory price, specified in ISO 8601 format
        /// </summary>
        public string IntroductoryPricePeriod { get; set; }

        /// <summary>
        /// Introductory price of the product in micor-units
        /// </summary>
        /// <value>The introductory price.</value>
        public Int64 MicrosIntroductoryPrice { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Plugin.InAppBillingProduct"/>
        /// has introductory price. This is an optional value in the answer from the server, requires a boolean to check if this exists
        /// </summary>
        /// <value><c>true</c> if has introductory price; otherwise, <c>false</c>.</value>
        public bool HasIntroductoryPrice => !string.IsNullOrEmpty(LocalizedIntroductoryPrice);


        /// <summary>
        /// Formatted original price of the item, including its currency sign.
        /// </summary>
        public string OriginalPrice { get; set; }

        /// <summary>
        /// Orginal price in micro-units, where 1,000,000, micro-units equal one unit of the currency
        /// </summary>
        public long MicrosOriginalPriceAmount { get; set; }

    }

}