# MemoryFootprint
An extension method to approximate the runtime memory consumption of an object.

I was suprised to find no existing method or popular library for this purpose, so I wrote one. The initial commit covers several cases, but I'm sure that there are many cases that it does not cover.

A list of visited objects is kept to avoid counting multiple references to the same object multiple times. If a non-string object is visited more than once (a circular reference), an InvalidOperationException will be thrown. Objects implmenting the ICollection interface will have their items evaluated, but not their fields or properties, as these can produce the same items multiple times.

## Example Usage:

```
public void SizeOfMyStruct_EqualsTwentyThree()
{
    MyStruct myStruct = new MyStruct {  MyChar = ' ', MyInt = 2, MyString = "Hello" };
    long size = myStruct.MemoryFootprint();
    Assert.Equal(23, size);
}
```
