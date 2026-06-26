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
            IQueryable<T> query = GetAll(true);
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }

        public T? FindOne(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public IQueryable<T> GetAll(bool noTracking = true)
        {
            return noTracking ? _dbSet.AsNoTracking() : _dbSet;
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public T Upd(T entity)
        {
            _context.ChangeTracker.Clear();
            _dbSet.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Del(T entity)
        {
            _context.ChangeTracker.Clear();
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void DelMany(List<T> entities)
        {
            if (entities == null || !entities.Any())
                return;

            _context.ChangeTracker.Clear();
            _dbSet.RemoveRange(entities);
            _context.SaveChanges();
        }

        public TEntity ToViewModel(T entity) => _mapper.Map<TEntity>(entity);
        public List<TEntity> ToListViewModel(List<T> entity) => _mapper.Map<List<TEntity>>(entity);
        public T ToEntity(TEntity viewModel) => _mapper.Map<T>(viewModel);
        public List<T> ToListEntity(List<TEntity> viewModel) => _mapper.Map<List<T>>(viewModel);
        public IMapper GetMap() => _mapper;
    }
}