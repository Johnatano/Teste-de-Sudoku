using System;
using System.Collections;

namespace Sudoku
{
	//genoma
	public class SudokuGenome : Genome
	{
		protected int[,] TheArray = new int[9,9];
		public static Random TheSeed = new Random((int)DateTime.Now.Ticks);
		protected int TheMin = 0;
		protected int TheMax = 100;

		public int this [int index1, int index2]
		{
			get 
			{
				return TheArray[index1, index2];
			}
		}

		public override int CompareTo(object a)
		{
			SudokuGenome Gene1 = this;
			SudokuGenome Gene2 = (SudokuGenome)a;
			return Math.Sign(Gene2.CurrentFitness  -  Gene1.CurrentFitness);
		}


		public override void SetCrossoverPoint(int crossoverPoint)
		{
			CrossoverPoint = 	crossoverPoint;
		}

		public SudokuGenome()
		{

		}


		public SudokuGenome(long length, object min, object max)
		{
			
			// construtores
			
			Length = length;
			TheMin = (int)min;
			TheMax = (int)max;
			for (int i = 0; i < 9; i++)
			{
				int offset = TheSeed.Next(9);
				for (int j = 0; j < 9; j++)
				{
					TheArray[j,i] = (int)GenerateGeneValue(1, 9);
				}
			}
		}

		public override void Initialize()
		{

		}

		public override bool CanDie(float fitness)
		{
			if (CurrentFitness <= (int)(fitness * 100.0f))
			{
				return true;
			}

			return false;
		}


		public override bool CanReproduce(float fitness)
		{
			if (SudokuGenome.TheSeed.Next(100) >= (int)(fitness * 100.0f))
			{
				return true;
			}

			return false;
		}


		public override object GenerateGeneValue(object min, object max)
		{
			return TheSeed.Next((int)min, (int)max + 1);
		}

		public override void Mutate()
		{
			int MutationIndex1 = TheSeed.Next((int)9);
			int MutationIndex2 = TheSeed.Next((int)9);
			int MutationIndex3 = TheSeed.Next((int)9);

			if (TheSeed.Next(2) == 1)
			{
				TheArray[MutationIndex1, MutationIndex2] = MutationIndex3 + 1;
			}
			else
			{
				int temp = 0;
			
				if (TheSeed.Next(2) == 1)
				{
			
					temp = TheArray[MutationIndex1, MutationIndex2];
					TheArray[MutationIndex1, MutationIndex2] = TheArray[MutationIndex3, MutationIndex2];
					TheArray[MutationIndex3, MutationIndex2] = temp;

				}
				else
				{
			
					temp = TheArray[MutationIndex2, MutationIndex1];
					TheArray[MutationIndex2, MutationIndex1] = TheArray[MutationIndex2, MutationIndex3];
					TheArray[MutationIndex2, MutationIndex3] = temp;
				}
			}

		

		}


		Hashtable RowMap = new Hashtable();
		Hashtable ColumnMap = new Hashtable();
		Hashtable SquareMap = new Hashtable();


		
		///// quadro 3 por 3 
		
