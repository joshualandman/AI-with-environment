using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	/* Individual class implements a single Individual from a population.
	 * In addition to being the base type for the Population array, it is
	 * used to pass Individuals around in Population and ThreshPop, but it
	 * is not used by the Main method.  Contains the mutation operator, since
 	 * mutation only involves a single individual.
	 */
	public class Individual
	{
		const float MUT_PROB = 0.2f;	// Mutation probability
		int fitness;
		uint chrom;		// up to 32-bit chromosome with an unsigned integer
		int nBits;		// Number of bits actually used (starting w/ least sig)

		// Called by InitPop() with no fitness value
		public Individual (uint newChrom, int nB)
		{
			chrom = newChrom;
			nBits = nB;
			fitness = 1;	// Default fitness must be non-zero
		}

		// Overload called by ReadPop() with fitness from last generation
		public Individual (uint newChrom, int nB, int fit)
		{
			chrom = newChrom;
			nBits = nB;
			fitness = fit;
		}

		// Getters and a setter
		public uint Chrom
		{
			get { return this.chrom; }
		}

		public int Fitness
		{
			get { return this.fitness; }
			set { this.fitness = value; }
		}

		// Mutates a random bit MUT_PROB of the time
		public void Mutate ()
		{
			if (Util.rand.NextDouble() < MUT_PROB)
			{
				int mutPt = Util.rand.Next(0, nBits);	// 0 to nBits - 1
				int mutMask = 1 << mutPt;		// Build mask of 1 at mutation point
				chrom = chrom ^ (uint)mutMask;	// xor the mask, which flips that bit
			}
		}

		// Make it easier to write an Individual
		public override string ToString()
		{
			return (chrom + " " + fitness);
		}
	}