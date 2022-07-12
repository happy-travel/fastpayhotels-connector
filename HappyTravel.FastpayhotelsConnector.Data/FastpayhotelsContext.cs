using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Data;

public class FastpayhotelsContext : DbContext
{
    public FastpayhotelsContext()
    { }

    public FastpayhotelsContext(DbContextOptions<FastpayhotelsContext> options) : base(options)
    { }


    public DbSet<Accommodation> Accommodations { get; set; }
    public DbSet<BookingCodeMapping> BookingCodeMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {        
        builder.Entity<Accommodation>(c =>
        {
            c.HasKey(c => c.Code);
        });

        builder.Entity<BookingCodeMapping>(c =>
        {
            c.HasKey(c => c.ReferenceCode);            
        });
    }
}