		private float CalculateSudokuFitness()
		{
			// define as linahs e paroes como 0 
			float fitnessColumns = 0;
			float fitnessRows = 0;
			float fitnessSquares = 0;

			// passa por cada coluna
			for (int i = 0; i < 9; i++)
			{
				
			//valor na celular
				ColumnMap.Clear();
				for (int j = 0; j < 9; j++)
				{
					 // verificar a linha
					if (ColumnMap[TheArray[i,j]] == null)
					{
						ColumnMap[TheArray[i,j]] = 0;
					}

					 ColumnMap[TheArray[i,j]] = ((int)ColumnMap[TheArray[i,j]]) + 1;
				}

				
				fitnessColumns += (float)(1.0f/ (10-ColumnMap.Count))/9.0f;
				
			}

			// passa por cada linha 
			for (int i = 0; i < 9; i++)
			{
				//passa o valro para a celula
				RowMap.Clear();
				for (int j = 0; j < 9; j++)
				{
				//verifica a linha 
					if (RowMap[TheArray[j,i]] == null)
					{
						RowMap[TheArray[j,i]] = 0;
					}

					RowMap[TheArray[j,i]] = ((int)RowMap[TheArray[j,i]]) + 1;
				}

	//entrada na linha
				fitnessRows += (float)(1.0f/ (10-RowMap.Count))/9.0f;

			}

//passa para o proximo quadro 
			for (int l = 0; l < 3; l++)
			{
				for (int k = 0; k < 3; k++)
				{
					//passa por cada celular
					SquareMap.Clear();
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							//verifica a linha
							if (SquareMap[TheArray[i + k*3,j + l*3]] == null)
							{
								SquareMap[TheArray[i+k*3,j+l*3]] = 0;
							}

			
							SquareMap[TheArray[i + k*3,j + l*3]] = ((int)SquareMap[TheArray[i + k*3,j + l*3]]) + 1;
						}
					}

					fitnessSquares += (float)(1.0f/ (10-SquareMap.Count))/9.0f;
				}

			}

			// linha  fitness 3x3 s
			CurrentFitness = fitnessColumns * fitnessRows * fitnessSquares;

			return CurrentFitness;
		}


		public override float CalculateFitness()
		{
			CalculateSudokuFitness();

			return CurrentFitness;
		}

		public override string ToString()
		{
			string strResult = "";
			for (int j = 0; j < Length; j++)
			{
				for (int i = 0; i < Length; i++)
				{
					strResult = strResult + ((int)TheArray[i, j]).ToString() + " ";
				}
				strResult += "\r\n";
			}

			strResult += "-->" + CurrentFitness.ToString();

			return strResult;
		}



		public override void CopyGeneInfo(Genome dest)
		{
			SudokuGenome theGene = (SudokuGenome)dest;
			theGene.Length = Length;
			theGene.TheMin = TheMin;
			theGene.TheMax = TheMax;
		}


		public override Genome Crossover(Genome g)
		{
			SudokuGenome aGene1 = new SudokuGenome();
			SudokuGenome aGene2 = new SudokuGenome();
			g.CopyGeneInfo(aGene1);
			g.CopyGeneInfo(aGene2);


			SudokuGenome CrossingGene = (SudokuGenome)g;
			if (TheSeed.Next(2) == 1)
			{
				for (int j = 0; j < 9; j++)
				{
					CrossoverPoint = TheSeed.Next(8) + 1;
					for (int k = 0; k < CrossoverPoint; k++)
					{
						aGene1.TheArray[k,j] = CrossingGene.TheArray[k, j];
						
						aGene2.TheArray[k ,j] = TheArray[k, j];
						
					}

					for (int k = CrossoverPoint; k < 9; k++)
					{
						aGene2.TheArray[k,j] = CrossingGene.TheArray[k, j];
						
						aGene1.TheArray[k ,j] = TheArray[k, j];
						
					}
				}
			}
			else
			{
				for (int j = 0; j < 9; j++)
				{
					CrossoverPoint = TheSeed.Next(8) + 1;
					for (int k = 0; k < CrossoverPoint; k++)
					{
						aGene1.TheArray[j,k] = CrossingGene.TheArray[j, k];
						
						aGene2.TheArray[j ,k] = TheArray[j, k];
						
					}

					for (int k = CrossoverPoint; k < 9; k++)
					{
						aGene2.TheArray[j,k] = CrossingGene.TheArray[j, k];
						
						aGene1.TheArray[j ,k] = TheArray[j, k];
						
					}
				}
			}
	

		// verifica gene
			SudokuGenome aGene = null;
			if (TheSeed.Next(2) == 1)			
			{
				aGene = aGene1;
			}
			else
			{
				aGene = aGene2;
			}

			return aGene;
		}

	}
}
