using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class UserProcess
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int ProcessID { get; set; }
    }
}
