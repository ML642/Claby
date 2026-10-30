# C# — Generyki, kolekcje, porównywanie i enumeratory

## 1. Generyki

Generyki pozwalają pisać kod działający dla różnych typów bez używania `object` i rzutowania.

```csharp
public class Box<T>
{
    public T Value;

    public Box(T value)
    {
        Value = value;
    }
}
```

Użycie:

```csharp
var a = new Box<int>(10);
var b = new Box<string>("hello");
```

---

# 2. Ograniczenia `where`

Ograniczenia określają, jakie typy mogą zostać podstawione za parametr generyczny.

## `where T : class`

`T` musi być typem referencyjnym.

```csharp
class Box<T> where T : class
{
}
```

Poprawne:

```csharp
Box<string>
Box<Student>
```

Niepoprawne:

```csharp
Box<int>
```

---

## `where T : class?`

`T` musi być typem referencyjnym, ale może być nullable.

```csharp
where T : class?
```

---

## `where T : struct`

`T` musi być typem wartościowym.

```csharp
class Box<T> where T : struct
{
}
```

Poprawne:

```csharp
Box<int>
Box<double>
Box<DateTime>
```

Niepoprawne:

```csharp
Box<string>
```

---

## `where T : notnull`

`T` nie może być `null`.

Często używane dla kluczy:

```csharp
class Registry<TKey, TValue>
    where TKey : notnull
{
}
```

---

## `where T : unmanaged`

`T` musi być typem wartościowym zawierającym tylko dane typu unmanaged.

Przykłady:

```csharp
int
double
char
bool
```

Przydatne głównie przy kodzie niskopoziomowym.

---

## `where T : new()`

`T` musi mieć publiczny konstruktor bezargumentowy.

```csharp
public T Create<T>()
    where T : new()
{
    return new T();
}
```

Bez `new()` nie można bezpiecznie zrobić:

```csharp
new T();
```

---

## `where T : BaseClass`

`T` musi być klasą `BaseClass` albo po niej dziedziczyć.

```csharp
class Repository<T>
    where T : Entity
{
}
```

---

## `where T : IInterface`

`T` musi implementować podany interfejs.

```csharp
where T : IComparable<T>
```

Dzięki temu wiadomo, że można wykonać:

```csharp
a.CompareTo(b);
```

---

## `where T : U`

`T` musi być zgodny z innym parametrem typu `U`.

```csharp
class Example<T, U>
    where T : U
{
}
```

---

## Łączenie ograniczeń

```csharp
class Repository<T>
    where T : class, IEntity, new()
{
}
```

`T`:

- musi być klasą,
- musi implementować `IEntity`,
- musi mieć konstruktor bezargumentowy.

---

# 3. `IEquatable<T>`

Służy do sprawdzania równości obiektów.

```csharp
public class Student : IEquatable<Student>
{
    public int Id { get; set; }

    public bool Equals(Student? other)
    {
        return other != null && Id == other.Id;
    }
}
```

W generykach:

```csharp
where TKey : notnull, IEquatable<TKey>
```

Wtedy można:

```csharp
key1.Equals(key2);
```

`IEquatable<T>` nie gwarantuje działania operatora:

```csharp
==
```

---

# 4. `IComparable<T>`

Obiekt sam definiuje swój domyślny porządek.

```csharp
public class Student : IComparable<Student>
{
    public string Name { get; set; }
    public double GPA { get; set; }

    public int CompareTo(Student? other)
    {
        return GPA.CompareTo(other!.GPA);
    }
}
```

Teraz:

```csharp
students.Sort();
```

może sortować studentów według GPA.

## Znaczenie wyniku `CompareTo`

```text
< 0    this jest przed other
= 0    elementy są równe w kontekście sortowania
> 0    this jest po other
```

Przykład rosnąco:

```csharp
return GPA.CompareTo(other.GPA);
```

Malejąco:

```csharp
return other.GPA.CompareTo(GPA);
```

---

# 5. `IComparer<T>`

`IComparer<T>` definiuje porządek poza klasą.

```csharp
public class StudentNameComparer : IComparer<Student>
{
    public int Compare(Student? x, Student? y)
    {
        return string.Compare(x!.Name, y!.Name);
    }
}
```

Użycie:

```csharp
students.Sort(new StudentNameComparer());
```

## Różnica

### `IComparable<T>`

```csharp
int CompareTo(T other);
```

Obiekt porównuje:

```text
this z other
```

Czyli:

> „Ja wiem, jak powinienem być domyślnie sortowany.”

