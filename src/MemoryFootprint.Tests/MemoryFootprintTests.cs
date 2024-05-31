namespace MemoryFootprint.Tests;

using Extensions.Object;
using FluentAssertions;

public class MemoryFootprintTests
{
    [Fact]
    public void SizeOfSbyte_EqualsOne()
    {
        const sbyte mySbyte = 2;
        long size = mySbyte.MemoryFootprint();
        Assert.Equal(1, size);
    }

    [Fact]
    public void SizeOfChar_EqualsTwo()
    {
        const char myChar = ' ';
        long size = myChar.MemoryFootprint();
        Assert.Equal(1, size);
    }

    [Fact]
    public void SizeOfDecimal_EqualsFour()
    {
        const decimal myDecimal = 2;
        long size = myDecimal.MemoryFootprint();
        Assert.Equal(16, size);
    }

    [Fact]
    public void SizeOfDouble_EqualsEight()
    {
        const double myDouble = 2;
        long size = myDouble.MemoryFootprint();
        Assert.Equal(8, size);
    }

    [Fact]
    public void SizeOfFloat_EqualsFour()
    {
        const float myFloat = 2;
        long size = myFloat.MemoryFootprint();
        Assert.Equal(4, size);
    }

    [Fact]
    public void SizeOfInt_EqualsFour()
    {
        const int myInt = 2;
        long size = myInt.MemoryFootprint();
        Assert.Equal(4, size);
    }

    [Fact]
    public void SizeOfUint_EqualsFour()
    {
        const uint myUint = 2;
        long size = myUint.MemoryFootprint();
        Assert.Equal(4, size);
    }

    [Fact]
    public void SizeOfNint_EqualsEight()
    {
        nint myNint = 2;
        long size = myNint.MemoryFootprint();
        Assert.Equal(8, size);
    }

    [Fact]
    public void SizeOfNuint_EqualsEight()
    {
        nuint myNuint = 2;
        long size = myNuint.MemoryFootprint();
        Assert.Equal(8, size);
    }

    [Fact]
    public void SizeOfLong_EqualsEight()
    {
        const long myLong = 2;
        long size = myLong.MemoryFootprint();
        Assert.Equal(8, size);
    }

    [Fact]
    public void SizeOfUlong_EqualsEight()
    {
        const ulong myUlong = 2;
        long size = myUlong.MemoryFootprint();
        Assert.Equal(8, size);
    }

    [Fact]
    public void SizeOfShort_EqualsTwo()
    {
        const short myShort = 2;
        long size = myShort.MemoryFootprint();
        Assert.Equal(2, size);
    }

    [Fact]
    public void SizeOfUshort_EqualsTwo()
    {
        const ushort myUshort = 2;
        long size = myUshort.MemoryFootprint();
        Assert.Equal(2, size);
    }

    struct MyStruct
    {
        public int MyInt;
        public char MyChar;
        public string MyString;
    }

    [Fact]
    public void SizeOfMyStruct_EqualsTwentyThree()
    {
        MyStruct myStruct = new MyStruct {  MyChar = ' ', MyInt = 2, MyString = "Hello" };
        long size = myStruct.MemoryFootprint();
        Assert.Equal(23, size);
    }

    class MyClass
    {
        public int MyInt;
        public char MyChar;
        public string MyString = default!;
    }

    [Fact]
    public void SizeOfMyClass_EqualsTwentyThree()
    {
        MyClass myClass = new MyClass { MyChar = ' ', MyInt = 2, MyString = "Hello" };
        long size = myClass.MemoryFootprint();
        Assert.Equal(23, size);
    }

    class MyReferenceClass
    {
        public string String1 = default!;
        public string String2 = default!;
    }

    [Fact]
    public void SizeOfMyReferenceClass_ShouldNotEqual()
    {
        const string s1 = "This is a test string";
        const string s2 = "This is a cool string";
        MyReferenceClass myRefClass1 = new MyReferenceClass { String1 = s1, String2 = s1 };
        MyReferenceClass myRefClass2 = new MyReferenceClass { String1 = s1, String2 = s2 };
        long size1 = myRefClass1.MemoryFootprint();
        long size2 = myRefClass2.MemoryFootprint();
        size1.Should().Be(50);
        size2.Should().Be(100);
        size1.Should().NotBe(size2);
    }

    public class MyRecursiveClass
    {
        public MyRecursiveClass Recurse = default!;
    }

    [Fact]
    public void RecursiveClass_ShouldNotThrow()
    {
        var c1 = new MyRecursiveClass();
        var c2 = new MyRecursiveClass();
        c1.Recurse = c2;
        c2.Recurse = c1;
        long size = 0;
        c1.Invoking(x => size = x.MemoryFootprint()).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NullObject_ShouldReturnZero()
    {
        object? obj = null;
        long size = obj.MemoryFootprint();
        size.Should().Be(0);
    }

    [Fact]
    public void Array_ShouldReturnSizeOfArray()
    {
        int[] obj = [1, 2, 3];
        long size = obj.MemoryFootprint();
        size.Should().BeGreaterOrEqualTo(obj.Length * sizeof(int));
    }

    [Fact]
    public void Collection_ShouldReturnSizeOfCollection()
    {
        var obj = new List<int> { 1, 2, 3 };
        long size = obj.MemoryFootprint();
        size.Should().BeGreaterOrEqualTo(obj.Count * sizeof(int));
    }

    [Fact]
    public void CustomObject_ShouldReturnSizeOfObject()
    {
        var obj = new CustomObject { Property1 = "Test", Property2 = 123 };
        long size = obj.MemoryFootprint();
        size.Should().BeGreaterOrEqualTo(sizeof(int) + (sizeof(char) * obj.Property1.Length));
    }

    private class CustomObject
    {
        public string Property1 { get; set; } = "Hello";
        public int Property2 { get; set; }
    }
}
