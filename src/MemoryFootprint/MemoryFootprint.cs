using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Extensions.Object;

public static class ObjectExtensions
{
    private static readonly HashSet<Type> _builtInTypes =
    [
        typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float),
        typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(IntPtr),
        typeof(UIntPtr)
    ];

    public static long MemoryFootprint(this object? obj)
    {
        // Visited objects are stored in a HashSet to avoid double counting
        return MemoryFootprint(obj, []);
    }

    private static long MemoryFootprint(object? obj, HashSet<object> visited)
    {
        if (obj == null)
            return 0;

        Type type = obj.GetType();

        if (_builtInTypes.Contains(type))
        {
            return Marshal.SizeOf(type);
        }

        var isVisited = visited.Contains(obj);

        if (type == typeof(string))
        {
            if (isVisited)
            {
                return 0;
            }
            else
            {
                visited.Add(obj);
                return IntPtr.Size + (((string)obj).Length * sizeof(char));
            }
        }

        if (isVisited)
        {
            throw new InvalidOperationException("Cyclic reference detected");
        }

        visited.Add(obj);
        long size = 0;

        if (typeof(IList).IsAssignableFrom(type))
        {
            size = IntPtr.Size; // Pointer to the list

            foreach (var item in (IList)obj)
            {
                size += MemoryFootprint(item, visited);
            }
            return size;
        }

        // Calculate size of all fields
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            object? fieldValue = field.GetValue(obj);
            size += MemoryFootprint(fieldValue, visited);
        }

        // Calculate size of all properties
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length == 0) // Skip indexers
            {
                object? propertyValue = property.GetValue(obj);
                size += MemoryFootprint(propertyValue, visited);
            }
        }

        return size;
    }
}
