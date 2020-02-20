using Blog.Auth;
using Blog.Helpers;
using Blog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class AdminController : BaseController
    {
        BlogDbContext db = new BlogDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            List<Kategori> kategoriler = db.Kategoriler.ToList();
            ViewBag.katSayi = kategoriler.Count;

            List<Yazi> yazilar = db.Yazilar.ToList();
            ViewBag.yaziSayi = yazilar.Count;

            List<Uye> uyeler = db.Uyeler.ToList();
            ViewBag.uyeSayi = uyeler.Count;

            return View();
        }

        public ActionResult Kategoriler()
        {                      
            List<Kategori> kategoriler = db.Kategoriler.ToList();

            return View(kategoriler);
        }

        public ActionResult KategoriEkle()
        {         
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle(Kategori model)
        {
            if (db.Kategoriler.Where(m => m.KategoriAdi == model.KategoriAdi).Count() > 0)
            {
                ViewBag.hata = "Girilen Kategori Kayıtlıdır !";
                return View();
            }
            else
            {
                Kategori kat = new Kategori();
                kat.KategoriAdi = model.KategoriAdi;
                db.Kategoriler.Add(kat);
                db.SaveChanges();
                ViewBag.sonuc = "Kategori Başarıyla Eklendi.";
                return View();
            }

        }

        public ActionResult KategoriDuzenle(int ? id)
        {
            Kategori kat = db.Kategoriler.Where(m => m.Id == id).SingleOrDefault();

            if (kat == null)
            {
                return RedirectToAction("Kategoriler");
            }

            Kategori yeni = new Kategori();
            yeni.Id = kat.Id;
            yeni.KategoriAdi = kat.KategoriAdi;
     
            return View(yeni);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriDuzenle(Kategori model)
        {
            Kategori kat = db.Kategoriler.Where(m => m.Id == model.Id).SingleOrDefault();

            if (kat != null)
            {
                kat.KategoriAdi = model.KategoriAdi;
                db.SaveChanges();

                TempData["sonuc"] = "\"" + model.KategoriAdi + "\" Adlı Kategori Başarıyla Güncellendi.";
            }

            return RedirectToAction("Kategoriler");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriSil(int id)
        {
            Kategori kat = db.Kategoriler.Where(m => m.Id == id).SingleOrDefault();

            if (db.Yazilar.Where(m => m.KategoriId == id).Count() > 0)
            {
                TempData["hata"] = "\"" + kat.KategoriAdi + "\" Adlı Kategori Altında Yazılar Bulunduğundan Silinemiyor !";

                return RedirectToAction("Kategoriler");
            }         

                db.Kategoriler.Remove(kat);
                db.SaveChanges();
            TempData["sonuc"] = "\"" + kat.KategoriAdi + "\" Adlı Kategori Başarıyla Silindi.";

            return RedirectToAction("Kategoriler");        
                 
        }

        public ActionResult Yazilar()
        {
            List<Yazi> yazilar = db.Yazilar.ToList();

            return View(yazilar);
        }

        public ActionResult YaziEkle()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YaziEkle(Yazi model, HttpPostedFileBase dosya)
        {         
            int uyeId = Convert.ToInt32(Session["uyeId"].ToString());

            if (db.Yazilar.Where(m => m.Baslik == model.Baslik).Count() > 0)
            {
                ViewBag.hata = "Girilen Başlık Kayıtlıdır !";

                ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

                return View(model);
            }
            else
            {
                Yazi yazi = new Yazi();

                if (ModelState.IsValid)
                {
                    if (dosya != null && dosya.ContentLength > 0 && dosya.ContentLength < 1024 * 1024 )
                    {
                        var ext = Path.GetExtension(dosya.FileName);

                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                        if (dosya.ContentType.StartsWith("image/") && allowedExtensions.Contains(ext))
                        {
                            string yeniDosyaAd = Guid.NewGuid().ToString() + ext;

                            string kaydetmeYolu = Server.MapPath("~/img/" + yeniDosyaAd);

                            dosya.SaveAs(kaydetmeYolu);

                            model.ResimAd = yeniDosyaAd;
                           
                        }
                    }

                    yazi.Baslik = model.Baslik;
                    yazi.KategoriId = model.KategoriId;
                    yazi.Icerik = model.Icerik;
                    yazi.ResimAd = model.ResimAd;
                    yazi.Tarih = DateTime.Now;
                    yazi.Okunma = 0;
                    yazi.UyeId = uyeId;

                    db.Yazilar.Add(yazi);
                    db.SaveChanges();

                    TempData["sonuc"] = "\"" + yazi.Baslik + "\" Başlıklı Yazı Başarıyla Eklendi.";

                    return RedirectToAction("Yazilar");
                }

                ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

                return View();

            }
         
        }

        //Yazı düzenleme kısmı Admin tarafından çıkarılabilir, etik olmadı :)
        public ActionResult YaziDuzenle(int ? id)
        { 
            Yazi yazi = db.Yazilar.Where(m => m.Id == id).SingleOrDefault();

            if (yazi == null)
            {
                return RedirectToAction("Yazilar");
            }

            Yazi model = new Yazi();
                model.Id = yazi.Id;
                model.Baslik = yazi.Baslik;
                model.Icerik = yazi.Icerik;
                model.KategoriId = yazi.KategoriId;

                ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");
                return View(model);         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YaziDuzenle(Yazi model, HttpPostedFileBase dosya)
        {
            if (ModelState.IsValid)
            {
                Yazi yazi = db.Yazilar.Where(m => m.Id == model.Id).SingleOrDefault();

                if (yazi == null)
                {
                    return RedirectToAction("Yazilar");
                }

                if (dosya != null && dosya.ContentLength > 0 && dosya.ContentLength < 1024 * 1024 )
                {
                    var ext = Path.GetExtension(dosya.FileName);

                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                    if (!dosya.ContentType.StartsWith("image/") || !allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("FotoAd", "Geçersiz Bir Resim Dosyası Yüklediniz !");
                    }

                    if (ModelState.IsValid)
                    {
                        if (!string.IsNullOrEmpty(yazi.ResimAd))
                        {
                            var dbdekiEskiResimYol = Server.MapPath("~/img/" + yazi.ResimAd);

                            if (System.IO.File.Exists(dbdekiEskiResimYol))
                            {
                                System.IO.File.Delete(dbdekiEskiResimYol);
                            }
                        }

                        string yeniDosyaAd = Guid.NewGuid().ToString() + ext;

                        string kaydetmeYolu = Server.MapPath("~/img/" + yeniDosyaAd);

                        dosya.SaveAs(kaydetmeYolu);

                        yazi.ResimAd = yeniDosyaAd;
                    }
                }

                if (ModelState.IsValid)
                {
                    yazi.Baslik = model.Baslik;
                    yazi.KategoriId = model.KategoriId;
                    yazi.Icerik = model.Icerik;                
                    db.SaveChanges();

                    TempData["sonuc"] = "\"" + yazi.Baslik + "\" Başlıklı Yazı Başarıyla Güncellendi.";

                    return RedirectToAction("Yazilar");               
                }
            }

            ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

            return View(model);
    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YaziSil(int id)
        {
            Yazi yazi = db.Yazilar.Where(m => m.Id == id).SingleOrDefault();

            if (yazi == null)
            {
                return RedirectToAction("Yazilar");
            }

            if (yazi != null)
            {
                if (System.IO.File.Exists(Server.MapPath("~/img/" + yazi.ResimAd)))
                {
                    System.IO.File.Delete(Server.MapPath("~/img/" + yazi.ResimAd));
                }

                List<Yorum> yorumlar = db.Yorumlar.Where(m => m.YaziId == id).ToList();
                db.Yorumlar.RemoveRange(yorumlar);
                db.Yazilar.Remove(yazi);

                db.SaveChanges();

                TempData["sonuc"] = "\"" + yazi.Baslik + "\" Başlıklı Yazı Başarıyla Silindi.";

                return RedirectToAction("Yazilar");
            }

            return RedirectToAction("Yazilar");
        }  
        
        public ActionResult Uyeler()
        {
            List<Uye> uyeler = db.Uyeler.ToList();

            return View(uyeler);
        }

        public ActionResult UyeEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeEkle(Uye model)
        {
            if (db.Uyeler.Where(m => m.KullaniciAdi == model.KullaniciAdi).Count() > 0)
            {
                ViewBag.hata = "Girilen Kullanıcı Adı Kayıtlıdır !";
                return View();
            }

            Uye yeni = new Uye();

            var keyNew = PasswordHelper.GeneratePassword(10);
            var password = PasswordHelper.EncodePassword(model.Sifre, keyNew);
            yeni.Sifre = password;
            yeni.VCode = keyNew;
            yeni.KullaniciAdi = model.KullaniciAdi;
            yeni.AdSoyad = model.AdSoyad;
            yeni.Email = model.Email;
            yeni.UyeAdmin = model.UyeAdmin;
            db.Uyeler.Add(yeni);
            db.SaveChanges();

            ViewBag.sonuc = "Üye Başarıyla Eklendi.";
            return View();
        }

        //Bu üye düzenleme kısmı da çıkarılabilir, admin şifreleri değiştirebilir :)
        public ActionResult UyeDuzenle(int? id)
        {
            Uye uye = db.Uyeler.Where(m => m.Id == id).SingleOrDefault();

            if (uye == null)
            {
                return RedirectToAction("Uyeler");
            }

            Uye model = new Uye();

            model.Sifre = uye.Sifre;
            model.VCode = uye.VCode;
            model.Id = uye.Id;
            model.KullaniciAdi = uye.KullaniciAdi;       
            model.AdSoyad = uye.AdSoyad;
            model.Email = uye.Email;
            model.UyeAdmin = uye.UyeAdmin;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeDuzenle(Uye model)
        {
            Uye uye = db.Uyeler.Where(m => m.Id == model.Id).SingleOrDefault();

            if (uye == null)
            {
                return RedirectToAction("Uyeler");
            }

            var keyNew = PasswordHelper.GeneratePassword(10);
            var password = PasswordHelper.EncodePassword(model.Sifre, keyNew);

            uye.KullaniciAdi = model.KullaniciAdi;
            uye.Sifre = password;
            uye.VCode = keyNew;
            uye.AdSoyad = model.AdSoyad;
            uye.Email = model.Email;
            uye.UyeAdmin = model.UyeAdmin;
            db.SaveChanges();

            TempData["sonuc"] = "Üye Başarıyla Güncellendi.";

            return RedirectToAction("Uyeler");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeSil(int id)
        {
            Uye uye = db.Uyeler.Where(m => m.Id == id).SingleOrDefault();

            if (uye == null)
            {
                return RedirectToAction("Uyeler");
            }
                   
            if (uye != null)
            {
                List<Yazi> yazilar = db.Yazilar.Where(m => m.UyeId == uye.Id).ToList();
                db.Yazilar.RemoveRange(yazilar);
                List<Yorum> yorumlar = db.Yorumlar.Where(m => m.UyeId == id).ToList();
                db.Yorumlar.RemoveRange(yorumlar);
                db.Uyeler.Remove(uye);
                db.SaveChanges();

                TempData["sonuc"] = "\"" + uye.KullaniciAdi + "\" Adlı Üye Başarıyla Silindi.";

                return RedirectToAction("Uyeler");
            }

            return RedirectToAction("Uyeler");
        }

        [HttpPost]

        public ActionResult BlogAra(string deger)
        {
            ViewBag.deger = deger;

            var aranan = db.Yazilar.Where(s => s.Baslik.Contains(deger) || s.Icerik.Contains(deger)).ToList();

            if (aranan.Count > 0)
            {
                ViewBag.kayityok = aranan.Count + " Adet Yazı Bulundu.";
            }
            else
            {
                ViewBag.kayityok = "Aradığınız Kritere Uygun Yazı Bulunamadı !";
            }

            return View(aranan.OrderByDescending(o => o.Id).ToList());
        }

    }
}