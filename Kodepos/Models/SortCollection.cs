using System.Collections;

namespace Kodepos.Models
{
    public record SortCollection : IEnumerable<Sort>
    {
        List<Sort> sorts;
        public SortCollection()
        {
            sorts = [];
        }

        public void Add(Sort sort)
        {
            sorts.Add(sort);
        }

        public IEnumerator<Sort> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(Sort sort)
        {
            sorts.Remove(sort);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
