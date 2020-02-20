namespace Blog.Migrations
{
    using Blog.Helpers;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.BlogDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Blog.Models.BlogDbContext context)
        {
            if (!context.Kategoriler.Any())
            {
                context.Kategoriler.Add(new Models.Kategori { KategoriAdi = "Yazýlým" });
                context.Kategoriler.Add(new Models.Kategori { KategoriAdi = "Edebiyat" });
                context.Kategoriler.Add(new Models.Kategori { KategoriAdi = "Bilim" });
                context.Kategoriler.Add(new Models.Kategori { KategoriAdi = "Sinema" });
            }

            if (!context.Uyeler.Any())
            {
                var keyNew = PasswordHelper.GeneratePassword(10);
                var password = PasswordHelper.EncodePassword("Ankara1.", keyNew);
                
                context.Uyeler.Add(new Models.Uye { KullaniciAdi = "admin", AdSoyad = "uye Admin", Email = "admin@example.com", Sifre = password, VCode = keyNew, UyeAdmin = 1 });
            }

        }
    }
}
