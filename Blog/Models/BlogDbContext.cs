using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext() : base ("Name=BaglantiCumlem")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Yazi>()
                .HasRequired(x => x.Kategori)
                .WithMany(x => x.Yazilar)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Yorum>()
               .HasRequired(x => x.Yazi)
               .WithMany(x => x.Yorumlar)
               .WillCascadeOnDelete(false);
        }

        public DbSet<Kategori> Kategoriler { get; set; }

        public DbSet<Yazi> Yazilar { get; set; }

        public DbSet<Uye> Uyeler { get; set; }

        public DbSet<Yorum> Yorumlar { get; set; }

    }
}