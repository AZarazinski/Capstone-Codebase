using MadisonCountyCollaborationApplication.Pages.DB;
using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Process
    {
        public int ProcessID { get; set; }

        [Required(ErrorMessage = "Collaboration Name is Required")]
        public string ProcessName { get; set; }

        public string NotesAndInfo { get; set; }




    }
}
