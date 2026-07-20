using AutoMapper;
using EcosCLM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcosCLM.Data.Repositories
{
    public class BaseRepository<T, TEntity> : IBaseRepository<T, TEntity> where T : class, new()
    {
        protected readonly IMapper _mapper;
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(DbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> CollectionBy(Expression<Func<T, IEnumerable<T>>> predicate, T entity)
        {
            return _context.Entry(entity).Collection(predicate).Query();
        }

        public IQueryable<T> IncludingAll(List<Expression<Func<T, object>>> includeProperties)
        {
            ArgumentNullException.ThrowIfNull(includeProperties);

            IQueryable<T> query = GetAll(true);
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate).ConfigureAwait(false);
        }

        public IQueryable<T> GetAll(bool noTracking = true)
        {
            return noTracking ? _dbSet.AsNoTracking() : _dbSet;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task<T> UpdAsync(T entity)
        {
            _context.ChangeTracker.Clear();
            _dbSet.Update(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task DelAsync(T entity)
        {
            _context.ChangeTracker.Clear();
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DelManyAsync(List<T> entity)
        {
            if (entity == null || entity.Count == 0)
                return;

            _context.ChangeTracker.Clear();
            _dbSet.RemoveRange(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public TEntity ToViewModel(T entity) => _mapper.Map<TEntity>(entity);

        public List<TEntity> ToListViewModel(List<T> entity) => _mapper.Map<List<TEntity>>(entity);

        public T ToEntity(TEntity viewModel) => _mapper.Map<T>(viewModel);

        public List<T> ToListEntity(List<TEntity> viewModel) => _mapper.Map<List<T>>(viewModel);

        public IMapper GetMap() => _mapper;
    }
}