### `IComparer<T>`

```csharp
int Compare(T x, T y);
```

Osobny obiekt porównuje:

```text
x z y
```

Czyli:

> „Ja określam dodatkowy sposób sortowania innych obiektów.”

Można mieć wiele comparerów:

```text
StudentNameComparer
StudentGpaComparer
StudentAgeComparer
StudentGpaDescendingComparer
```

---

# 6. Generic Bubble Sort z `IComparable<T>`

```csharp
public static void BubbleSort<T>(T[] array)
    where T : IComparable<T>
{
    for (int i = 0; i < array.Length - 1; i++)
    {
        for (int j = 0; j < array.Length - 1 - i; j++)
        {
            if (array[j].CompareTo(array[j + 1]) > 0)
            {
                (array[j], array[j + 1]) =
                    (array[j + 1], array[j]);
            }
        }
    }
}
```

Tutaj typ `T` sam musi wiedzieć, jak się porównywać.

---

# 7. Generic Bubble Sort z `IComparer<T>`

```csharp
public static void BubbleSort<T>(
    T[] array,
    IComparer<T> comparer)
{
    for (int i = 0; i < array.Length - 1; i++)
    {
        for (int j = 0; j < array.Length - 1 - i; j++)
        {
            if (comparer.Compare(
                    array[j],
                    array[j + 1]) > 0)
            {
                (array[j], array[j + 1]) =
                    (array[j + 1], array[j]);
            }
        }
    }
}
```

Tutaj sposób porównywania jest przekazywany z zewnątrz.

---

# 8. Zamiana dwóch wartości

W C# można zrobić:

```csharp
(a, b) = (b, a);
```

Dla tablicy:

```csharp
(array[i], array[j]) =
    (array[j], array[i]);
```

Klasycznie:

```csharp
T temp = array[i];
array[i] = array[j];
array[j] = temp;
```

---

# 9. `IEnumerable<T>`

`IEnumerable<T>` oznacza:

> Po tym obiekcie można iterować.

Najważniejsza metoda:

```csharp
IEnumerator<T> GetEnumerator();
```

Przykłady:

```csharp
List<int>
HashSet<int>
Dictionary<int, string>
```

implementują `IEnumerable<T>`.

Dzięki temu można używać:

```csharp
foreach (var x in collection)
{
    Console.WriteLine(x);
}
```

---

# 10. `IEnumerator<T>`

Enumerator odpowiada za konkretny przebieg po kolekcji.

Najważniejsze elementy:

```csharp
T Current { get; }

bool MoveNext();

void Reset();

void Dispose();
```

## Mechanizm

Początkowo:

```text
position = -1
```

Potem:

```text
MoveNext()
position = 0
Current = pierwszy element

MoveNext()
position = 1
Current = drugi element

...

MoveNext()
false
```

---

# 11. Jak działa `foreach`

Kod:

```csharp
foreach (var item in collection)
{
    Console.WriteLine(item);
}
```

działa koncepcyjnie podobnie do:

```csharp
var enumerator = collection.GetEnumerator();

while (enumerator.MoveNext())
{
    var item = enumerator.Current;
    Console.WriteLine(item);
}

enumerator.Dispose();
```

Czyli:

```text
IEnumerable<T>
       |
       | GetEnumerator()
       v
IEnumerator<T>
       |
       +-- MoveNext()
       +-- Current
```

---

# 12. Własny enumerator

```csharp
public class MyEnumerator<T> : IEnumerator<T>
{
    private readonly T[] data;
    private readonly int count;
    private int position;

    public MyEnumerator(T[] data, int count)
    {
        this.data = data;
        this.count = count;
        position = -1;
    }

    public T Current
    {
        get
        {
            if (position < 0 || position >= count)
                throw new InvalidOperationException();

            return data[position];
        }
    }

    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
        position++;
        return position < count;
    }

    public void Reset()
    {
        position = -1;
    }

    public void Dispose()
    {
    }
}
```

---

# 13. Własna kolekcja `IEnumerable<T>`

```csharp
public class MyCollection<T> : IEnumerable<T>
{
    private T[] data;
    private int count;

    public MyCollection(int capacity)
    {
        data = new T[capacity];
    }

    public void Add(T item)
    {
        data[count++] = item;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new MyEnumerator<T>(
            data,
            count
        );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

Teraz działa:

```csharp
var c = new MyCollection<int>(10);

c.Add(10);
c.Add(20);
c.Add(30);

