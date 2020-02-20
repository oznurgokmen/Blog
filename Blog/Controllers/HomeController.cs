using Blog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        BlogDbContext db = new BlogDbContext();

        // GET: Home
        public ActionResult Index()
        {
            List<Yazi> yazilar = db.Yazilar.OrderByDescending(m => m.Id).Take(3).ToList();

            return View(yazilar);
        }

        public ActionResult Kategoriler()
        {
            List<Kategori> kategoriler = db.Kategoriler.ToList();

            return PartialView(kategoriler);
        }

        public ActionResult KategoriYazi(int? id)
        {
            Kategori kat = db.Kategoriler.Where(s => s.Id == id).SingleOrDefault();

            if (kat == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.kategori = kat.KategoriAdi;

            List<Yazi> yazilar = db.Yazilar.Where(s => s.KategoriId == id).ToList();

            if (yazilar.Count == 0)
            {
                ViewBag.kayityok = "Henüz Bu Kategoride Yazı Yoktur !";
            }

            return View(yazilar);
        }

        public ActionResult YaziDetay(int? id)
        {
            Yazi yazi = db.Yazilar.Where(m => m.Id == id).SingleOrDefault();

            if (yazi == null)
            {
                return RedirectToAction("Index");
            }

            yazi.Okunma += 1;

            db.SaveChanges();

            return View(yazi);
        }
        
        public PartialViewResult Yorumlar(int ? id)
        {
            List<Yorum> yorumlar = db.Yorumlar.Where(m => m.YaziId == id).ToList();

            return PartialView(yorumlar);
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

        public ActionResult SonEklenenler()
        {
            List<Yazi> yazilar = db.Yazilar.OrderByDescending(m => m.Id).Take(5).ToList();

            return PartialView(yazilar);
        }

        public ActionResult Contact()
        {
            return View();
        }

    }
 
}
