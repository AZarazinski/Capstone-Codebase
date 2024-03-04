using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class KnowledgeItems
    {

        public int knowledgeItemID { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        public String? title { get; set; }
        [Required(ErrorMessage = "Subject is Required")]
        public String? KISubject { get; set; }
        [Required(ErrorMessage = "Category is Required")]
        public String? category { get; set; }
        [Required(ErrorMessage = "Information is Required")]
        public String? information { get; set; }
        [Required(ErrorMessage = "Date is Required")]
        public DateTime? KMDate { get; set; }
        public String? author { get; set; }

    }
}
