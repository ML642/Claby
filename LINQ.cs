namespace Lab14;

public class Program
{
    /// <summary>
    /// Zadanie 1 (0.5 pkt)
    /// Znajdź osoby (w kolejności alfabetycznej według nazwiska, a następnie imienia), które nie zrobiły jeszcze żadnych zakupów.
    /// Zwróć jedynie ID, imię i nazwisko, bez ich adresu.
    /// 
    /// Wskazówka: klauzula join...into, metoda Count()
    /// </summary>
    public static IEnumerable<(uint ID, string FirstName, string LastName)> GetCustomersWithoutOrders(Database database)
    {   
        var sq = database.Customers.GroupJoin(
            database.Orders,
            left => left.CustomerId,
            right => right.CustomerId,
            (left, group) =>
            new {
                 left.CustomerId,
                 left.FirstName,
                 left.LastName,
                 count = group.Count()   
            }
        )
        .OrderBy(let => let.LastName).ThenBy(left=>left.FirstName)
        .Where(let => let.count == 0)
        .Select(ord => (ord.CustomerId,ord.FirstName,ord.LastName));
       
        
        return sq;
    }

    /// <summary>
    /// Zadanie 2 (0.5 pkt)
    /// Na podstawie daty rejestracji, wybierz te osoby, które zarejestrowały się co najmniej N dni temu i zwróć je w kolejności od najdawniejszej rejestracji.
    /// Załóż, że daty rejestracji są przechowywane w czasie uniwersalnym (UTC).
    /// Wymaganie: nie liczyć kilka razy tego samego (można użyć klauzuli let)
    /// </summary>
    public static IEnumerable<(string FirstName, string LastName, int Days)>
    GetCustomersRegisteredAtLeastNDaysAgo(Database database, int daysAgo)
{
    DateTime now = DateTime.UtcNow;

    return database.Customers
        .Select(customer => new
        {
            customer.FirstName,
            customer.LastName,
            customer.RegistrationDate,
            Days = (now - customer.RegistrationDate).Days
        })
        .Where(customer => customer.Days >= daysAgo)
        .OrderBy(customer => customer.RegistrationDate)
        .Select(customer => (
            customer.FirstName,
            customer.LastName,
            customer.Days
        ));
    }

    /// <summary>
    /// Zadanie 3 (1.0 pkt)
    /// Znajdź 10 klientów, którzy jako pierwsi założyli zamówienie.
    /// Zwróć informacje o ID klienta, dacie pierwszego zamówienia oraz liczbie dni która upłynęła pomiędzy rejestracją a pierwszym zamówieniem.
    /// </summary>
    public static IEnumerable<(uint ID, DateTime FirstOrderDate, double DaysFromRegistration)> GetCustomersFirstOrderInfo(Database database)
    {
        return database.Customers
                .GroupJoin(
                    database.Orders,
                    customer => customer.CustomerId,
                    order => order.CustomerId,
                    (customer, orders) => new { customer, orders })
                .Where(x => x.orders.Any())
                .Select(x => new
                {
                    x.customer.CustomerId,
                    x.customer.RegistrationDate,
                    FirstOrderDate = x.orders.Min(order => order.Date)
                })
                .OrderBy(x => x.FirstOrderDate)
                .Take(10)
                .Select(x => (
                    ID: x.CustomerId,
                    x.FirstOrderDate,
                    DaysFromRegistration: (x.FirstOrderDate - x.RegistrationDate).TotalDays
                ));
    }

    /// <summary>
    /// Zadanie 4 (1.0 pkt)
    /// Wyznaczyć 5 przedmiotów z największą liczbą łącznie zamówionych sztuk.
    /// Wynik zwrócić w kolejności zaczynając od tych z największą liczbą.
    /// </summary>
    public static IEnumerable<(string ProductName, long TotalCount)> GetMostOrderedProducts(Database database)
    {
        return database.Products
                .GroupJoin(
                    database.OrderProducts,
                    product => product.ProductId,
                    orderProduct => orderProduct.ProductId,
                    (product, orderProducts) => new
                    {
                        ProductName = product.Name,
                        TotalCount = orderProducts.Sum(x => (long)x.Quantity)
                    }).OrderByDescending(x=>x.TotalCount).Take(5)
                    .Select(x => (x.ProductName, x.TotalCount));
    }
            

    /// <summary>
    /// Zadanie 5 (2.0 pkt)
    /// Wyznaczyć dla każdej osoby całkowitą kwotę jaką wydała w sklepie, ilość zamówień oraz ile wydała średnio na zamówienie.
    /// Wynik zwrócić w kolejności alfabetycznej według nazwiska, a następnie imienia klienta.
    /// </summary>
    public static IEnumerable<(string FirstName, string LastName, decimal TotalExpenses, int OrderCount, decimal AvgExpenses)> GetCustomersExpenses(Database database)
    {
        return Enumerable.Empty<(string, string, decimal, int, decimal)>();
    }

    private static void Main()
    {
        Database database = Database.CreateDatabase();

        Console.WriteLine(" --==== Zadanie 1 (0.5 pkt) ====--");
        foreach (var record in GetCustomersWithoutOrders(database))
            Console.WriteLine("{0,2} {1,10} {2,10}",
                record.ID, record.FirstName, record.LastName);


        Console.WriteLine("\n --==== Zadanie 2 (0.5 pkt) ====--");
        foreach (var record in GetCustomersRegisteredAtLeastNDaysAgo(database, 500))
            Console.WriteLine("{0,10} {1,10} {2,6:0.0}",
                record.LastName, record.FirstName, record.Days);


        Console.WriteLine("\n --==== Zadanie 3 (1.0 pkt) ====--");
        foreach (var record in GetCustomersFirstOrderInfo(database))
            Console.WriteLine("{0,3} {1} {2,6:0.0}",
                record.ID, record.FirstOrderDate, record.DaysFromRegistration);


        Console.WriteLine("\n --==== Zadanie 4 (1.0 pkt) ====--");
        foreach (var record in GetMostOrderedProducts(database))
            Console.WriteLine("{0,15} {1,3}",
                record.ProductName, record.TotalCount);


        Console.WriteLine("\n --==== Zadanie 5 (2.0 pkt) ====--");
        foreach (var record in GetCustomersExpenses(database))
            Console.WriteLine("{0,10} {1,-10}: {2,2} {3,10:0.00} {4,10:0.00}",
                record.FirstName, record.LastName, record.OrderCount, record.TotalExpenses, record.AvgExpenses);
    }
}
