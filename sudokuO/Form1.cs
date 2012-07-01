using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

namespace Sudoku
{
	/// descriçao para form 1
	public class Form1 : System.Windows.Forms.Form
	{
		
		///design
		
		private System.ComponentModel.Container components = null;

		bool _threadFlag = false;
		public Form1()
		{
		
			InitializeComponent();


            // execução em paralelo
			_oThread = new Thread(new ThreadStart(GenomeThread));
			_oThread.Start();

			Invalidate();
			
			
		}

		private System.Windows.Forms.StatusBar statusBar1;

		Thread _oThread = null;

		void GenomeThread()
		{
			CalculateGeneration(1000, 100000);
		}

		int ToPercent (float val)
		{
			return (int)(val * 100);
		}

	    Genome _gene = null;
		public void CalculateGeneration(int nPopulation, int nGeneration)
		{
			int _previousFitness = 0;
			Population TestPopulation = new Population();// criando a população
			
			for (int i = 0; i < nGeneration; i++)
			{
				if (_threadFlag)
					break;
				TestPopulation.NextGeneration();
				Genome g = TestPopulation.GetHighestScoreGenome();

				if (i % 100 == 0)
				{
					Console.WriteLine("Generation #{0}", i);
					if (  ToPercent(g.CurrentFitness) != _previousFitness)
					{
						Console.WriteLine(g.ToString());
						_gene = g;
						statusBar1.Text = String.Format("Current Fitness = {0}", g.CurrentFitness.ToString("0.00"));
						this.Text = String.Format("Sudoko Grid - Generation {0}", i);
						Invalidate();
						_previousFitness = ToPercent(g.CurrentFitness);
					}

					if (g.CurrentFitness > .9999)
					{
						Console.WriteLine("Final Solution at Generation {0}", i);
						statusBar1.Text = "Finished";
						Console.WriteLine(g.ToString());
						break;
					}
				}

			} 

			
		}


		//para limprar o que esta sendo utilizado
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		//para suporat o metodo
		private void InitializeComponent()
		{
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.SuspendLayout();
            
            // // para mostrar a barra de status 
            
            this.statusBar1.Location = new System.Drawing.Point(0, 326);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(352, 22);
            this.statusBar1.TabIndex = 0;
            this.statusBar1.Text = "Ready...";
            
            // primeiro form
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(352, 348);
            this.Controls.Add(this.statusBar1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);

		}
		#endregion

		//ponto de entrada da minha aplicação
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		Pen _penThick = new Pen(Color.Black, 3);
		Font _sudukoFont = new Font("Arial", 16, FontStyle.Regular);

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// para desenha o meu quadrado
			Graphics g = e.Graphics;

			Rectangle r = ClientRectangle;
			r.Inflate(-statusBar1.Height -2, -statusBar1.Height - 2);
			g.DrawRectangle(_penThick, r);

			int spacingX = r.Width/9;
			int spacingY = r.Height/9;
			for (int i = 0; i < 9; i++)
			{
				if (i % 3 == 0)
				{
					g.DrawLine(_penThick, r.Left, r.Top + spacingY * i, r.Right, r.Top + spacingY*i);
					g.DrawLine(_penThick, r.Left + spacingX * i, r.Top, r.Left + spacingX * i, r.Bottom);
				}
				else
				{
					g.DrawLine(Pens.Black, r.Left, r.Top + spacingY * i, r.Right, r.Top + spacingY*i);
					g.DrawLine(Pens.Black, r.Left + spacingX * i, r.Top, r.Left + spacingX * i, r.Bottom);
				}
			}

			for (int i = 0; i < 9; i++)
				for (int j = 0; j < 9; j++)
				{
					if (_gene != null)
					{
						int val = (_gene as SudokuGenome)[i, j];
						g.DrawString(val.ToString(), _sudukoFont, Brushes.Black, r.Left + i*spacingX + 5, r.Top + j*spacingY + 5, new StringFormat());
					}
				}

		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_threadFlag = true;
		}

        private void Form1_Load(object sender, EventArgs e)
        {

        }
	}
}
