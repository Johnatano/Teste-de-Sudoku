using System;
using System.Collections;

namespace Sudoku
{
//população sudoku
	public class Population
	{

		protected const int kLength = 9;
		protected const int kCrossover = kLength/2;
		protected const int kInitialPopulation = 1000;
		protected const int kPopulationLimit = 50;
		protected const int kMin = 1;
		protected const int kMax = 1000;
		protected const float  kMutationFrequency = 0.33f;
		protected const float  kDeathFitness = -1.00f;
		protected const float  kReproductionFitness = 0.0f;

		protected ArrayList Genomes = new ArrayList();
		protected ArrayList GenomeReproducers  = new ArrayList();
		protected ArrayList GenomeResults = new ArrayList();
		protected ArrayList GenomeFamily = new ArrayList();

		protected int		  CurrentPopulation = kInitialPopulation;
		protected int		  Generation = 1;
		protected bool	  Best2 = true;

		public Population()
		{
		//construtor
			for  (int i = 0; i < kInitialPopulation; i++)
			{
				SudokuGenome aGenome = new SudokuGenome(kLength, kMin, kMax);
				aGenome.SetCrossoverPoint(kCrossover);
				aGenome.CalculateFitness();
				Genomes.Add(aGenome);
			}

		}

		private void Mutate(Genome aGene)
		{
			if (SudokuGenome.TheSeed.Next(100) < (int)(kMutationFrequency * 100.0))
			{
			  	aGene.Mutate();
			}
		}

		public void NextGeneration()
		{
//incrementa a geração 
			Generation++; 


		// verifica quem pode acabar 
			for  (int i = 0; i < Genomes.Count; i++)
			{
				if  (((Genome)Genomes[i]).CanDie(kDeathFitness))
				{
					Genomes.RemoveAt(i);
					i--;
				}
			}


// determina a produção 
			GenomeReproducers.Clear();
			GenomeResults.Clear();
			for  (int i = 0; i < Genomes.Count; i++)
			{
				if (((Genome)Genomes[i]).CanReproduce(kReproductionFitness))
				{
					GenomeReproducers.Add(Genomes[i]);			
				}
			}
			
            // Faz o cruzamento dos genes e adicioná-los à população
			 DoCrossover(GenomeReproducers);

			Genomes = (ArrayList)GenomeResults.Clone();

			//para mudar a populaçaõ o gene
			for  (int i = 0; i < Genomes.Count; i++)
			{
				Mutate((Genome)Genomes[i]);
			}

            // Calcular adequação de todos os genes
			for  (int i = 0; i < Genomes.Count; i++)
			{
				((Genome)Genomes[i]).CalculateFitness();
			}

			//matar os metodos da população 
			if (Genomes.Count > kPopulationLimit)
				Genomes.RemoveRange(kPopulationLimit, Genomes.Count - kPopulationLimit);
			
			CurrentPopulation = Genomes.Count;
			
		}

		public  void CalculateFitnessForAll(ArrayList genes)
		{
			foreach(SudokuGenome lg in genes)
			{
			  lg.CalculateFitness();
			}
		}

		public void DoCrossover(ArrayList genes)
		{
			ArrayList GeneMoms = new ArrayList();
			ArrayList GeneDads = new ArrayList();

			for (int i = 0; i < genes.Count; i++)
			{
// escolher aleatoriamente "a mae e o pai "
				if (SudokuGenome.TheSeed.Next(100) % 2 > 0)
				{
					GeneMoms.Add(genes[i]);
				}
				else
				{
					GeneDads.Add(genes[i]);
				}
			}

		// Agora torná-los iguais
			if (GeneMoms.Count > GeneDads.Count)
			{
				while (GeneMoms.Count > GeneDads.Count)
				{
					GeneDads.Add(GeneMoms[GeneMoms.Count - 1]);
					GeneMoms.RemoveAt(GeneMoms.Count - 1);
				}

				if (GeneDads.Count > GeneMoms.Count)
				{
					GeneDads.RemoveAt(GeneDads.Count - 1); //verifica se nao estao iguais 
				}

			}
			else
			{
				while (GeneDads.Count > GeneMoms.Count)
				{
					GeneMoms.Add(GeneDads[GeneDads.Count - 1]);
					GeneDads.RemoveAt(GeneDads.Count - 1);
				}

				if (GeneMoms.Count > GeneDads.Count)
				{
                    GeneMoms.RemoveAt(GeneMoms.Count - 1); //verifica se nao estao iguais 
				}
			}

			//faz o cruzamento
			for (int i = 0; i < GeneDads.Count; i ++)
			{
			//pega o melhor entre a geração 
				SudokuGenome babyGene1 = (SudokuGenome)((SudokuGenome)GeneDads[i]).Crossover((SudokuGenome)GeneMoms[i]);
			    SudokuGenome babyGene2 = (SudokuGenome)((SudokuGenome)GeneMoms[i]).Crossover((SudokuGenome)GeneDads[i]);
			
				GenomeFamily.Clear();
				GenomeFamily.Add(GeneDads[i]);
				GenomeFamily.Add(GeneMoms[i]);
				GenomeFamily.Add(babyGene1);
				GenomeFamily.Add(babyGene2);
				CalculateFitnessForAll(GenomeFamily);
				GenomeFamily.Sort();

				if (Best2 == true)
				{
				// se acontecer adiciona fitness no topo
					GenomeResults.Add(GenomeFamily[0]);					
					GenomeResults.Add(GenomeFamily[1]);					

				}
				else
				{
					GenomeResults.Add(babyGene1);
					GenomeResults.Add(babyGene2);
				}
			}

		}

		public Genome GetHighestScoreGenome()
		{
			Genomes.Sort();
			return (Genome)Genomes[0];
		}

		public virtual void WriteNextGeneration()
		{
			//escreve no topo 
			Console.WriteLine("Generation {0}\n", Generation);
			if (Generation % 1  == 0) //Imprimir apenas a cada 100 gerações
			{
				Genomes.Sort();
				for  (int i = 0; i <  CurrentPopulation ; i++)
				{
					Console.WriteLine(((Genome)Genomes[i]).ToString());
				}

				Console.WriteLine("Hit the enter key to continue...\n");
				Console.ReadLine();
			}
		}
	}
}
