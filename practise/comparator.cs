public class Student : IComparable<Student>
{
    public string Name { get; set; }
    public double GPA { get; set; }

    public int CompareTo(Student? other)
    {
        return GPA.CompareTo(other!.GPA); // teraz mozna sortowac przez student.Sort()
    }

}

public class StudentClassComparator : IComparable<Student>
{
    public int Compare(Student? x, Student? y)
    {
        return string.Compare(x!.Name,y!.Name);
    }

    public static void BubbleSort<T>(
    T[] array,
    IComparer<T> comparer)
    {
        for(int i = 0 ; i < array.Count; i++)
        {
            for(int j = 0 ; j < array.Count-1; j++)
            {
                if (comparer.Compare(array[j], array[j + 1])>0)
                {
                    (array[j],array[j+1]) = (array[j+1],array[j]);
                } 
            }
        }
    }

    public class StudentClassComparator : IComparer<Student>
    {
        public int Compare(Student? x, Student? y)
        {
            return string.Compare(x!.Name, y!.Name);
        }
    }
}

class Student : IComparable<Student>
{
    
}


//students.Sort();
//students.Sort(new StudentNameComparer());