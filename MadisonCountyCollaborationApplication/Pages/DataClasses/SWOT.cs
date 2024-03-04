using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class SWOT
    {

        public int swotID { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        public String? title { get; set; }
        [Required(ErrorMessage = "Category is Required")]
        public String? category { get; set; }
        [Required(ErrorMessage = "A Strength is Required")]
        public String? strengths { get; set; }
        [Required(ErrorMessage = "A Weakness is Required")]
        public String? weaknesses { get; set; }
        [Required(ErrorMessage = "An Opportunity is Required")]
        public String? opportunities { get; set; }
        [Required(ErrorMessage = "A Threat is Required")]
        public String? threats { get; set; }
        [Required(ErrorMessage = "Date is Required")]
        public DateTime? swotDate { get; set; }
        public String? author { get; set; }

    }
}
