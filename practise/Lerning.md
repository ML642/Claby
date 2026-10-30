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