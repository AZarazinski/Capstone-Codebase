using MathNet.Numerics.Statistics;

namespace MadisonCountyCollaborationApplication.Pages.DataClasses
{
    public class Simulation
    {
        public DataClasses.Distribution AssignDistribution(Parameters distribution)
        {
            if (distribution.dist.Equals("Uniform"))
            {
                if (Convert.ToDouble(distribution.param1) < Convert.ToDouble(distribution.param2))
                {
                    return new DataClasses.Uniform(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param2));
                }
                else
                {
                    throw new InvalidOperationException("Min must be below Max");
                }
            }
            else if (distribution.dist.Equals("Triangular"))
            {
                if (Convert.ToDouble(distribution.param1) <= Convert.ToDouble(distribution.param2) &
                    Convert.ToDouble(distribution.param1) < Convert.ToDouble(distribution.param3))
                {
                    if (Convert.ToDouble(distribution.param3) <= Convert.ToDouble(distribution.param2))
                    {
                        return new DataClasses.Triangular(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param3),
                            Convert.ToDouble(distribution.param2));
                    }
                    else
                    {
                        throw new InvalidOperationException("Likely must be below the max");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Min must be below max and Likely");
                }

            }
            else if (distribution.dist.Equals("Normal"))
            {
                return new DataClasses.Normal(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param2));
            }
            else if (distribution.dist.Equals("Lognormal"))
            {
                return new DataClasses.Normal(new Random(), Convert.ToDouble(distribution.param1), Convert.ToDouble(distribution.param2));
            }
            else
            {
                return new DataClasses.Constant(new Random(), Convert.ToDouble(distribution.param1));
            }
        }
        public double GenerateResult(DataClasses.Distribution dist, double initial, string growth, double years)
        {
            if (growth.Equals("Constant"))
            {
                return initial * Math.Pow(1 + dist.GenerateRandom(), years);
            }
            else if (growth.Equals("Nonconstant"))
            {
                for (int i = 0; i < years; i++)
                {
                    initial *= 1 + dist.GenerateRandom();
                }
                return initial;
            }
            else
            {
                return dist.GenerateRandom();
            }
        }
        //non parametric bootstrap
        public double[] ConfidenceInterval(double[] Results, double significance)
        {
            double[] CI = new double[2];
            double[] bootstrap = new double[200];
            double[] sample = new double[Results.Length];
            Random rand = new Random();
            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < sample.Length; j++)
                {
                    sample[j] = Results[rand.Next(Results.Length)];
                }
                bootstrap[i] = sample.Mean();
            }
            double tau = (1 - significance) / 2;
            CI[0] = bootstrap.Quantile(tau);
            CI[1] = bootstrap.Quantile(significance + tau);
            return CI;
        }
    }

}
