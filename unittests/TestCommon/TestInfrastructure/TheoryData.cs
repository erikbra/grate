// using System.Collections;
//
// namespace TestCommon.TestInfrastructure;
//
// public abstract record TheoryData() : IEnumerable<object[]>
// {
//     readonly List<object[]> _data = [];
//     protected void AddRow(params object[] values) => _data.Add(values);
//     public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
// }
//
// public record TheoryData<T1> : TheoryData;
// public record TheoryData<T1, T2> : TheoryData;
// public record TheoryData<T1, T2, T3> : TheoryData;
// public record TheoryData<T1, T2, T3, T4> : TheoryData;
// public record TheoryData<T1, T2, T3, T4, T5> : TheoryData;
// public record TheoryData<T1, T2, T3, T4, T5, T6> : TheoryData;
// public record TheoryData<T1, T2, T3, T4, T5, T6, T7> : TheoryData;