foreach (var x in c)
{
    Console.WriteLine(x);
}
```

---

# 14. `yield return`

W praktyce najczęściej nie piszemy `IEnumerator<T>` ręcznie.

Zamiast tego:

```csharp
public IEnumerable<int> Numbers()
{
    yield return 1;
    yield return 2;
    yield return 3;
}
```

Kompilator sam generuje enumerator.

`yield` działa leniwie.

```csharp
IEnumerable<int> result = Numbers();
```

nie oznacza jeszcze wykonania całej metody.

Kolejne elementy są generowane podczas kolejnych:

```text
MoveNext()
```

---

# 15. `yield break`

Kończy iterację.

```csharp
IEnumerable<int> PositiveNumbers(
    IEnumerable<int> numbers)
{
    foreach (var x in numbers)
    {
        if (x < 0)
            yield break;

        yield return x;
    }
}
```

---

# 16. Najważniejsze kolekcje

## `List<T>`

Dynamiczna tablica.

```csharp
List<int> numbers = new();
```

Dobre do:

- dostępu po indeksie,
- przechowywania elementów w kolejności,
- iteracji.

```text
[index]     szybkie
Add         zwykle szybkie
wyszukanie  O(n)
```

---

## `Dictionary<TKey, TValue>`

Mapa:

```text
key -> value
```

```csharp
Dictionary<string, int> ages = new();

ages["Anna"] = 20;
```

Dobre do szybkiego wyszukiwania po kluczu.

```csharp
ages.TryGetValue("Anna", out int age);
```

---

## `HashSet<T>`

Zbiór unikalnych elementów.

```csharp
HashSet<string> sensors = new();

sensors.Add("A");
sensors.Add("A");
```

W zbiorze nadal będzie tylko jedno `"A"`.

Dobre do:

- usuwania duplikatów,
- szybkiego `Contains`,
- operacji na zbiorach.

---

## `SortedSet<T>`

Zbiór:

- bez duplikatów,
- automatycznie uporządkowany.

---

## `SortedDictionary<TKey, TValue>`

Słownik automatycznie uporządkowany według kluczy.

---

## `LinkedList<T>`

Lista dwukierunkowa.

Element posiada odniesienia do:

```text
previous <- element -> next
```

Przydatna, gdy często dodajemy/usuwamy elementy mając już odpowiedni node.

---

## `Queue<T>`

FIFO:

```text
First In, First Out
```

```csharp
queue.Enqueue(x);
queue.Dequeue();
queue.Peek();
```

---

## `Stack<T>`

LIFO:

```text
Last In, First Out
```

```csharp
stack.Push(x);
stack.Pop();
stack.Peek();
```

---

## `PriorityQueue<TElement, TPriority>`

Elementy pobierane są według priorytetu.

```csharp
PriorityQueue<string, int> queue = new();

queue.Enqueue("task", 5);
```

---

# 17. `Predicate<T>`

Funkcja:

```text
T -> bool
```

```csharp
Predicate<int> positive =
    x => x > 0;
```

Użycie:

```csharp
if (positive(10))
{
}
```

Przykład z `yield`:

```csharp
public IEnumerable<T> FindAll<T>(
    IEnumerable<T> data,
    Predicate<T> predicate)
{
    foreach (var item in data)
    {
        if (predicate(item))
            yield return item;
    }
}
```

---

# 18. `Func<>`

Delegat zwracający wartość.

```csharp
Func<int, int> square =
    x => x * x;
```

```text
int -> int
```

Więcej argumentów:

```csharp
Func<int, int, int> add =
    (a, b) => a + b;
```

Ostatni typ jest typem wyniku.

---

# 19. `Action<>`

Delegat bez wartości zwracanej.

```csharp
Action<string> print =
    text => Console.WriteLine(text);
```

Czyli:

```text
string -> void
```

---

# 20. Kowariancja `out`

`out T` oznacza, że `T` jest używany jako wynik.

```csharp
interface IProducer<out T>
{
    T Produce();
}
```

Przykład:

```csharp
IEnumerable<string> strings = new List<string>();

IEnumerable<object> objects = strings;
```

To działa, ponieważ:

```csharp
IEnumerable<out T>
```

jest kowariantny.

---

# 21. Kontrawariancja `in`

`in T` oznacza, że `T` jest używany jako wejście.

```csharp
interface IConsumer<in T>
{
    void Consume(T item);
}
```

Przykład systemowy:

```csharp
IComparer<in T>
```

---

# 22. `List<T>` jest inwariantny

To nie działa:

```csharp
List<string> strings = new();

