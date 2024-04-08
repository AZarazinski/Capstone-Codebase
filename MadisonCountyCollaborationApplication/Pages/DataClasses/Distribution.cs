using System.Reflection.Metadata;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public abstract class Distribution
    {
        public Random rand;
        public Distribution(Random random)
        {
            rand = random;
        }
        public abstract double GenerateRandom();
        
        public abstract string PrintType();
        
    }
}
