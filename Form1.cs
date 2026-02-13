using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLogic;

namespace GalacticGuard
{
    public partial class Form1 : Form

    {
        int playerX = 350; // Initial position
        int playerY = 440;
        int moveSpeed = 15;
        bool IsLeftPressed = false;
        bool IsRightPressed = false;
        int score = 0;
        bool IsGameOver=false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;


            //Making thread and tell him that run MoveEnemies mehtod
            Thread enemyThread = new Thread(MoveEnemies);
            enemyThread.IsBackground=true; // stop game by stoping thread
            enemyThread.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g=e.Graphics;



            lock (gamelock) 
            { 
            // Ship  body ( blue triangle or rectangle)
            // Dring temporery ship triangle
            Brush shipBrush = Brushes.DeepSkyBlue;
            g.FillRectangle(shipBrush, playerX + 15, playerY - 10, 20, 20);



            foreach(Rectangle laser in lasers)
            {
                e.Graphics.FillRectangle(Brushes.Yellow, laser);
            }



            //Show enemies on Screen
            foreach(Rectangle enemy in enemies)
            {
                g.FillEllipse(Brushes.Red,enemy); //Red Colour Enemies
            }

            }
            //To show Score
            String ScoreText = "Score: " + score;
            Font scoreFont = new Font("Arial", 16, FontStyle.Bold);
            g.DrawString(ScoreText, scoreFont, Brushes.White, 10, 10);




            //Show Gameover
            if (IsGameOver)
            {   //Dim screen when game over
                g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 0, 0, this.Width, this.Height);
                g.DrawString("GAME OVER", new Font("Arial", 30, FontStyle.Bold), Brushes.Red, this.Width/2-150,this.Height/2-80);
                g.DrawString("FINAL SCORE: "+score, new Font("Arial", 20,FontStyle.Bold), Brushes.White, this.Width / 2 - 130, this.Height / 2 + 20);
            }


        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //// Assign mouse X position to ship
            //playerX = e.X - 25; // 25 muinus for keeping mouse in center
            //// To keep screen refresh
            //this.Invalidate();
        }

        private readonly object gamelock = new object();
        List<Rectangle> lasers = new List<Rectangle>();
        Physics gamePhysics= new Physics(); //DLL class object


        List<Rectangle>enemies = new List<Rectangle>(); //Enemies list
        Random rand = new Random();




        //Movedown Event
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Smooth Movement logic
            if (IsLeftPressed && playerX > 0)
            {
                playerX -= moveSpeed;
            }
            if (IsRightPressed && playerX < this.ClientSize.Width - 50) 
            { 
                playerX += moveSpeed;
            }
            


            //Lock as the enemies did not clash with thread

            lock (gamelock)
            {
                //Collision Detection when laser fire hits enemies the enemies will disappared
                for (int i = 0; i < lasers.Count; i++)
                {
                    //Move Enemies
                    int newY = gamePhysics.Movelaser(lasers[i].Y, 10);

                    //Check if the laser is out of screen
                    if (gamePhysics.IsOutOfBounds(newY))
                    {
                        lasers.RemoveAt(i--);
                        continue;
                    }


                    lasers[i] = new Rectangle(lasers[i].X, newY, 10, 20);


                    //Collision check weather the laser hits the enemy

                    for (int j = 0; j < enemies.Count; j++)
                    {
                        if (lasers[i].IntersectsWith(enemies[j]))
                        {
                            enemies.RemoveAt(j); //Enemy End
                            lasers.RemoveAt(i); //Laser End
                            i--;
                            score += 10;
                            break;
                        }
                    }

                }


                this.Invalidate();

            }

        }


        //MoveEnemies code
        private void MoveEnemies()
        {
            
            while (!IsGameOver) {

                Rectangle shipRect = new Rectangle(playerX, playerY, 50, 30);

                //For Creating new Enemies
                if (enemies.Count < 5)
                {
                    // In Screen with create enemie anywhere
                    enemies.Add(new Rectangle(rand.Next(0, this.ClientSize.Width - 40), -50, 40, 40));
                }

                //Move all the enemies from top to down

                for (int i = 0;i < enemies.Count; i++)
                {
                    Rectangle en = enemies[i];
                    enemies[i] = new Rectangle(en.X, en.Y+5, 40, 40);


                    //If enemies is out of screen or hit the ship then game over
                    if (enemies[i].IntersectsWith(shipRect) || enemies[i].Y>600)
                    {
                        IsGameOver = true;
                        this.Invoke((MethodInvoker)delegate
                        {
                            timer1.Stop();
                            this.Invalidate();
                        });
                        break;
                    }
                }



                if (this.IsHandleCreated)
                {
                    try { 
                    //Tell UI thread to draw screen again(invoke)
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Invalidate();
                    });
                    }
                    catch { break; }

                }
               

                Thread.Sleep(50);
            
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) IsLeftPressed = true;
            if (e.KeyCode == Keys.Right) IsRightPressed = true;
           
            if(e.KeyCode == Keys.Space)
            {
                lock (gamelock) { 
                //Rectangle for getting laser for the tip of the ship
                Rectangle newlaser = new Rectangle(playerX + 20, playerY, 10, 20);
                lasers.Add(newlaser);
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) IsLeftPressed = false;
            if (e.KeyCode == Keys.Right) IsRightPressed = false;
        }
    }
}
