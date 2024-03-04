using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Plans
    {

        public int planID { get; set; }
        [Required(ErrorMessage = "Plan Name is Required")]

        public String? planName { get; set; }
        [Required(ErrorMessage = "Plan Description is Required")]

        public String? planDesc { get; set; }
        [Required(ErrorMessage = "Date is Required")]

        public DateTime? dateCreated { get; set; }

        public int collabID { get; set; }

    }
}
