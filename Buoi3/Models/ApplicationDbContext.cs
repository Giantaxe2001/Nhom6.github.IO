using Buoi3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
public class ApplicationDbContext : IdentityDbContext
{
	public ApplicationDbContext()
	{
	}

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<RevenueStatisticViewModel> Statistics { get; set; }
    public DbSet<Poppular> poppulars { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }


}
