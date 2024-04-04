namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class WhiteListService
    {
        private List<string> _whitelist;

        public WhiteListService()
        {
            // Initialize the whitelist with some default values
            _whitelist = new List<string> { ".csv", ".doc", ".docx", ".pdf", ".png", ".pdf", ".jpg", ".jpeg", ".txt" };
        }

        public List<string> GetWhitelist()
        {
            return _whitelist;
        }

        public void SetWhitelist(List<string> whitelist)
        {
            _whitelist = whitelist;
        }
    }
}
