using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    [Table("Kategoriler")]

    public class Kategori
    {
        [DisplayName("Kategori Id")]
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen Kategori Adı Giriniz !")]

        [DisplayName("Kategori Adı")]

        [StringLength(50, ErrorMessage = "Kategori Adı En Fazla 50 Karakter Uzunluğunda Olmalıdır !")]

        public string KategoriAdi { get; set; }

        public virtual List<Yazi> Yazilar { get; set; }

    }
}