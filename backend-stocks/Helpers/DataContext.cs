namespace WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataContext : DbContext
{
    public DbSet<Access> Accesses => Set<Access>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Stock> Stock => Set<Stock>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<Unity> Unities => Set<Unity>();
    public DbSet<User> Users => Set<User>();

    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Configuration.GetConnectionString("StocksDatabase"));
    }
}
