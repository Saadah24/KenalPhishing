using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KenalPhihsing.Models
{
    public class ModulePage
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int PageNumber { get; set; }
        public string PageType { get; set; } // "Content", "Simulation", "Quiz"
        public string PageTitle { get; set; }

        public string BodyContent { get; set; }

        // --- TAMBAHAN UNTUK ENGINE SIMULASI ---
        public string SimSender { get; set; }    // Contoh: "Maybank Official"
        public string SimSubject { get; set; }   // Contoh: "Akaun anda disekat!"
        public string SimBody { get; set; }      // Contoh: "Sila sahkan maklumat..."
        public string SimPhishLink { get; set; } // Contoh: "http://maybank-secure.com"
        public string SimClues { get; set; }     // Contoh: "Semak URL tu betul-betul!"
        public string SimType { get; set; } // "Financial", "Social", "Webmail", "Government"

        // --- Kuiz Kekal ---
        public string QuizQuestion { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; } // Simpan "A", "B", "C", atau "D"

        public virtual Module Module { get; set; }
    }
}