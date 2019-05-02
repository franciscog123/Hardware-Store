using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryStore.Library
{
    class Order
    {
        private Location _location;
        private Customer _customer;
        private DateTime _orderTime;
        private List<Products> _productsSold;

        public int OrderId { get; set; }

    }
}
