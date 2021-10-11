# BitwiseToShortCircuitAnalyzer

A Roslyn analyzer for finding and fixing instances of bitwise operators that should be using short-circuit operators instead.

[![NuGet](https://img.shields.io/nuget/v/BitwiseToShortCircuitAnalyzer.svg?style=flat)](https://www.nuget.org/packages/BitwiseToShortCircuitAnalyzer/)

## Examples

### Bitwise And (&)
```cs
 // Will be flagged for fix.
bool EvaluateAnd(bool a, bool b) => a & b;

// Are both valid and will be ignored.
bool EvaluateAnd2(bool a, bool b) => a && b;
int EvaluateAnd(int a, int b) => a & b;
```

### Bitwise Or (|)
```cs
 // Will be flagged for fix.
bool EvaluateOr(bool a, bool b) => a | b;

// Are both valid and will be ignored.
bool EvaluateOr2(bool a, bool b) => a || b;
int EvaluateOr(int a, int b) => a | b;
```