List<object> objects = strings;
```

Mimo że:

```text
string -> object
```

`List<string>` nie jest `List<object>`.

---

# 23. Najważniejsze rzeczy do zapamiętania

```text
IEquatable<T>
    Equals()
    równość

IComparable<T>
    CompareTo()
    obiekt definiuje swój domyślny porządek

IComparer<T>
    Compare()
    zewnętrzny obiekt definiuje sposób sortowania

IEnumerable<T>
    GetEnumerator()
    obiekt można iterować

IEnumerator<T>
    MoveNext()
    Current
    przechowuje stan iteracji

yield return
    automatycznie tworzy logikę enumeratora

List<T>
    dynamiczna tablica

Dictionary<K,V>
    key -> value

HashSet<T>
    unikalne elementy
```

## Najważniejsze `where`

```csharp
where T : class
where T : class?
where T : struct
where T : notnull
where T : unmanaged
where T : new()
where T : BaseClass
where T : IInterface
where T : U
```

Przykład z kilkoma constraintami:

```csharp
public class Registry<TKey, TValue, TPriority>
    where TKey : notnull, IEquatable<TKey>
    where TPriority : IComparable<TPriority>
{
}
```

Tutaj:

```text
TKey
    nie może być null
    można sprawdzać równość przez Equals()

TPriority
    można porównywać przez CompareTo()
```
# C# — dodatkowe constraints generyczne

## 1. `INumber<T>`

`INumber<T>` służy do tworzenia generycznego kodu działającego na liczbach.

```csharp
using System.Numerics;

public static T Add<T>(T a, T b)
    where T : INumber<T>
{
    return a + b;
}
```

Działa np. dla:

```csharp
int
double
decimal
BigInteger
```

Przykład:

```csharp
int a = Add(2, 3);
double b = Add(2.5, 3.1);
```

Dzięki `INumber<T>` można używać operacji takich jak:

```csharp
+
-
*
/
<
>
```

---

## 2. `IBinaryInteger<T>`

Constraint dla typów całkowitych.

```csharp
where T : IBinaryInteger<T>
```

Przykładowe typy:

```csharp
int
long
short
BigInteger
```

Przydatne, gdy metoda ma działać tylko na liczbach całkowitych.

---

## 3. `IFloatingPoint<T>`

Constraint dla liczb zmiennoprzecinkowych.

```csharp
where T : IFloatingPoint<T>
```

Przydatne, jeśli metoda wymaga operacji specyficznych dla liczb typu floating point.

---

## 4. `IFloatingPointIeee754<T>`

Constraint dla liczb zgodnych ze standardem IEEE 754.

```csharp
where T : IFloatingPointIeee754<T>
```

Przykłady:

```csharp
float
double
Half
```

---

## 5. `INumberBase<T>`

Bardziej podstawowy interfejs liczbowy.

```csharp
where T : INumberBase<T>
```

Używa się go wtedy, gdy nie potrzebujemy wszystkich możliwości `INumber<T>`.

---

# 6. Constraints dla konkretnych operatorów

Można wymagać tylko jednej konkretnej operacji.

## Dodawanie

```csharp
where T : IAdditionOperators<T, T, T>
```

Przykład:

```csharp
public static T Add<T>(T a, T b)
    where T : IAdditionOperators<T, T, T>
{
    return a + b;
}
```

---

## Odejmowanie

```csharp
where T : ISubtractionOperators<T, T, T>
```

---

## Mnożenie

```csharp
where T : IMultiplyOperators<T, T, T>
```

---

## Dzielenie

```csharp
where T : IDivisionOperators<T, T, T>
```

---

# 7. `IComparable<T>`

Constraint oznacza:

> Obiekty typu `T` można uporządkować.

```csharp
where T : IComparable<T>
```

Przykład:

```csharp
public static T Max<T>(T a, T b)
    where T : IComparable<T>
{
    return a.CompareTo(b) > 0 ? a : b;
}
```

Działa dla:

```csharp
int
double
string
DateTime
```

oraz własnych klas implementujących:

```csharp
IComparable<T>
```

Ważne:

```csharp
where T : IComparable<T>
```

nie oznacza, że `T` jest liczbą.

Oznacza tylko, że można ustalić kolejność elementów.

---

# 8. `IEquatable<T>`

Constraint oznacza:

> Obiekty typu `T` można porównywać pod względem równości.

```csharp
where T : IEquatable<T>
```

Dzięki temu można używać:

```csharp
a.Equals(b)
```

Przykład:

```csharp
public static bool Same<T>(T a, T b)
    where T : IEquatable<T>
{
    return a.Equals(b);
}
```

---

# 9. `IParsable<T>`

Pozwala tworzyć generyczny kod parsujący wartości ze `string`.

```csharp
where T : IParsable<T>
```

Przykład:

```csharp
public static T Read<T>(string text)
    where T : IParsable<T>
{
    return T.Parse(text, null);
}
```

Użycie:

```csharp
int x = Read<int>("123");

