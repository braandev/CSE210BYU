using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Address address1 = new Address("123 Main Street", "New York", "NY", "USA");
        Customer customer1 = new Customer("Brandon Cazorla", address1);

        Address address2 = new Address("456 Maple Ave", "Toronto", "ON", "Canada");
        Customer customer2 = new Customer("Sophie Miller", address2);

        Product product1 = new Product("Wireless Mouse", "A101", 15.99, 2);
        Product product2 = new Product("Mechanical Keyboard", "A102", 49.99, 1);
        Product product3 = new Product("HD Monitor", "A103", 129.99, 1);

        Order order1 = new Order(customer1);
        order1.AddProduct(product1);
        order1.AddProduct(product2);
        order1.AddProduct(product3);

        Product product4 = new Product("USB Cable", "B201", 5.50, 3);
        Product product5 = new Product("Laptop Stand", "B202", 29.99, 1);

        Order order2 = new Order(customer2);
        order2.AddProduct(product4);
        order2.AddProduct(product5);

        Console.WriteLine("=========== ORDER 1 ===========");
        Console.WriteLine(order1.GetPackingLabel());
        Console.WriteLine(order1.GetShippingLabel());
        Console.WriteLine($"Total Price: ${order1.CalculateTotalPrice():0.00}");
        Console.WriteLine();

        Console.WriteLine("=========== ORDER 2 ===========");
        Console.WriteLine(order2.GetPackingLabel());
        Console.WriteLine(order2.GetShippingLabel());
        Console.WriteLine($"Total Price: ${order2.CalculateTotalPrice():0.00}");
    }
}
