using MadisonCountyCollaborationApplication.Pages.DataClasses;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Constant: Distribution
    {
        public double constant;
        public Random rand;
        //public Constant(double value, Random rand) : base(rand)
        //{
        //    constant = value;
        //}

        public Constant(Random rand, double value) : base(rand)
        {
            constant = value;
            this.rand = rand;
        }

        public override double GenerateRandom()
        {
            return constant;
        }
        public override string PrintType()
        {
            return "Constant";
        }
    }
}
