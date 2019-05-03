using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryStore.Library
{

    class Location
    {
        private string _name;
        private string _address;
        private int _inventoryId;

        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Address{ get; set; }
        public int InventoryId { get; set; }

        /*todo:inventory decreases when orders are accepted
        reject orders that cane be fulfilled with remaining inventory
        relationship between products sold must have some complexity
        has order history*/
    }
}