double y = Read<double>("3.14");

DateTime d = Read<DateTime>("2026-10-30");
```

---

# 10. `ISpanParsable<T>`

Podobne do `IParsable<T>`, ale pozwala parsować z:

```csharp
ReadOnlySpan<char>
```

```csharp
where T : ISpanParsable<T>
```

Jest używane głównie w bardziej wydajnym kodzie, gdzie chcemy ograniczyć tworzenie nowych obiektów `string`.

---

# 11. `IEnumerable<T>` jako constraint

Można wymagać, aby dany typ był iterowalny.

```csharp
public static void Print<TCollection, T>(
    TCollection collection)
    where TCollection : IEnumerable<T>
{
    foreach (var item in collection)
    {
        Console.WriteLine(item);
    }
}
```

Często jednak prościej jest po prostu przyjąć:

```csharp
public static void Print<T>(
    IEnumerable<T> collection)
{
}
```

---

# 12. A co ze `string`?

Nie ma sensu pisać:

```csharp
where T : string
```

Jeżeli metoda ma działać tylko na `string`, zwykle po prostu piszemy:

```csharp
public static void Print(string text)
{
}
```

`string` implementuje jednak wiele przydatnych interfejsów:

```csharp
IComparable<string>
IEquatable<string>
IEnumerable<char>
```

Dlatego `string` pasuje np. do:

```csharp
where T : IComparable<T>
```

Przykład:

```csharp
string result = Max("Anna", "Bob");
```

---

# 13. Najważniejsza różnica

## Jeśli potrzebujemy tylko porównywania

```csharp
where T : IComparable<T>
```

Możemy:

```csharp
a.CompareTo(b)
```

Działa np. dla:

```csharp
int
double
string
DateTime
```

---

## Jeśli potrzebujemy arytmetyki

```csharp
where T : INumber<T>
```

Możemy:

```csharp
a + b
a - b
a * b
a / b
```

---

# 14. Przydatna tabela

| Constraint | Znaczenie |
|---|---|
| `IEquatable<T>` | można sprawdzać równość przez `Equals` |
| `IComparable<T>` | można ustalać kolejność przez `CompareTo` |
| `IEnumerable<T>` | można iterować po elementach |
| `IParsable<T>` | można parsować ze `string` |
| `ISpanParsable<T>` | można parsować z `ReadOnlySpan<char>` |
| `INumber<T>` | pełniejsze operacje liczbowe |
| `INumberBase<T>` | podstawowe możliwości liczbowych typów |
| `IBinaryInteger<T>` | liczby całkowite |
| `IFloatingPoint<T>` | liczby zmiennoprzecinkowe |
| `IFloatingPointIeee754<T>` | liczby zgodne z IEEE 754 |
| `IAdditionOperators<...>` | operator `+` |
| `ISubtractionOperators<...>` | operator `-` |
| `IMultiplyOperators<...>` | operator `*` |
| `IDivisionOperators<...>` | operator `/` |

---

# 15. Przykład bardziej złożonego constraintu

```csharp
public static T AverageLike<T>(T a, T b)
    where T : INumber<T>
{
    return (a + b) / T.CreateChecked(2);
}
```

Tutaj dzięki:

```csharp
where T : INumber<T>
```

wiemy, że można:

```csharp
+
/
```

oraz korzystać z operacji generycznej matematyki.

---

# 16. Najważniejsze do zapamiętania

```text
IEquatable<T>
    -> Equals()

IComparable<T>
    -> CompareTo()
    -> porządek

INumber<T>
    -> operacje liczbowe

IBinaryInteger<T>
    -> liczby całkowite

IFloatingPoint<T>
    -> liczby zmiennoprzecinkowe

IParsable<T>
    -> Parse()

IEnumerable<T>
    -> iteracja
```

Najważniejsze rozróżnienie:

```csharp
where T : IComparable<T>
```

oznacza:

> `T` można porównać i uporządkować.

Natomiast:

```csharp
where T : INumber<T>
```


# C# — najważniejsze kolekcje i ich metody

## 1. `List<T>`

Dynamiczna tablica.

```csharp
List<int> numbers = new();

