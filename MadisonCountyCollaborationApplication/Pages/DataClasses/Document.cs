using Microsoft.CodeAnalysis;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Document
    {
        public int documentID { get; set; }
        public string documentName { get; set; }
        public string displayDocName { get; set; }
        public string documentType { get; set; }
        public DateTime dateCreated { get; set; }
        public int userID { get; set; }
        public string userFullName {  get; set; }
    }
}
