using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Documents
    {

        public int DocumentID { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public String? DocumentName { get; set; }
        [Required(ErrorMessage = "Content is Required")]
        public byte[] Content { get; set; }

    }
}
