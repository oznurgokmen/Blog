namespace Blog.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ilk : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Kategoriler",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KategoriAdi = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Yazilar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Baslik = c.String(nullable: false, maxLength: 50),
                        Icerik = c.String(nullable: false),
                        ResimAd = c.String(),
                        Tarih = c.DateTime(nullable: false),
                        KategoriId = c.Int(nullable: false),
                        UyeId = c.Int(nullable: false),
                        Okunma = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kategoriler", t => t.KategoriId)
                .ForeignKey("dbo.Uyeler", t => t.UyeId, cascadeDelete: true)
                .Index(t => t.KategoriId)
                .Index(t => t.UyeId);
            
            CreateTable(
                "dbo.Uyeler",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KullaniciAdi = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Sifre = c.String(nullable: false, maxLength: 100),
                        VCode = c.String(),
                        AdSoyad = c.String(nullable: false),
                        UyeAdmin = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Yorumlar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        YorumIcerik = c.String(nullable: false),
                        Tarih = c.DateTime(nullable: false),
                        UyeId = c.Int(nullable: false),
                        YaziId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Uyeler", t => t.UyeId, cascadeDelete: true)
                .ForeignKey("dbo.Yazilar", t => t.YaziId)
                .Index(t => t.UyeId)
                .Index(t => t.YaziId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Yorumlar", "YaziId", "dbo.Yazilar");
            DropForeignKey("dbo.Yorumlar", "UyeId", "dbo.Uyeler");
            DropForeignKey("dbo.Yazilar", "UyeId", "dbo.Uyeler");
            DropForeignKey("dbo.Yazilar", "KategoriId", "dbo.Kategoriler");
            DropIndex("dbo.Yorumlar", new[] { "YaziId" });
            DropIndex("dbo.Yorumlar", new[] { "UyeId" });
            DropIndex("dbo.Yazilar", new[] { "UyeId" });
            DropIndex("dbo.Yazilar", new[] { "KategoriId" });
            DropTable("dbo.Yorumlar");
            DropTable("dbo.Uyeler");
            DropTable("dbo.Yazilar");
            DropTable("dbo.Kategoriler");
        }
    }
}