numbers.Add(10);
numbers.Add(20);
numbers.Add(30);
```

Najważniejsze metody i właściwości:

```csharp
list.Add(value);              // dodaje element na końcu
list.AddRange(collection);    // dodaje wiele elementów

list.Remove(value);           // usuwa pierwszy pasujący element
list.RemoveAt(index);         // usuwa element pod indeksem
list.RemoveAll(predicate);    // usuwa wszystkie spełniające warunek

list.Contains(value);         // czy zawiera element
list.IndexOf(value);          // indeks elementu

list.Insert(index, value);    // wstawia element
list.Clear();                 // usuwa wszystkie elementy

list.Count                    // liczba elementów

list[index]                   // dostęp po indeksie

list.Sort();                  // sortowanie naturalne
list.Sort(comparer);          // sortowanie przez IComparer<T>

list.Reverse();               // odwraca kolejność
```

Przykład:

```csharp
List<int> numbers = [5, 2, 8];

numbers.Sort();

Console.WriteLine(numbers[0]); // 2
```

Najważniejsze:

```text
dostęp po indeksie      szybki
Add                     zwykle szybki
Contains                O(n)
Remove                   O(n)
```

---

# 2. `Dictionary<TKey, TValue>`

Przechowuje pary:

```text
key -> value
```

Przykład:

```csharp
Dictionary<string, int> ages = new();

ages.Add("Anna", 20);
ages["Bob"] = 22;
```

Najważniejsze:

```csharp
dict.Add(key, value);

dict[key] = value;

dict[key];

dict.ContainsKey(key);

dict.ContainsValue(value);

dict.TryGetValue(key, out value);

dict.Remove(key);

dict.Clear();

dict.Count;

dict.Keys;

dict.Values;
```

Najbezpieczniejsze pobieranie:

```csharp
if (dict.TryGetValue("Anna", out int age))
{
    Console.WriteLine(age);
}
```

Iteracja:

```csharp
foreach (var item in dict)
{
    Console.WriteLine(item.Key);
    Console.WriteLine(item.Value);
}
```

albo:

```csharp
foreach (var (key, value) in dict)
{
    Console.WriteLine($"{key}: {value}");
}
```

Typowe zastosowania:

```text
liczenie wystąpień
grupowanie danych
mapowanie key -> value
szybkie wyszukiwanie po kluczu
```

Przykład liczenia:

```csharp
Dictionary<string, int> counts = new();

foreach (var name in names)
{
    if (counts.ContainsKey(name))
        counts[name]++;
    else
        counts[name] = 1;
}
```

---

# 3. `HashSet<T>`

Zbiór unikalnych elementów.

```csharp
HashSet<string> sensors = new();

sensors.Add("A");
sensors.Add("B");
sensors.Add("A");
```

W zbiorze będą:

```text
A
B
```

Najważniejsze:

```csharp
set.Add(value);

set.Remove(value);

set.Contains(value);

set.Clear();

set.Count;
```

Operacje na zbiorach:

```csharp
set.UnionWith(other);             // suma

set.IntersectWith(other);         // część wspólna

set.ExceptWith(other);            // różnica

set.SymmetricExceptWith(other);   // elementy tylko w jednym zbiorze

set.IsSubsetOf(other);

set.IsSupersetOf(other);

set.Overlaps(other);
```

Typowe zastosowanie:

```text
unikalne wartości
usuwanie duplikatów
szybkie Contains
```

---

# 4. `SortedSet<T>`

Podobny do:

```csharp
HashSet<T>
```

ale elementy są automatycznie uporządkowane.

```csharp
SortedSet<int> numbers = new();

numbers.Add(5);
numbers.Add(1);
numbers.Add(3);
```

Iteracja:

```text
1
3
5
```

Najważniejsze:

```csharp
set.Add(value);
set.Remove(value);
set.Contains(value);
set.Count;

set.Min;
set.Max;
```

Można podać własny comparer:

```csharp
SortedSet<Student> students =
    new(new StudentNameComparer());
```

---

# 5. `SortedDictionary<TKey, TValue>`

Podobny do:

```csharp
Dictionary<TKey, TValue>
```

ale klucze są przechowywane w porządku sortowania.

```csharp
SortedDictionary<int, string> dict = new();

