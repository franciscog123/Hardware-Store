using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using NLog;
using HardwareStore.DataAccess.Entities;
using HardwareStore.Library.Interfaces;
using HardwareStore.Library;
using HardwareStore.DataAccess.Repositories;

namespace HardwareStore.ProjectUI
{
    public class Program
    {
        //add logging
        public static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main()
        {
            var optionsBuilder = new DbContextOptionsBuilder<HardwareStoreDbContext>();
            optionsBuilder.UseSqlServer(SecretConfiguration.ConnectionString);
            var dbContext = new HardwareStoreDbContext(optionsBuilder.Options);

            CustomerRepository customerRepository = new CustomerRepository(dbContext);
            OrderRepository orderRepository = new OrderRepository(dbContext);
            LocationRepository locationRepository = new LocationRepository(dbContext);
            ProductsRepository productsRepository = new ProductsRepository(dbContext);

            Console.WriteLine("Welcome to Francisco's Hardware Store! ");
            
            while(true)
            {
                Console.WriteLine("\no:\tPlace an order");
                Console.WriteLine("n:\tSearch for customer by name");
                Console.WriteLine("l:\tDisplay all orders by store location");
                Console.WriteLine("c:\tDisplay all orders by customer");
                Console.WriteLine("a\tView all orders that have been placed.");
                Console.WriteLine("q\tQuit");

                String input = Console.ReadLine();
                if(input=="o")
                {
                    customerRepository.DisplayCustomers();
                    Console.WriteLine("Which customer is the order for? input customer id:");
                    input = Console.ReadLine();
                    int val = 0;
                    if (Int32.TryParse(input, out val))
                    {
                        Library.Customer customer=  customerRepository.GetCustomerById(Int32.Parse(input));
                        int customerId = Int32.Parse(input);
                        Console.WriteLine("Your default store id is "+customer.DefaultStoreId+" location: "+locationRepository.GetLocationById(customer.DefaultStoreId).Name);
                        Console.WriteLine("Do you want to order from a different location? input y for yes");
                        input=Console.ReadLine();
                        if(input=="y")
                        {
                            locationRepository.DisplayLocations();
                            Console.WriteLine("input location id");
                            input = Console.ReadLine();
                            int val2 = 0;
                            if (Int32.TryParse(input, out val2))
                                {
                                    int storeLoc = Int32.Parse(input); //storeloc will be ordering from
                                    var inventory = locationRepository.GetInventoryByLocationId(storeLoc);
                                    Console.WriteLine("Printing Inventory for location id "+storeLoc);
                                    foreach(var inv in inventory)
                                    {
                                        Console.WriteLine($"ProductId: {inv.ProductId} Amount in stock: {inv.AmountInStock} Product name: {productsRepository.GetProductByProductId(inv.ProductId).ProductName} Price {productsRepository.GetProductByProductId(inv.ProductId).Price}");
                                    }
                                Console.WriteLine("Place your order.");
                                Console.WriteLine("input product id of the product you wish to buy.");
                                 input = Console.ReadLine();
                                int productId = Int32.Parse(input);
                                Console.WriteLine("input amount of product you wish to buy");
                                input = Console.ReadLine();
                                int productAmount = Int32.Parse(input);
                                decimal total = productAmount * productsRepository.GetProductByProductId(productId).Price;

                                Console.WriteLine("Placing order...");
                                Order myOrder = new Order();
                                myOrder.OrderTime = DateTime.Now;
                                myOrder.LocationId = storeLoc;
                                myOrder.CustomerId = customerId;
                                myOrder.OrderTotal = total;
                                orderRepository.AddOrder(myOrder);
                                orderRepository.Save();

                                Library.OrderItem myOrderItem = new Library.OrderItem();
                                myOrderItem.OrderId = orderRepository.GetLastOrderAdded();
                                myOrderItem.OrderItemNum = 1;
                                myOrderItem.QuantityBought = productAmount;
                                myOrderItem.ProductId = productId;
                                myOrderItem.Price = productsRepository.GetProductByProductId(productId).Price;
                                orderRepository.AddOrderItem(myOrderItem);
                                orderRepository.Save();

                                Console.WriteLine("OrderPlaced");
                                }
                            //Console.WriteLine("checking if input parsed"+input);
                                

                        }
                        else
                        {
                            Console.WriteLine("Order will be placed from default location.");
                            int storeLoc = customerRepository.GetCustomerById(customerId).DefaultStoreId;
                            var inventory = locationRepository.GetInventoryByLocationId(storeLoc);
                            Console.WriteLine("Printing Inventory for location id " + storeLoc);
                            foreach (var inv in inventory)
                            {
                                Console.WriteLine($"ProductId: {inv.ProductId} Amount in stock: {inv.AmountInStock} Product name: {productsRepository.GetProductByProductId(inv.ProductId).ProductName} Price {productsRepository.GetProductByProductId(inv.ProductId).Price}");
                            }
                            Console.WriteLine("Place your order.");
                            Console.WriteLine("input product id of the product you wish to buy.");
                            input = Console.ReadLine();
                            int productId = Int32.Parse(input);
                            Console.WriteLine("input amount of product you wish to buy");
                            input = Console.ReadLine();
                            int productAmount = Int32.Parse(input);
                            decimal total = productAmount * productsRepository.GetProductByProductId(productId).Price;

                            Console.WriteLine("Placing order...");
                            Order myOrder = new Order();
                            myOrder.OrderTime = DateTime.Now;
                            myOrder.LocationId = storeLoc;
                            myOrder.CustomerId = customerId;
                            myOrder.OrderTotal = total;
                            orderRepository.AddOrder(myOrder);
                            orderRepository.Save();

                            Library.OrderItem myOrderItem = new Library.OrderItem();
                            myOrderItem.OrderId = orderRepository.GetLastOrderAdded();
                            myOrderItem.OrderItemNum = 1;
                            myOrderItem.QuantityBought = productAmount;
                            myOrderItem.ProductId = productId;
                            myOrderItem.Price = productsRepository.GetProductByProductId(productId).Price;
                            orderRepository.AddOrderItem(myOrderItem);
                            orderRepository.Save();

                            Console.WriteLine("OrderPlaced");
                            //order from default store
                        }
                    }
                    else
                    {
                        Console.WriteLine("No matches");
                    }
                }
                if(input=="n")
                {
                    do
                    {
                        Console.WriteLine("Enter first name:");
                        input = Console.ReadLine();
                    }
                    while (input.Length == 0);

                    string input2;
                    do
                    {
                        Console.WriteLine("Enter last name: ");
                        input2 = Console.ReadLine();
                    }
                    while (input2.Length==0);
                    var customerList = customerRepository.GetCustomerByName(input, input2);
                    bool flag = customerList.Any();
                    if (!flag)
                    {
                        Console.WriteLine("no matches");
                    }
                    foreach (var c in customerList)
                    {
                        customerRepository.DisplayCustomer(c);
                    }
                }
                if(input =="l")
                {
                    bool flag = false;
                    Console.WriteLine("Do you want to see all of the details for each order? input y for yes");
                    string deets = Console.ReadLine();
                    if (deets == "y")
                        flag = true;

                    Console.WriteLine("Displaying all stores.");
                    locationRepository.DisplayLocations();

                    Console.WriteLine("Input a store id: ");
                    input=Console.ReadLine();
                    int val = 0;
                    if (Int32.TryParse(input, out val))
                    {
                        Console.WriteLine("Showing all orders for location " + input);
                        foreach (var x in locationRepository.GetOrderHistoryByLocation(Int32.Parse(input)))
                        {
                            if(flag)
                            {
                                orderRepository.DisplayOrderDetailsAll(x.OrderId);
                            }
                            else
                                orderRepository.DisplayOrderDetailsShort(x.OrderId);
                        }
                    }
                    else
                        Console.WriteLine("no matches");
                }
                if(input=="c")
                {
                    bool flag = false;
                    Console.WriteLine("Do you want to see all of the details for each order? input y for yes");
                    string deets = Console.ReadLine();
                    if (deets == "y")
                        flag = true;

                    Console.WriteLine("Displaying all customers.");
                    customerRepository.DisplayCustomers();

                    
                    Console.WriteLine("Input a customer id: ");
                    input = Console.ReadLine();
                    int val = 0;
                    if (Int32.TryParse(input, out val))
                    {
                        Console.WriteLine("Showing all orders for customer " + input);
                        foreach (var x in customerRepository.GetOrderHistoryByCustomer(Int32.Parse(input)))
                        {
                            if (!flag)
                                orderRepository.DisplayOrderDetailsShort(x.OrderId);
                            else
                                orderRepository.DisplayOrderDetailsAll(x.OrderId);
                        }
                    }
                    else
                        Console.WriteLine("no matches"); 
                }
                if(input=="a")
                {
                    Console.WriteLine("Displaying all orders placed.");
                    foreach(var o in orderRepository.GetOrders())
                    {
                        Console.WriteLine("Orders" + "id: " + o.OrderId + " " + o.OrderTime + " Total: " + o.OrderTotal + " id: " + o.LocationId + o.CustomerId);
                    }
                }
                if(input=="q")
                {
                    break;
                }
            }
        }
    }
}

