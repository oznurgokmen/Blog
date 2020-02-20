using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models
{
    [Table("Yazilar")]

    public class Yazi
    {
        public int Id { get; set; }

        [DisplayName("Yazı Başlığı")]

        [Required(ErrorMessage = "Lütfen Yazı Başlığını Giriniz !")]

        [StringLength(50, ErrorMessage = "Yazı Başlığı En Fazla 50 Karakter Olmalıdır !")]

        public string Baslik { get; set; }

        [AllowHtml]

        [DisplayName("Yazı İçeriği")]

        [Required(ErrorMessage = "Lütfen Yazı İçeriğini Giriniz !")]

        public string Icerik { get; set; }

        [DisplayName("Yazı Fotoğrafı")]

        public string ResimAd { get; set; }

        public DateTime Tarih { get; set; }

        [DisplayName("Yazı Kategori")]

        [Required(ErrorMessage = "Lütfen Yazı Kategorisini Seçiniz !")]

        public int KategoriId { get; set; }

        public int UyeId { get; set; }

        public int Okunma { get; set; }

        public virtual Kategori Kategori { get; set; }

        public virtual Uye Uye { get; set; }

        public virtual List<Yorum> Yorumlar { get; set; }

    }
}