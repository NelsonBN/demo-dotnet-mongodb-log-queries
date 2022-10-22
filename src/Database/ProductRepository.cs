using Demo.Models;
using MongoDB.Driver;

namespace Demo.Database;

public interface IProductRepository
{
    Task AddAsync(Product entity, CancellationToken cancellationToken = default);
    Task<Product> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> ListAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Product entity, CancellationToken cancellationToken = default);
}

public class ProductRepository : IProductRepository
{
    private const string COLLECTION_NAME = nameof(Product);

    private readonly IMongoCollection<Product> _collection;


    public ProductRepository(IMongoDatabase database)
        => _collection = database.GetCollection<Product>(COLLECTION_NAME);

    public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
        => await _collection
            .InsertOneAsync(
                entity,
                default,
                cancellationToken
            );

    public async Task<Product> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await _collection
            .Find(f => f.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Product>> ListAsync(CancellationToken cancellationToken = default)
        => await _collection
            .Find(f => true)
            .ToListAsync(cancellationToken);

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        => await _collection.ReplaceOneAsync(
            r => r.Id == entity.Id,
            entity,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken
        );
}
