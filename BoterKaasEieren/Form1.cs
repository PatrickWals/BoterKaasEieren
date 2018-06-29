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
        string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\Databae_BKE.mdb";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createField();
            MessageBox.Show("S = Savegame    L = loadgame");
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
            }
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
                Console.WriteLine();
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
            OleDbConnection connection = new OleDbConnection(connectionString);
            string Query = "UPDATE BKE SET "+
                "A1 = @A1, " +
                "A2 = @A2, " +
                "A3 = @A3, " +
                "B1 = @B1, " +
                "B2 = @B2, " +
                "B3 = @B3, " +
                "C1 = @C1, " +
                "C2 = @C2, " +
                "C3 = @C3" ;

            try
            {
                connection.Open();
                OleDbCommand cmd = new OleDbCommand(Query, connection);

                cmd.Parameters.Add(new OleDbParameter("@A1", Field[0, 0].Tag.ToString()));
                cmd.Parameters.Add(new OleDbParameter("@A2", Field[0, 1].Tag.ToString()));
                cmd.Parameters.Add(new OleDbParameter("@A3", Field[0, 2].Tag.ToString()));

                cmd.Parameters.Add(new OleDbParameter("@B1", Field[1, 0].Tag.ToString()));
                cmd.Parameters.Add(new OleDbParameter("@B2", Field[1, 1].Tag.ToString()));
                cmd.Parameters.Add(new OleDbParameter("@B3", Field[1, 2].Tag.ToString()));

                cmd.Parameters.Add(new OleDbParameter("@C1", Field[2, 0].Tag.ToString()));
                cmd.Parameters.Add(new OleDbParameter("@C2", Field[2, 1].Tag.ToString()));
                cmd.Parameters.Add(new OleDbParameter("@C3", Field[2, 2].Tag.ToString()));

                cmd.ExecuteNonQuery();
                {
                    Console.WriteLine("Game Saved!");
                }
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
            setField();
            setFieldColor();
        }

        private void setField()
        {
            string query = "SELECT * FROM BKE";
            OleDbConnection connection = new OleDbConnection(connectionString);

            try
            {
                connection.Open();

                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Field[0, 0].Tag = reader[0].ToString();
                    Field[0, 1].Tag = reader[1].ToString();
                    Field[0, 2].Tag = reader[2].ToString();

                    Field[1, 0].Tag = reader[3].ToString();
                    Field[1, 1].Tag = reader[4].ToString();
                    Field[1, 2].Tag = reader[5].ToString();

                    Field[2, 0].Tag = reader[6].ToString();
                    Field[2, 1].Tag = reader[7].ToString();
                    Field[2, 2].Tag = reader[8].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Game Loaded");
                connection.Close();
            }


        }

        private void setFieldColor()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Field[i,j].Tag.ToString() == "X")
                    {
                        Field[i,j].BackColor = Color.Red;
                    }
                    if (Field[i, j].Tag.ToString() == "O")
                    {
                        Field[i,j].BackColor = Color.Green;
                    }
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar.ToString().ToLower()== "s")
            {
                saveSnapshot();
            }
            if (e.KeyChar.ToString().ToLower() == "l")
            {
                loadSnapshot();
            }
        }
    }
}
