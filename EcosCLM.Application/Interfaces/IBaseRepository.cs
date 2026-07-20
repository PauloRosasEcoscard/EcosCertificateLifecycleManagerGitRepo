using AutoMapper;
using System.Linq.Expressions;

namespace EcosCLM.Application.Interfaces
{
    public interface IBaseRepository<T, TEntity>
    {
        public IQueryable<T> CollectionBy(Expression<Func<T, IEnumerable<T>>> predicate, T entity);
        public IQueryable<T> IncludingAll(List<Expression<Func<T, object>>> includeProperties);
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        public IQueryable<T> GetAll(bool noTracking = true);

        public Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        public Task<T> AddAsync(T entity);
        public Task<T> UpdAsync(T entity);
        public Task DelAsync(T entity);
        public Task DelManyAsync(List<T> entity);

        public IMapper GetMap();
        public TEntity ToViewModel(T entity);
        public List<TEntity> ToListViewModel(List<T> entity);
        public T ToEntity(TEntity viewModel);
        public List<T> ToListEntity(List<TEntity> viewModel);
    }
}