/*
Console.WriteLine("getting all customers");
var CustomerList = customerRepository.GetCustomers().ToList();
foreach (var cust in CustomerList)
{
    Console.WriteLine("Id: " + cust.CustId + " Name: " + cust.FirstName + " " + cust.LastName + " phone: " + cust.PhoneNumber + " DefaultlocId: " + cust.DefaultStoreId);
}

Console.WriteLine("find customer by name bob smith");
var custo = customerRepository.GetCustomerByName("Bob","smith");
foreach(var cust in custo)
{
    Console.WriteLine("Id: "+cust.CustId+" Name: "+ cust.FirstName+" " +cust.LastName+" phone: " +cust.PhoneNumber+" DefaultlocId: "+cust.DefaultStoreId);
}

Console.WriteLine("getting all orders");
foreach (var o in orderRepository.GetOrders())
{
    Console.WriteLine("Orders"+"id: "+o.OrderId+" "+o.OrderTime +" Total: "+o.OrderTotal+" id: "+o.LocationId+ o.CustomerId);
}


Console.WriteLine("getting order history for customer id 1");
foreach (var cust in customerRepository.DisplayOrderHistoryByCustomer(1))
{
    Console.WriteLine("OrderId: "+cust.OrderId+" OrderTime: "+cust.OrderTime+" Location Id: "+cust.LocationId+ "Customer id: "+cust.CustomerId+" Order Total: "+cust.OrderTotal);
}

Console.WriteLine("getting order history by location 2");
foreach (var loc in locationRepository.GetOrderHistoryByLocation(2))
{
    Console.WriteLine("Order ID: "+loc.OrderId+"Order time: "+loc.OrderTime+" Location Id: "+loc.LocationId+ "Customer id: "+loc.CustomerId+" Order Total: "+loc.OrderTotal);
}

Console.WriteLine("Printing all products");
foreach(var p in productsRepository.GetAllProducts())
{
    Console.WriteLine($"Product id: {p.ProductId} Product name: {p.ProductName} Description: {p.Description} Price {p.Price}");
}

Library.Products prod = productsRepository.GetProductByProductId(2);
Console.WriteLine("getting product by prod id 2");
Console.WriteLine($"Product id: {prod.ProductId} Name: {prod.ProductName} Description {prod.Description} Price: {prod.Price}");

Console.WriteLine("Getting all orderItems");
IEnumerable<Library.OrderItem> items = orderRepository.GetAllItems();
foreach(var x in items)
{
    Console.WriteLine($"Order Id {x.OrderId} OrderItemNum: {x.OrderItemNum} quantity bought {x.QuantityBought}");
}

Console.WriteLine("Getting items for order id 5");
foreach(var x in orderRepository.GetItemsByOrderId(5))
{
    Console.WriteLine($"Order Id {x.OrderId} OrderItemNum: {x.OrderItemNum} quantity bought {x.QuantityBought}");
}

Console.WriteLine("Getting items for order id 5 using class method");
orderRepository.DisplayOrderDetailsAll(5);
*/
/*
Order order = new Order();
order.OrderTime = DateTime.Now;
order.LocationId = 3;
order.CustomerId = 1;
order.OrderTotal = 60;

Library.OrderItem item = new Library.OrderItem();
item.OrderId = 7;
item.ProductId = 5;
item.OrderItemNum = 1;
item.QuantityBought = 12;
item.Price = 5.00m;


Console.WriteLine("adding order");
orderRepository.AddOrder(order);
orderRepository.AddOrderItem(item);
Console.WriteLine("added order not saved");
orderRepository.Save();
orderRepository.Save();
Console.WriteLine("added order and saved");*/




