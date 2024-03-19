namespace _488Labs.Pages.DataClasses
{
    public class Constant: Distribution
    {
        private double constant;
        private Random rand;
        //public Constant(double value, Random rand) : base(rand)
        //{
        //    constant = value;
        //}

        public Constant(Random rand, double value) : base(rand)
        {
            constant = value;
            this.rand = rand;
        }

        public double GenerateRandom()
        {
            return constant;
        }
    }
}
