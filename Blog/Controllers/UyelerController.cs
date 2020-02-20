using Blog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Blog.Helpers;

namespace Blog.Controllers
{
    public class UyelerController : Controller
    {
        BlogDbContext db = new BlogDbContext();

        // GET: Uyeler
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UyeOl()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UyeOl(Uye model)
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

            yeni.AdSoyad = model.AdSoyad;
            yeni.Email = model.Email;
            yeni.KullaniciAdi = model.KullaniciAdi;
            
            db.Uyeler.Add(yeni);
            db.SaveChanges();

            Uye uye = db.Uyeler.OrderByDescending(u => u.Id).FirstOrDefault();
            Session["uyeOturum"] = true;
            Session["uyeId"] = uye.Id;
            Session["uyeKadi"] = uye.KullaniciAdi;
            Session["uyeAdmin"] = uye.UyeAdmin;

            return RedirectToAction("Index", "Home");
        }

        public ActionResult OturumAc(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OturumAc(Uye model, string returnUrl)
        {
            Uye uye = db.Uyeler.Where(m => m.KullaniciAdi == model.KullaniciAdi).SingleOrDefault();       

            if (uye != null)
            {
                var hashCode = uye.VCode;

                var encodingPasswordString = PasswordHelper.EncodePassword(model.Sifre, hashCode);

                Uye uyeOturumAcan = db.Uyeler.Where(m => m.KullaniciAdi == model.KullaniciAdi && m.Sifre == encodingPasswordString).FirstOrDefault();

                if (uyeOturumAcan != null)
                {
                    Session["uyeOturum"] = true;
                    Session["uyeId"] = uyeOturumAcan.Id;
                    Session["uyeKadi"] = uyeOturumAcan.KullaniciAdi;
                    Session["uyeAdmin"] = uyeOturumAcan.UyeAdmin;

                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return Redirect(returnUrl);
                    }
                }

                ViewBag.hata = "Geçersiz Kullanıcı Adı veya Parola !";
                return View();

            }
            else
            {
                ViewBag.hata = "Geçersiz Kullanıcı Adı veya Parola !";
                return View();
            }
        }

        public ActionResult OturumKapat(string returnUrl)
        {
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }  
        
        public JsonResult YorumYap(string yorum, int Id)
        {
            int uyeId = Convert.ToInt32(Session["uyeId"].ToString());

            Yorum yeni = new Yorum();

            yeni.YaziId = Id;
            yeni.YorumIcerik = yorum;
            yeni.UyeId = uyeId;
            yeni.Tarih = DateTime.Now;
            db.Yorumlar.Add(yeni);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
                
        }

        public JsonResult YorumSil(int id)
        {
            Yorum yorum = db.Yorumlar.Where(m => m.Id == id).SingleOrDefault();
            if (yorum != null)
            {
                db.Yorumlar.Remove(yorum);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UyeDetay(int ? id)
        {
            Uye uye = db.Uyeler.Where(m => m.Id == id).SingleOrDefault();

            if (uye == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(uye);
        }

        public ActionResult YaziEkle()
        {
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

            ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult YaziEkle(Yazi model, HttpPostedFileBase dosya)
        {
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

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
                    if (dosya != null && dosya.ContentLength > 0 && dosya.ContentLength < 1024 * 1024)
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

                    ViewBag.sonuc = "Yazı Başarıyla Eklendi.";

                    ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

                    return View(yazi);
                }

                ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

                return View();

            }

        }

        public ActionResult YaziDuzenle(int? id)
        {
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

            int uyeId = Convert.ToInt32(Session["uyeId"].ToString());

            Yazi yazi = db.Yazilar.Where(m => m.Id == id && m.UyeId == uyeId).SingleOrDefault();

            if (yazi == null)
            {
                return RedirectToAction("UyeDetay/" + uyeId);
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
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

            int uyeId = Convert.ToInt32(Session["uyeId"].ToString());

            if (ModelState.IsValid)
            {
                Yazi yazi = db.Yazilar.Where(m => m.Id == model.Id).SingleOrDefault();

                if (yazi == null)
                {
                    return RedirectToAction("UyeDetay/" + uyeId);
                }

                if (dosya != null && dosya.ContentLength > 0 && dosya.ContentLength < 1024 * 1024)
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

                    ViewBag.sonuc = "Yazı Başarıyla Güncellendi.";

                    ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

                    return View(yazi);
                }
            }

            ViewBag.KategoriId = new SelectList(db.Kategoriler.ToList(), "Id", "KategoriAdi");

            return View(model);

        }
        
        public ActionResult YaziSil(int? id)
        {
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

            int uyeId = Convert.ToInt32(Session["uyeId"].ToString());

            Yazi yazi = db.Yazilar.Where(m => m.Id == id && m.UyeId == uyeId).SingleOrDefault();

            if (yazi == null)
            {
                return RedirectToAction("UyeDetay/" + uyeId);
            }

            if (yazi != null)
            {
                if (System.IO.File.Exists(Server.MapPath("~/img/" + yazi.ResimAd)))
                {
                    System.IO.File.Delete(Server.MapPath("~/img/" + yazi.ResimAd));
                }

                db.Yazilar.Remove(yazi);

                db.SaveChanges();

                TempData["sonuc"] = "\"" + yazi.Baslik + "\" Başlıklı Yazı Başarıyla Silindi.";

                return RedirectToAction("UyeDetay/" + uyeId);
            }

            return RedirectToAction("UyeDetay/" + uyeId);
        }

        public ActionResult ProfilDuzenle(int? id)
        {
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

            Uye uye = db.Uyeler.Where(m => m.Id == id).SingleOrDefault();

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
        public ActionResult ProfilDuzenle(Uye model)
        {
            if (Session["uyeOturum"] == null)
            {
                return RedirectToAction("OturumAc");
            }

            int uyeId = Convert.ToInt32(Session["uyeId"].ToString());

            Uye uye = db.Uyeler.Where(m => m.Id == model.Id).SingleOrDefault();

            var keyNew = PasswordHelper.GeneratePassword(10);
            var password = PasswordHelper.EncodePassword(model.Sifre, keyNew);

            uye.KullaniciAdi = model.KullaniciAdi;
            uye.Sifre = password;
            uye.VCode = keyNew;
            uye.AdSoyad = model.AdSoyad;
            uye.Email = model.Email;
            uye.UyeAdmin = model.UyeAdmin;
            db.SaveChanges();

            TempData["sonuc"] = "Bilgileriniz Başarıyla Güncellendi.";

            return RedirectToAction("UyeDetay/" + uyeId);
        }
    }
}