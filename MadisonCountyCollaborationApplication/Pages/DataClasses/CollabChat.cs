using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class CollabChat
    {

        [Required(ErrorMessage = "Information is Required")]

        public String? messageInfo { get; set; }
        [Required(ErrorMessage = "Time is Required")]

        public DateTime? messageTime { get; set; }
        public int userID { get; set; }
        public int collabID { get; set; }

    }
}
