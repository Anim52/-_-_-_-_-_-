using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Infrastructure.Data
{
    

    public sealed class DbContextFactory<T> : IDbContextFactory<T> where T : DbContext
    {
        private readonly IDbContextFactory<T> _inner;

        public DbContextFactory(IDbContextFactory<T> inner) => _inner = inner;

        public T CreateDbContext() => _inner.CreateDbContext();
    }
}
