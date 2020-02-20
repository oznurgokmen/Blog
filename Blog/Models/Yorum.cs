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
    [Table("Yorumlar")]

    public class Yorum
    {
        public int Id { get; set; }

        [DisplayName("Yorum")]

        [Required(ErrorMessage = "Lütfen Yorum İçeriğini Giriniz !")]

        public string YorumIcerik { get; set; }

        public DateTime Tarih { get; set; }

        public int UyeId { get; set; }

        public int YaziId { get; set; }

        public virtual Uye Uye { get; set; }

        public virtual Yazi Yazi { get; set; }
    }
}