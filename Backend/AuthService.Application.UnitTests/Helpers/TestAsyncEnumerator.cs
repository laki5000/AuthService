namespace AuthService.Application.UnitTests.Helpers
{
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;

        public T Current => _enumerator.Current;

        public ValueTask DisposeAsync() { _enumerator.Dispose(); return ValueTask.CompletedTask; }

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
    }
}
