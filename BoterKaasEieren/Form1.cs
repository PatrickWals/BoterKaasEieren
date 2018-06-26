using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BoterKaasEieren
{
    public partial class Form1 : Form
    {
        bool player1 = true; // variable used to define which player's turn it is
        int[] scoreArray = new int[2] {0,0};
        FlowLayoutPanel flp1;
        PictureBox[,] Field = new PictureBox[3, 3];
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=BKE.accdb";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createField();
            setField("XOXEEEOXO", 5, 4);
        }

        private void Field_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;


            
            if (pb.Tag.ToString() == "E") // a turn is only Valid when the Tag of a PictureBox is empty
            {
                
                Console.WriteLine("ClicK");
                if (player1)
                {
                    pb.BackColor = Color.Red;
                    pb.Tag = "X";// set the tag to X
                    player1 = false;// end of player 1's turn
                    checkWinner("X");//this checks if player X has won 
                }
                else
                {
                    pb.BackColor = Color.Green;
                    pb.Tag = "O";// set the tag to O
                    player1 = true;// end of player 2's turn
                    checkWinner("O");//this checks if player O has won                  
                }

                checkDraw();
                saveSnapshot();
                loadSnapshot();

            }
            
            //Used for Testing purposes
            //foreach (var item in Field)
            //{
            //    Console.Write(item.Tag);
            //}
            //Console.WriteLine();
        }

        private void createField()
        {
            // this creates a flowLayoutPanel to contain the PictureBoxes
            flp1 = new FlowLayoutPanel();
            flp1.Location = new Point(0, 0);
            flp1.Size = new Size(this.Width, this.Height-15);
            this.Controls.Add(flp1);

            //This code creates an 2d Array and fills it with picture boxes.

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Field[i, j] = new PictureBox();
                    Field[i, j].Name = "R" + i + "C" + j;// set the name Row I Collumn J
                    Field[i, j].Size = new Size(flp1.Width / 3 - 10, flp1.Height / 3 - 10);
                    Field[i, j].BackColor = Color.White;
                    Field[i, j].Tag = "E"; // the tag is used to check if the Picture box has been checked
                    Field[i, j].Click += new EventHandler(Field_Click);// this code adds a click eventhandler
                    Console.Write(Field[i, j].Name + " ");//Used for Testing purposes

                    flp1.Controls.Add(Field[i, j]);// this code adds the Picturebox to the flowlayoutPanel
                }
                Console.WriteLine("");
            }
        }

        private void checkWinner(string ltr)
        {
            for (int i = 0; i < 3; i++)
            {
                //this checks for all three in a row
                if (Field[i, 0].Tag.ToString() == ltr && Field[i, 1].Tag.ToString() == ltr && Field[i, 2].Tag.ToString() == ltr)
                {
                    Console.WriteLine("Win " + ltr);                    
                    endWin(ltr);
                }

                //this checks for all three in a collumn
                if (Field[0, i].Tag.ToString() == ltr && Field[1, i].Tag.ToString() == ltr && Field[2, i].Tag.ToString() == ltr)
                {
                    Console.WriteLine("Win " + ltr);
                    endWin(ltr);
                }
            }

            //this checks for all three in diagonals
            if (Field[0, 0].Tag.ToString() == ltr && Field[1, 1].Tag.ToString() == ltr && Field[2, 2].Tag.ToString() == ltr)
            {
                Console.WriteLine("Win "+ltr);
                endWin(ltr);
            }

            if (Field[0, 2].Tag.ToString() == ltr && Field[1, 1].Tag.ToString() == ltr && Field[2, 0].Tag.ToString() == ltr)
            {
                Console.WriteLine("Win " + ltr);
                endWin(ltr);
            }
        }
        
        private void checkDraw()
        {   
            //this array is used to check if the game board is full
            //counter is used to fill the array
            string[] drawArray = new string[9];
            int counter = 0;

            //every Tag property into drawArray
            foreach (var item in Field)
            {
                drawArray[counter] = item.Tag.ToString();
                counter += 1;
            }
            counter = 0;
            
            //if all the squares are filled then     
            if (!drawArray.Contains<string>("E"))
            {
                Console.WriteLine("Draw");
                endDraw();
            }
        }

        private void endWin(string ltr)
        {
            string Winner = ltr;
            if (ltr == "X")
            {
                scoreArray[0] += 1;
            } else {
                scoreArray[1] += 1;
            }

            MessageBox.Show("Player " + Winner +" Won!", "Congratulations!", MessageBoxButtons.OK);
            MessageBox.Show("X "+scoreArray[0]+"-"+scoreArray[1]+ " O", "Score", MessageBoxButtons.OK);

            string text = "Do you want to play another game?";
            string caption = "Player";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            
            result = MessageBox.Show(text, caption, buttons);
            
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                 resetField();
            }
            if (result == System.Windows.Forms.DialogResult.No)
            {
                this.Close();
            }
        }

        private void endDraw()
        {

            MessageBox.Show("it's a Draw", "Draw", MessageBoxButtons.OK);

            string text = "Do you want to play another game?";
            string caption = "Player";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(text, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                resetField();
            }
            else //(result == System.Windows.Forms.DialogResult.No)
            {
                this.Close();
            }
        }

        private void resetField()
        {
            player1 = true;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Field[i, j].Tag = "E";
                    Field[i, j].BackColor = Color.White;
                }
            }
        }

        private void saveSnapshot()
        {
            string status = getStatus();


            OleDbConnection connection = new OleDbConnection(connectionString);
            try
            {
                connection.Open();
                OleDbCommand cmd = connection.CreateCommand();

                cmd.Parameters.Add(new OleDbParameter("@status", status));
                cmd.Parameters.Add(new OleDbParameter("@scoreP1", scoreArray[0]));
                cmd.Parameters.Add(new OleDbParameter("@status", scoreArray[1]));

                cmd.CommandText = "INSERT INTO BKE (gameID, gamestatus, scoreP1, scoreP2 )" +
                               "VALUES (1, @status, @scoreP1, @scoreP2 );";

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }
    
        private void loadSnapshot()
        {

            Console.WriteLine(getGamestate());

        }

        private void setField(string status, int P1, int P2)
        {
            int counter = 0;
            scoreArray[0] = P1;
            scoreArray[1] = P2;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write("I"+i +"J"+ j+" ");
                }
                Console.WriteLine();
                 
            }

            

        }

        private string getStatus()
        {
            string status = "";
            foreach (var item in Field)
            {
                status += item.Tag;
            }
            //Console.WriteLine(status);
            return status;  
        }
    }
}
