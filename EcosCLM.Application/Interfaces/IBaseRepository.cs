using AutoMapper;
using System.Linq.Expressions;

namespace EcosCLM.Application.Interfaces
{
    public interface IBaseRepository<T, TEntity>
    {
        IQueryable<T> CollectionBy(Expression<Func<T, IEnumerable<T>>> predicate, T entity);
        IQueryable<T> IncludingAll(List<Expression<Func<T, object>>> includeProperties);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll(bool NoTracking = false);
        T FindOne(Expression<Func<T, bool>> predicate);
        bool Exists(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        T Upd(T entity);
        void Del(T entity);
        void DelMany(List<T> entity);
        IMapper GetMap();

        TEntity ToViewModel(T entity);
        List<TEntity> ToListViewModel(List<T> entity);
        T ToEntity(TEntity viewModel);
        List<T> ToListEntity(List<TEntity> viewModel);
    }
}