dict[10] = "A";
dict[2] = "B";
dict[5] = "C";
```

Iteracja:

```text
2
5
10
```

Najważniejsze metody są prawie takie same jak w `Dictionary`:

```csharp
dict.Add(key, value);

dict[key];

dict.ContainsKey(key);

dict.TryGetValue(key, out value);

dict.Remove(key);

dict.Count;

dict.Keys;

dict.Values;
```

---

# 6. `Queue<T>`

Kolejka FIFO:

```text
First In, First Out
```

Pierwszy dodany element wychodzi jako pierwszy.

```csharp
Queue<int> queue = new();
```

Najważniejsze:

```csharp
queue.Enqueue(value);     // dodaj na koniec

queue.Dequeue();          // pobierz i usuń pierwszy

queue.Peek();             // zobacz pierwszy bez usuwania

queue.TryDequeue(out value);

queue.TryPeek(out value);

queue.Count;

queue.Clear();

queue.Contains(value);
```

Przykład:

```csharp
queue.Enqueue(10);
queue.Enqueue(20);
queue.Enqueue(30);

Console.WriteLine(queue.Dequeue()); // 10
Console.WriteLine(queue.Dequeue()); // 20
```

Typowe zastosowania:

```text
BFS
kolejki zadań
przetwarzanie w kolejności przyjścia
```

---

# 7. `Stack<T>`

Stos LIFO:

```text
Last In, First Out
```

Ostatni dodany element wychodzi jako pierwszy.

```csharp
Stack<int> stack = new();
```

Najważniejsze:

```csharp
stack.Push(value);       // dodaj na stos

stack.Pop();             // pobierz i usuń ostatni

stack.Peek();            // zobacz ostatni bez usuwania

stack.TryPop(out value);

stack.TryPeek(out value);

stack.Count;

stack.Clear();

stack.Contains(value);
```

Przykład:

```csharp
stack.Push(10);
stack.Push(20);
stack.Push(30);

Console.WriteLine(stack.Pop()); // 30
Console.WriteLine(stack.Pop()); // 20
```

Typowe zastosowania:

```text
DFS
undo
nawiasy
rekurencja zastępowana iteracją
```

---

# 8. `LinkedList<T>`

Lista dwukierunkowa.

Każdy element ma:

```text
Previous <- Node -> Next
```

Tworzenie:

```csharp
LinkedList<int> list = new();
```

Najważniejsze:

```csharp
list.AddFirst(value);

list.AddLast(value);

list.RemoveFirst();

list.RemoveLast();

list.Remove(value);

list.Find(value);

list.FindLast(value);

list.First;

list.Last;

list.Count;

list.Clear();
```

Praca z node:

```csharp
LinkedListNode<int>? node = list.Find(10);

if (node != null)
{
    Console.WriteLine(node.Value);
    Console.WriteLine(node.Next?.Value);
    Console.WriteLine(node.Previous?.Value);
}
```

Dodawanie względem node:

```csharp
list.AddBefore(node, value);

list.AddAfter(node, value);
```

Typowe zastosowanie:

```text
częste dodawanie/usuwanie elementów
gdy mamy już LinkedListNode<T>
```

---

# 9. `PriorityQueue<TElement, TPriority>`

Kolejka priorytetowa.

```csharp
PriorityQueue<string, int> queue = new();
```

Dodawanie:

```csharp
queue.Enqueue("low", 10);
queue.Enqueue("high", 1);
queue.Enqueue("medium", 5);
```

Domyślnie mniejszy priorytet jest pobierany wcześniej.

```csharp
queue.Dequeue(); // "high"
```

Najważniejsze:

```csharp
queue.Enqueue(element, priority);

queue.Dequeue();

queue.Peek();

queue.TryDequeue(out element, out priority);

queue.TryPeek(out element, out priority);

queue.Count;

queue.Clear();
```

Typowe zastosowania:

```text
Dijkstra
planowanie zadań
wydarzenia według priorytetu
```

---

# 10. Tablica `T[]`

Nie jest dokładnie tym samym co `List<T>`, ale jest bardzo ważna.

```csharp
int[] array = new int[10];
```

albo:

```csharp
int[] array = [1, 2, 3];
```

Najważniejsze:

```csharp
array.Length;

array[index];

Array.Sort(array);

Array.Reverse(array);

Array.IndexOf(array, value);

Array.BinarySearch(array, value);

