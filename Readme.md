## Bulk Operations EntityFramework

This projects tests out bulk operations in EntityFramework for .NET Framework.
EF6 will be used and Benchmark.DotNet will be used to measure the performance.

The following video on Youtube has been used as a reference and inspiration:

https://www.youtube.com/watch?v=U6-k0TZL_Sw
_Milan Jovanović - What Is the Fastest Way To Do a Bulk Insert? Let’s Find Out_

### SqlBulkCopy

This repo contains helper functionality to easily perform _SqlBulkCopy_ using EntityFramework.
EntityFramework 6.5 is used in .NET Framework in the example code.

The BulkInsert benchmark is available here to test out the performance of the different techniques
to save a batch of entities to the database. We want the best speed and least amount of memory used.
BechmarkDotNet clearly shows the winner after running the benchmarks. Vary the _Param_ of the batch to test it out yourself.


