# MemoryFootprint
An extension method to calculate the memory consumption of an object.

I was suprised to find no existing method or popular library for this purpose, so I wrote one. The initial commit covers several cases, but I'm sure that there are many cases that it does not cover.

A list of visited objects is kept to avoid counting multiple references to the same object multiple times, and a stack of the current tree of objects is kept to detect circular references, which will cause an ArgumentException to be thrown.
