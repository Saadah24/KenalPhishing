using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KenalPhihsing.Models
{
    public class SecurityAlert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Severity { get; set; } // Tinggi, Sederhana, Rendah
        public DateTime DatePublished { get; set; }

        // TAMBAH DUA FIELD INI:
        public string ScammerInfo { get; set; }  // Contoh: No Telefon atau Link
        public string ExampleMessage { get; set; } // Teks mesej scam yang diterima

        public string Content { get; set; } // Ini akan jadi bahagian "Analisis Pakar"
    }
}