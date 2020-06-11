using System.Numerics;

namespace ApplicationCore.Helpers
{
    public class NewtonResult
    {
        public SolutionStatus Status { get; set; }
        
        /// <summary>
        /// The solution found
        /// </summary>
        public Complex Solution { get; set; }
        
        /// <summary>
        /// The solution right before the method converged. It can be used to determine how fast it converged
        /// </summary>
        public Complex PreviousSolution { get; set; }
        
        /// <summary>
        /// The number of iterations needed to find the solution
        /// </summary>
        public int Iterations { get; set; }
    }
}