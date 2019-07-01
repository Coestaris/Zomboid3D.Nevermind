using System.Collections.Generic;

namespace Nevermind.Compiler
{
    public struct TokenIterator<T>
    {
        private readonly List<T> _list;
        public T Current;
        public int Index;

        public TokenIterator(List<T> list)
        {
            _list = list;
            Index = -1;
            Current = default(T);
        }

        public T GetNext()
        {
            if(Index >= _list.Count - 1) return default(T);
            Current = _list[++Index];
            return Current;
        }
    }
}