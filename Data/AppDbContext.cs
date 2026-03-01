using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirlfriendRateApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GirlfriendRateApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RatingItem> RatingItems { get; set; }
    }
}