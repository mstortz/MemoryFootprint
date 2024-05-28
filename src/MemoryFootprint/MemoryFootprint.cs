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
        var visited = new HashSet<object>(new ReferenceEqualityComparer());

        // Stack is used to detect cyclic references
        var stack = new HashSet<object>(new ReferenceEqualityComparer());

        // Clear the hash codes used by the ReferenceEqualityComparer, otherwise the hash codes will leak memory over multiple calls
        ReferenceEqualityComparer.HashCodes = [];

        return MemoryFootprint(obj, visited, stack);
    }

    private static long MemoryFootprint(object? obj, HashSet<object> visited, HashSet<object> stack)
    {
        if (obj == null)
            return 0;

        Type type = obj.GetType();

        if (type == typeof(string))
        {
            if (visited.Contains(obj))
            {
                return 0;
            }
            visited.Add(obj);
            return IntPtr.Size + (((string)obj).Length * sizeof(char));
        }

        if (_builtInTypes.Contains(type))
        {
            return Marshal.SizeOf(type);
        }

        var isVisited = visited.Contains(obj);
        if (isVisited)
        {
            if (stack.Contains(obj))
            {
                throw new ArgumentException("Cyclic reference detected.");
            }
            else
            {
                return 0;
            }
        }

        visited.Add(obj);
        stack.Add(obj);

        long size = 0;

        if (typeof(IList).IsAssignableFrom(type))
        {
            size = IntPtr.Size; // Pointer to the list

            foreach (var item in (IList)obj)
            {
                size += MemoryFootprint(item, visited, stack);
            }

            stack.Remove(obj);
            return size;
        }

        // Calculate size of all fields
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            object? fieldValue = field.GetValue(obj);
            size += MemoryFootprint(fieldValue, visited, stack);
        }

        // Calculate size of all properties
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (property.GetIndexParameters().Length == 0) // Skip indexers
            {
                object? propertyValue = property.GetValue(obj);
                size += MemoryFootprint(propertyValue, visited, stack);
            }
        }

        stack.Remove(obj);
        return size;
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        // Static so that the two comparers share the same hash codes
        public static Dictionary<object, int> HashCodes { get; set; } = default!;

        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            if (!HashCodes.TryGetValue(obj, out var value))
            {
                value = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
                HashCodes[obj] = value;
            }
            return value;
        }
    }
}
