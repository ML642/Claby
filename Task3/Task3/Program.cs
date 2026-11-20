using System.Collections.Specialized;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Task3;

public class Program {
    public static void Main(string[] args) {
        Database database = Database.GetInstance();

        // 1. List people (in alphabetical order by surname, then first name) who do not have a loyalty card.
        var query = database.Customers.
        Where(customer => !database.LoyaltyCards.Any(card=>card.CustomerId == customer.CustomerId))
        .OrderBy((customer)=>customer.LastName).ThenBy((customer)=>customer.FirstName);
        
        
        // Hints: join...into clause, Count() method
        Console.WriteLine("****** Question 1 ******");
        Console.WriteLine("Provided:");

        /* your code here */
        foreach ( var e in query)
                    {
                        Console.WriteLine(e);
                    }
        Console.WriteLine();
        Console.WriteLine("Expected:");
        Console.WriteLine("Bąk Magdalena");
        Console.WriteLine("Bąk Marek");
        Console.WriteLine("Wiśniewski Artur");

        // 2. Based on the PESEL number, select people who are more than 29 years old, by birth year. Display these people from oldest to youngest.
        // Requirement: do not count the same person multiple times (you may use the let clause)
        // Hint: A PESEL number from 2000 onwards differs in that 20 is added to the month value
        


        var query2 = database.Customers
                .Select(cust => new
                {
                    Customer = cust,
                    BirthYear =
                        (int.Parse(cust.PESEL.Substring(2, 2)) >= 21
                            ? 2000
                            : 1900)
                        + int.Parse(cust.PESEL.Substring(0, 2))
                })
                .Where(x => x.BirthYear <= 1996)
                .OrderBy(x => x.BirthYear)
                .Select(x => x.Customer);
        foreach(var i in query2)
        {
            Console.WriteLine(i);
        }

        Console.WriteLine();
        Console.WriteLine("****** Question 2 ******");
        Console.WriteLine("Provided:");

        var query3 = database.Customers.GroupJoin(
            database.Orders,
            (cust)=>cust.CustomerId,
            (order)=>order.CustomerId,
            (cust, order) => new
            {
                Customer = cust,
                Order = order,
            }).Select(x=>{
                DateTime min = DateTime.MaxValue;
                foreach(var Ae in x.Order)
                {
                    if(Ae.Date < min)
                    {
                        min = Ae.Date;
                    }
                }
                return new{ x.Customer, min
                
            };}).OrderBy(x=>x.min).Take(3);

        Console.WriteLine();
        Console.WriteLine("Expected:");
        Console.WriteLine("Kowalski Jan 117");
        Console.WriteLine("Marciniak Jerzy 35");
        Console.WriteLine("Antoniewski Michał 32");

        // 3. Determine the 3 people who have been using the store's services the longest.

        var query4 = database.Products.GroupJoin(
            database.OrderProducts,
            product => product.ProductId,
            order => order.ProductId,
            (product, order) =>
            {
                return new
                {
                  prod_ = product,
                  order_ = order  
                };
            }
        ).Select(x =>
        {
            var i = x.order_.Sum(o=>o.Quantity);
            return new
            {
                prod_x = x.prod_,
                i
            };
        }).OrderByDescending(x=>x.i).Take(5);
        Console.WriteLine();
        Console.WriteLine("****** Question 3 ******");
        Console.WriteLine("Provided:");

        /* your code here */

        Console.WriteLine();
        Console.WriteLine("Expected:");
        Console.WriteLine("Nowak Andrzej 02.01.2017");
        Console.WriteLine("Antoniewski Michał 22.03.2017");
        Console.WriteLine("Kowalska Aneta 12.03.2018");

        // 4. Determine the 5 items with the highest total number of ordered units.
        Console.WriteLine();
        Console.WriteLine("****** Question 4 ******");
        Console.WriteLine("Provided:");

        /* your code here */

        Console.WriteLine();
        Console.WriteLine("Expected:");
        Console.WriteLine("Gaming chair 21");
        Console.WriteLine("Marker 11");
        Console.WriteLine("CD 10");
        Console.WriteLine("Pen 10");
        Console.WriteLine("Monitor 5");

        // 5. For each person, determine the total amount they spent in the store (taking into account the discount available to people with a loyalty card).
        // List the customers in alphabetical order by surname, then first name.
        Console.WriteLine();
        Console.WriteLine("****** Question 5 ******");
        Console.WriteLine("Provided:");
    
        var subquery = database.OrderProducts.Join(
            database.Products,
            order => order.ProductId,
            prod => prod.ProductId,
            (order, prod) =>
            new {
                Order = order,
                price = order.Quantity * prod.Price
            }
        );
        var subquery2 = database.Orders.Join(
            subquery,
            x=>x.OrderId,
            ord => ord.Order.OrderId,
            (x, ord) =>
            new {
              A = x.CustomerId,
              B = ord.price  
            }
        );

        var query5 = database.Customers.GroupJoin(
            subquery2,
            cust=>cust.CustomerId,
            prod=>prod.A,
            (cust, prod) =>
            new {
                Cust = cust,
                Prod = prod
            }
        ).Select(x => new
        {
            A = x.Cust,
            B = x.Prod.Sum(y=>y.B)
        }).Select(x =>
        {
            var a = database.LoyaltyCards.Find(y=>y.CustomerId == x.A.CustomerId);
            
            if(a != null)
            {
               return new {A =  x.A, B =x.B * ( 1 - a.Discount) };    
            }
            else
            {
                return new {A =  x.A, B =x.B  };    
            }
            
        });

        Console.WriteLine();
        Console.WriteLine("Expected:");
        Console.WriteLine("Antoniewski Michał 1053");
        Console.WriteLine("Bąk Magdalena 2912");
        Console.WriteLine("Bąk Marek 3000");
        Console.WriteLine("Czerwińska Justyna 2593,5");
        Console.WriteLine("Kowalska Aneta 212,5");
        Console.WriteLine("Kowalski Jan 3654");
        Console.WriteLine("Marciniak Jerzy 13600");
        Console.WriteLine("Nowak Alina 1400");
        Console.WriteLine("Nowak Andrzej 2550");
        Console.WriteLine("Wiśniewski Artur 2025");

        Console.WriteLine();
    }
}
