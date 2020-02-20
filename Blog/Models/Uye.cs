using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    [Table("Uyeler")]

    public class Uye
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Adı Giriniz !")]

        [DisplayName("Kullanıcı Adı")]

        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "E-Posta Giriniz !")]

        [DisplayName("E-Posta")]
        [EmailAddress(ErrorMessage = "Lütfen E-Posta Formatıyla Giriş Yapınız !")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola Giriniz !")]
        [StringLength(100, ErrorMessage = "{0} En Az {2} Karakter Uzunluğunda Olmalıdır !", MinimumLength = 6)]
        [DataType(DataType.Password)]       
        [DisplayName("Parola")]
        public string Sifre { get; set; }

        public string VCode { get; set; }

        [Required(ErrorMessage = "Adı Soyadı Giriniz !")]

        [DisplayName("Adı Soyadı")]

        public string AdSoyad { get; set; }

        [DisplayName("Üye Statüsü")]

        public int UyeAdmin { get; set; }

        public virtual List<Yazi> Yazilar { get; set; }

        public virtual List<Yorum> Yorumlar { get; set; }
    }
}