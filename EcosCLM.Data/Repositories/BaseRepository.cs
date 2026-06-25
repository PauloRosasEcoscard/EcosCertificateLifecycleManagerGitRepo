using AutoMapper;
using EcosCLM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcosCLM.Data.Repositories
{
    public class BaseRepository<T, TEntity> : IBaseRepository<T, TEntity> where T : class, new()
    {
        private IMapper _mapper;
        private DbContext _context;

        public BaseRepository(DbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public IQueryable<T> CollectionBy(Expression<Func<T, IEnumerable<T>>> predicate, T entity)
        {
            return _context.Entry<T>(entity).Collection<T>(predicate).Query();
        }

        public IQueryable<T> IncludingAll(List<Expression<Func<T, object>>> includeProperties)
        {
            IQueryable<T> query = GetAll();
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public T FindOne(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Any(predicate);
        }

        public IQueryable<T> GetAll(bool NoTracking = false)
        {
            var query = _context.Set<T>();

            if (NoTracking)
                return query.AsNoTracking();

            return query;
        }

        public T Add(T entity)
        {
            //_context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public T Upd(T entity)
        {
            // Obtém os metadados da entidade (incluindo a chave primária) dinamicamente
            var entityType = _context.Model.FindEntityType(typeof(T));
            var keyProperties = entityType.FindPrimaryKey().Properties;
            var keyValues = keyProperties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray();

            // Tenta localizar no contexto a instância com os mesmos valores de chave
            var trackedEntity = _context.Set<T>().Find(keyValues);

            if (trackedEntity != null)
            {
                // Se a entidade já estiver sendo rastreada, atualize os valores da instância existente
                _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                // Se não estiver sendo rastreada, anexa a entidade e marca-a como modificada
                _context.Set<T>().Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
            return entity;
        }

        public void Del(T entity)
        {
            _context.Entry<T>(entity);
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public void DelMany(List<T> entities)
        {
            if (entities == null || !entities.Any())
                return;

            _context.Set<T>().RemoveRange(entities);
            _context.SaveChanges();
        }


        public TEntity ToViewModel(T entity)
        {
            return _mapper.Map<TEntity>(entity);
        }

        public List<TEntity> ToListViewModel(List<T> entity)
        {
            return _mapper.Map<List<TEntity>>(entity);
        }

        public T ToEntity(TEntity viewModel)
        {
            return _mapper.Map<T>(viewModel);
        }

        public List<T> ToListEntity(List<TEntity> viewModel)
        {
            return _mapper.Map<List<T>>(viewModel);
        }
        public IMapper GetMap()
        {
            return _mapper;
        }
    }
}
