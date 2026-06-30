using AutoMapper;
using System.Linq.Expressions;

namespace EcosCLM.Application.Interfaces
{
    public interface IBaseRepository<T, TEntity>
    {
        IQueryable<T> CollectionBy(Expression<Func<T, IEnumerable<T>>> predicate, T entity);
        IQueryable<T> IncludingAll(List<Expression<Func<T, object>>> includeProperties);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll(bool noTracking = true);

        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdAsync(T entity);
        Task DelAsync(T entity);
        Task DelManyAsync(List<T> entity);

        IMapper GetMap();
        TEntity ToViewModel(T entity);
        List<TEntity> ToListViewModel(List<T> entity);
        T ToEntity(TEntity viewModel);
        List<T> ToListEntity(List<TEntity> viewModel);
    }
}