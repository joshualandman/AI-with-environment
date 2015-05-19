using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

	/* Population class implements a single population. Used by ThreshPop for both
	 * oldP and newP. 
	 */
	public class Population
	{
		const double CROSSOVER_PROB = 0.9;	// 90% chance of crossover in BreedDude()
		int popSize;			// Population size
		int nBits;           	// Number of bits per chromosome
		int nChromVals;      	// Number of different chromosome values (2 to the nBits)
		Individual [] dudes;	// Array of Individuals
		int nDudes = 0;			// Current number of Individuals
		int totFit = 0;      	// Total fitness for all individuals in population
		char[] delim = {' '};	// Used in ReadPop to split input lines

		// Constructor sets up an empty population with popN individuals
		//   and chromosome length of cLeng (sets iBits)
		public Population (int popN, int cLeng)
		{
			popSize = popN;
			dudes = new Individual[popSize];
			nDudes = 0;
			nBits = cLeng;
			nChromVals = 1 << nBits;
			totFit = 0;
		}

		// Returns true if population is full
		public bool Full
		{
			get { return nDudes == popSize; }
		}

		// Fills population with new random chromosomes for generation 0
		public void InitPop()
		{
			for (int i = 0; i < popSize; i++)
			{
				dudes[i] = new Individual ((uint) Util.rand.Next (nChromVals), nBits);
			}
			nDudes = popSize;
			totFit = popSize;      // Default fitness for each Individual == 1
		}

		// Fills population by reading individuals from a file already opened to inStream
		// Assumes file is correctly formatted with correct number of lines
		public void ReadPop(StreamReader inStream)
		{
			for (int i = 0; i < popSize; i++)
			{
				string line = inStream.ReadLine();		// Read a line
				string [] tokens = line.Split (delim);	// Split into "words"
				uint chr = UInt32.Parse(tokens[0]);		// Convert words to numbers
				int fit = int.Parse(tokens[1]);
				dudes [i] = new Individual (chr, nBits, fit); // Put Individual in population
				totFit += fit;							// Accumulate total fitness for selection
			}
			nDudes = popSize;							// Show the population full
		}

		// Write the population out to a data file that can be read by ReadPop
		public void WritePop(StreamWriter outStream)
		{
			for (int i = 0; i < nDudes; i++)
			{
				outStream.WriteLine (dudes[i]);
			}
		}

		// Display the Population on the Console
		public void DisplayPop()
		{
			for (int i = 0; i < nDudes; i++)
			{
				Console.WriteLine (dudes [i]);
			}
			Console.WriteLine ();
		}

		// Breed a new Individual using crossover and mutation
		public Individual BreedDude()
		{
			Individual p1 = Select ();					// Get 2 parents
			Individual p2 = Select ();
			uint c1 = p1.Chrom;							// Extract their chromosomes
			uint c2 = p2.Chrom;

			if (Util.rand.NextDouble () < CROSSOVER_PROB) // Probably do crossover
			{
				uint kidChrom = CrossOver (c1, c2);		// Make new chromosome
				Individual newDude = new Individual (kidChrom, nBits); // Make Individual
				newDude.Mutate ();						// Maybe mutate a bit
				return newDude;							// Send it back
			}
			else
				// No crossover => Pick one of the parents to return unchanged
				return (Util.rand.NextDouble() < 0.5 ? p1 : p2);
		}

		// Roulette-wheel selection selects in linear proportion to fitness
		// Uses totFit, which was accumulated when population was filled
		public Individual Select()
		{
			// Roll a random integer from 0 to totFit - 1
			int roll = Util.rand.Next (totFit);

			// Walk through the population accumulating fitness
			int accum = dudes[0].Fitness;	// Initialize to the first one
			int iSel = 0;
			// until the accumulator passes the rolled value
			while (accum <= roll && iSel < nDudes-1)
			{
				iSel++;
				accum += dudes[iSel].Fitness;
			}
			// Return the Individual where we stopped
			return dudes[iSel];
		}

		// Find the best (highest fitness) Individual in the population
		// Used to implement elitism => best of old Pop survives in new
		public Individual BestDude ()
		{
			// Initialize to the first Individual in the array
			int whereBest = 0;			// Initialze to the first one
			int bestFit = dudes[0].Fitness;

			// Walk through the rest to get the overall best one
			for (int i = 1; i < nDudes; i++)
				if (dudes [i].Fitness > bestFit)
				{
					whereBest = i;
					bestFit = dudes [i].Fitness;
				}
			return dudes[whereBest];
		}

		// Add a new Individual to the population in the next open spot
		public int AddNewInd (Individual newDude)
		{
			int wherePut = -1;			// -1 in case something breaks
			if (Full)
				Console.WriteLine ("Panic!  Tried to add too many dudes");
			else
			{
				wherePut = nDudes;
				dudes[wherePut] = newDude;
				nDudes++;				// Increment for next time
			}
			return wherePut;			// Return offset in array where it landed
		}

		// Get Individual at offset where in the array
		public Individual GetDude (int where)
		{
			return dudes [where];
		}

		// Set fitness of Individual at offset where to fitVal
		public void SetFitness (int where, int fitVal)
		{
			dudes[where].Fitness = fitVal;
		}

		// Single-point crossover of two parents, returns new kid
		// Uses bit shift tricks to get each parent's contribution on opposite
		// sides of random crossover point
		uint CrossOver(uint p1, uint p2)
		{
			int xOverPt = Util.rand.Next (0, nBits);	// Pick random crossover point
			p1 = (p1 >> xOverPt) << xOverPt;			// Get p1's bits to the left
			p2 = (p2 << (32 - xOverPt)) >> (32 - xOverPt); // p2's to the right
			uint newKid = p1 | p2;						// Or them together
			return newKid;
		}
	}
