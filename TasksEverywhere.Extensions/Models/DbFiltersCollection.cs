using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Extensions.Models
{
    public class DbFiltersCollection : List<Abstract.IDbFilter>
    {
        public DbFiltersCollection()
        {
        }

        public DbFiltersCollection(int capacity) : base(capacity)
        {
        }

        public DbFiltersCollection(IEnumerable<Abstract.IDbFilter> collection) : base(collection)
        {
        }

        public IEnumerable<Abstract.IDbFilter> GetFilters()
        {
            var filters = this.Where(x => x.GetType().Equals(typeof(DbFilter)));
            return new DbFiltersCollection(filters);
        }

        public bool HasTop()
        {
            return this.Exists(x => x.GetType().Equals(typeof(DbTop)));
        }

        public DbTop GetTopFilter()
        {
            return this.Where(x => x.GetType().Equals(typeof(DbTop))).First().ToType<DbTop>();
        }

    }
}
