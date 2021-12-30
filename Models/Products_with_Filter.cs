using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCommerceWebsite_1912C1.Models
{
    public class Products_with_Filter
    {
        public PagedList.IPagedList<Product> productList { get; set; }
    }
}