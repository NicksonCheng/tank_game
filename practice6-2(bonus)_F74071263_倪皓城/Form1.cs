using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practice6_2_bonus__F74071263_倪皓城
{
    public partial class Form1 : Form
    {
        private Button button1;
        private Timer time1;
        private Timer bomb_timer;
        private Timer interval;
        
        private int padding_count = 0;
        private GroupBox box1;
        //location
        private int x, y;
        private int score = 0;
        private int count = -1;//章魚落下時間間隔
        private int num = 0;//章魚的數量
        private Random rnd;
        private bool dead_flag = true;
        private PictureBox self, enemy;
        private List<PictureBox> ghost;
        private List<int> y2 = new List<int>();
        private List<int> x2 = new List<int>();
        private Label label1,label2;

        private PictureBox[] bomb;
        private int sum = 0;
        private int b_dy = -10;
        private int[] b_y = new int[10];
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(400, 400);
            bomb = new PictureBox[10];
            rnd = new Random();
            this.BackColor = Color.Black;
            box1 = new GroupBox();
            box1.Size = new Size(10, 400);
            box1.Location = new Point(300, 0);
            box1.BackColor = Color.White;
            this.Controls.Add(box1);

            label1 = new Label();
            label1.Text = "score:";
            label1.ForeColor = Color.White;
            label1.Location = new Point(320, 10);
            this.Controls.Add(label1);
            label2 = new Label();
            label2.Text = "";
            label2.ForeColor = Color.White;
            label2.Location = new Point(320, 300);
            this.Controls.Add(label2);
            button1 = new Button();
            button1.Text = "重新開始";
            button1.BackColor = Color.White;
            button1.Location = new Point(310, 50);
            button1.Enabled = false;
            this.Controls.Add(button1);

            //我的飛船
            self = new PictureBox();
            self.Image = Image.FromFile("ship 1.png");
            self.Size = new Size(50, 50);
            self.SizeMode = PictureBoxSizeMode.Zoom;

            self.Location = new Point(100, 300);
            x = 100;
            y = 300;
            //外星章魚們
            ghost = new List<PictureBox>();
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);


            //timer
            time1 = new Timer();
            
            time1.Interval = 50;

            time1.Tick += new EventHandler(attack);
            //重新開始遊戲連結
            button1.Click += new EventHandler(restart);


            bomb_timer = new Timer();
            bomb_timer.Interval = 50;
            bomb_timer.Enabled = false;
            bomb_timer.Tick += new EventHandler(Shot_Tocus);


            interval = new Timer();
            interval.Interval = 1000;
           
            interval.Tick += new EventHandler(bomb_padding);
            DialogResult result=MessageBox.Show("遊戲玩法:玩家用坦克躲章魚 可按space使用砲彈攻擊 每次有10顆 球要消失才能設下一顆 用完之後需要等30秒能再次使用 碰到章魚就掛囉", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            if (result == DialogResult.OK)
            {
                time1.Enabled = true;
            }


        }
        private void attack(object sendor, EventArgs e)
        {

            //掉下吧章魚
            if (count >= 10 || count == -1)
            {

                enemy = new PictureBox();
                enemy.Image = Image.FromFile("Invader.png");
                enemy.Size = new Size(30, 30);
                enemy.SizeMode = PictureBoxSizeMode.Zoom;
                y2.Add(0);

                //不超過邊界
                x2.Add(rnd.Next(box1.Location.X - enemy.Width));
                enemy.Location = new Point(x2[num], 0);
                ghost.Add(enemy);
                this.Controls.Add(enemy);

                num++;
                count = 0;

            }

            for (int i = 0; i < num; i++)
            {
                y2[i] += 15;
                ghost[i].Location = new Point(x2[i], y2[i]);


                //躲過章魚
                if (ghost[i].Bottom >= this.Height)
                {

                    //score++;
                    //怕記憶體爆掉
                    label1.Text = "score:" + score.ToString();
                    //ghost[i].Dispose();
                    ghost.RemoveAt(i);
                    x2.RemoveAt(i);
                    y2.RemoveAt(i);
                    ghost.TrimExcess();
                    x2.TrimExcess();
                    y2.TrimExcess();

                    //章魚到底了要list退回一個
                    num--;


                    //label1.Text = ghost.LastIndexOf(enemy).ToString();
                }



            }
            for (int i = 0; i < num; i++)
            {
                //如國碰到鬼
                if ((ghost[i].Top <= self.Bottom) && (ghost[i].Bottom) >= self.Top && ghost[i].Left <= self.Right && ghost[i].Right >= self.Left && !ghost[i].IsDisposed)
                {
                    time1.Enabled = false;
                    label1.Text = "game over!!";
                    score = 0;
                    dead_flag = false;
                    button1.Enabled = true;
                    interval.Enabled = false;
                    label2.Text = "";

                }
            }
            count++;
            this.Controls.Add(self);
        }
        private void Shot_Tocus(object sendor, EventArgs e)
        {

            for (int i = 0; i < sum; i++)
            {
                if (!bomb[i].IsDisposed)
                {
                    b_y[i] += b_dy;
                    bomb[i].Location = new Point(bomb[i].Location.X, b_y[i]);

                }

                if (bomb[i].Location.Y <= 0)
                {
                    this.Controls.Remove(bomb[i]);
                    bomb[i].Dispose();

                    b_y[i] = 0;
                }

            }

            //幹掉章魚
            for (int i = 0; i < num; ++i)
            {
                for (int j = 0; j < sum; ++j)
                {
                    if (bomb[j].Right >= ghost[i].Left && bomb[j].Left <= ghost[i].Right && !bomb[j].IsDisposed && !ghost[i].IsDisposed)
                    {
                        if (bomb[j].Top <= ghost[i].Bottom)
                        {
                            score++;
                            //怕記憶體爆掉
                            label1.Text = "score:" + score.ToString();
                            this.Controls.Remove(ghost[i]);
                            ghost.RemoveAt(i);
                            
                            x2.RemoveAt(i);
                            y2.RemoveAt(i);
                            ghost.TrimExcess();
                            x2.TrimExcess();
                            y2.TrimExcess();

                            //章魚到底了要list退回一個
                            num--;



                            this.Controls.Remove(bomb[j]);
                            bomb[j].Dispose();

                            b_y[j] = 0;
                        }
                    }

                }
            }

        }
        private void bomb_padding(object sendor,EventArgs e)
        {
            label2.Text = "倒數:"+(30 - padding_count).ToString();
            padding_count++;
            
            if (padding_count >= 30)
            {
                sum = 0;
                padding_count = 0;
                label2.Text = "";

            }
            
               

        }
        private void Form1_KeyDown(object sendor, KeyEventArgs e)
        {
            //label1.Text = dead_flag.ToString();
            if (dead_flag)
            {

                switch (e.KeyValue)
                {
                    //left
                    case 37:
                        if (self.Location.X > 0)
                            x -= 10;
                        break;
                    //right
                    case 39:
                        if (self.Location.X + self.Width < box1.Location.X)
                            x += 10;
                        break;
                    //up
                    case 38:
                        if (self.Location.Y > 0)
                            y -= 10;
                        break;
                    //down
                    case 40:
                        if (self.Location.Y + self.Height + 40 < this.Height)
                        {

                            y += 10;
                        }

                        break;
                    case 32:
                        //前一個球消失才能在設下一個
                        if(sum ==0 || bomb[sum-1].IsDisposed)
                        {
                            if (sum < 10)
                            {
                                bomb[sum] = new PictureBox();
                                bomb[sum].Image = Image.FromFile("pp.gif");
                                bomb[sum].Size = new Size(20, 20);
                                bomb[sum].SizeMode = PictureBoxSizeMode.Zoom;
                                bomb[sum].Location = new Point(self.Left + self.Width / 2 - bomb[sum].Width / 2, self.Top - bomb[sum].Height);
                                this.Controls.Add(bomb[sum]);
                                b_y[sum] = bomb[sum].Location.Y;
                                sum++;
                                bomb_timer.Enabled = true;
                                interval.Enabled = false;

                            }
                            if(sum>=10)
                                interval.Enabled = true;

                        }

                        break;



                }
                self.Location = new Point(x, y);
            }


        }
        private void restart(object sendor, EventArgs e)
        {
            for (int i = 0; i < ghost.Count; i++)
            {
                this.Controls.Remove(ghost[i]);
            }
            ghost.Clear();
            y2.Clear();
            x2.Clear();

            label1.Text = ghost.Count.ToString();
            
            count = -1;
            num = 0;
            self.Location = new Point(100, 300);
            time1.Enabled = true;
            dead_flag = true;
            x = 100;
            y = 300;
            button1.Enabled = false;

           
            sum = 0;
            padding_count = 0;
        }
    }
}

    



   
        

       
           
        
        