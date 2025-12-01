using Backend.Data;
using Backend.Models;

namespace Backend.Services;

public interface IDbService
{
    // CREATE
    Task<bool> AddAsync<T>(T entity) where T : class;

    // READ
    Task<T?> GetByIdAsync<T>(object id) where T : class;

    // UPDATE
    Task<bool> UpdateAsync<T>(T entity) where T : class;

    // DELETE
    Task<bool> DeleteAsync<T>(object id) where T : class;

    // SAVE
    Task<bool> SaveAsync<T>();


    // Query all
    IQueryable<T> Query<T>() where T : class;
}


public class DbService : IDbService
{
    private readonly ApplicationDbContext _context;
    public DbService(ApplicationDbContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    // CREATE
    public async Task<bool> AddAsync<T>(T entity) where T : class
        => _context.Set<T>().Add(entity) != null && await _context.SaveChangesAsync() > 0;

    // READ
    public async Task<T?> GetByIdAsync<T>(object id) where T : class
        => await _context.Set<T>().FindAsync(id);

    // UPDATE
    public async Task<bool> UpdateAsync<T>(T entity) where T : class
        => _context.Set<T>().Update(entity) != null && await _context.SaveChangesAsync() > 0;

    // DELETE
    public async Task<bool> DeleteAsync<T>(object id) where T : class
        => await GetByIdAsync<T>(id) is T entity
            ? (_context.Set<T>().Remove(entity) != null && await _context.SaveChangesAsync() > 0)
            : false;

    // Query all
    public IQueryable<T> Query<T>() where T : class
        => _context.Set<T>();

    public async Task<bool> SaveAsync<T>() => await _context.SaveChangesAsync() > 0;
}