Array.Clear(array);
```

Tablica ma stały rozmiar.

```csharp
int[] array = new int[10];
```

nie może nagle mieć 20 elementów.

---

# 11. `IEnumerable<T>`

To nie jest konkretna kolekcja.

To interfejs oznaczający:

> po elementach można iterować.

```csharp
IEnumerable<int>
```

Najważniejsze:

```csharp
GetEnumerator();
```

Dzięki niemu działa:

```csharp
foreach (var x in collection)
{
}
```

---

# 12. `IEnumerator<T>`

Enumerator odpowiada za przechodzenie po kolekcji.

Najważniejsze:

```csharp
MoveNext();

Current;

Reset();

Dispose();
```

Schemat:

```text
IEnumerable<T>
    |
    | GetEnumerator()
    v
IEnumerator<T>
    |
    +-- MoveNext()
    +-- Current
```

---

# 13. `ICollection<T>`

Bazowy interfejs dla wielu kolekcji.

Najważniejsze:

```csharp
Add(item);

Remove(item);

Contains(item);

Clear();

CopyTo(array, index);

Count;

IsReadOnly;
```

---

# 14. `IList<T>`

Rozszerza `ICollection<T>` o operacje indeksowe.

Najważniejsze:

```csharp
list[index];

IndexOf(item);

Insert(index, item);

RemoveAt(index);
```

`List<T>` implementuje:

```csharp
IList<T>
```

---

# 15. `IDictionary<TKey, TValue>`

Interfejs słowników.

Najważniejsze:

```csharp
dict[key];

Add(key, value);

ContainsKey(key);

TryGetValue(key, out value);

Remove(key);

Keys;

Values;

Count;
```

Implementują go m.in.:

```csharp
Dictionary<TKey, TValue>

SortedDictionary<TKey, TValue>
```

---

# 16. `ISet<T>`

Interfejs zbiorów.

Najważniejsze:

```csharp
Add(item);

Remove(item);

Contains(item);

UnionWith(other);

IntersectWith(other);

ExceptWith(other);

IsSubsetOf(other);

IsSupersetOf(other);

Overlaps(other);
```

Implementują:

```csharp
HashSet<T>

SortedSet<T>
```

---

# 17. Najważniejsze metody wspólne

W wielu kolekcjach spotkasz:

```csharp
Count
```

liczba elementów.

```csharp
Add(...)
```

dodawanie.

```csharp
Remove(...)
```

usuwanie.

```csharp
Contains(...)
```

sprawdzenie obecności.

```csharp
Clear()
```

usunięcie wszystkich elementów.

```csharp
foreach (...)
```

iteracja.

---

# 18. Kiedy której kolekcji używać?

| Potrzeba | Kolekcja |
|---|---|
| elementy + indeks | `List<T>` |
| stała liczba elementów | `T[]` |
| key → value | `Dictionary<K,V>` |
| unikalne elementy | `HashSet<T>` |
| unikalne + posortowane | `SortedSet<T>` |
| key → value + posortowane klucze | `SortedDictionary<K,V>` |
| FIFO | `Queue<T>` |
| LIFO | `Stack<T>` |
| szybkie operacje przez node | `LinkedList<T>` |
| element według priorytetu | `PriorityQueue<E,P>` |

---

# 19. Najważniejsze do zapamiętania na lab

```text
List<T>
    Add
    Remove
    RemoveAt
    Contains
    Count
    [index]
    Sort

Dictionary<K,V>
    Add
    [key]
    ContainsKey
    TryGetValue
    Remove
    Keys
    Values

HashSet<T>
    Add
    Remove
    Contains
    UnionWith
    IntersectWith

Queue<T>
    Enqueue
    Dequeue
    Peek

Stack<T>
    Push
    Pop
    Peek

LinkedList<T>
    AddFirst
    AddLast
    Remove
    Find
    First
    Last

PriorityQueue<E,P>
    Enqueue
    Dequeue
    Peek
```

## Minimalny zestaw, który trzeba znać bardzo dobrze

```text
List<T>
Dictionary<TKey, TValue>
HashSet<T>
Queue<T>
Stack<T>
```

A następnie:

```text
SortedSet<T>
SortedDictionary<TKey, TValue>
LinkedList<T>
PriorityQueue<TElement, TPriority>
```

Najważniejsze interfejsy:

```text
IEnumerable<T>   -> iteracja
IEnumerator<T>   -> stan iteracji

ICollection<T>   -> Add / Remove / Count

IList<T>         -> indeksy

IDictionary<K,V> -> key -> value

ISet<T>          -> zbiór
```

oznacza:

> `T` zachowuje się jak typ liczbowy i można wykonywać na nim generyczne operacje arytmetyczne.
