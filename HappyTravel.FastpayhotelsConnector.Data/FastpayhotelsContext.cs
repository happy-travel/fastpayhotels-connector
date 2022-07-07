﻿using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Data;

public class FastpayhotelsContext : DbContext
{
    public FastpayhotelsContext()
    { }

    public FastpayhotelsContext(DbContextOptions<FastpayhotelsContext> options) : base(options)
    { }


    public DbSet<Accommodation> Accommodations { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {        
        builder.Entity<Accommodation>(c =>
        {
            c.HasKey(c => c.Code);
        });
    }
}
