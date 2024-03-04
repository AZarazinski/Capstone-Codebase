using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Collaboration
    {

        public int collabID { get; set; }
        [Required(ErrorMessage = "Collaboration Name is Required")]
        public String? collabName { get; set; }

        public String? notesAndInfo { get; set; }
        [Required(ErrorMessage = "Date is Required")]

        public DateTime? dateCreated { get; set; }
        [Required(ErrorMessage = "Privacy Type is Required")]

        public String? collabType { get; set; }

    }
}
