using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace speed_calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lastDirectory = "";
            lastFilename = "";
        }

        private void calculate(object sender = null, EventArgs e = null)
        {
            try
            {
                calculate(((Control)sender).Parent);
            }
            catch (Exception)
            {
                foreach (Control control in panel_all.Controls)
                {
                    calculate(control);
                }
            }
        }

        private void calculate(Control control)
        {

            if (!(control is Panel)) throw new Exception();

            try
            {
                double speed = Convert.ToDouble(hero_speed.Text);
                if (mode2.Checked)
                {
                    speed = Math.Sqrt(speed);
                }

                double mspeed = Convert.ToDouble(control.Controls.Find("monster_speed", true)[0].Text);
                bool priority = ((CheckBox)control.Controls.Find("monster_priority", true)[0]).Checked;

                if (speed < 1e-8 || mspeed < 1e-8) return;
                speed = 1.0 / speed; mspeed = 1.0 / mspeed;

                Queue<double> me = new Queue<double>(), monster = new Queue<double>();
                List<int> ans = new List<int>();
                for (int i = 1; i <= 30; i++)
                {
                    me.Enqueue(i * speed);
                    monster.Enqueue(i * mspeed);
                }
                for (int i = 1; i <= 20; i++)
                {
                    double x = me.Peek(), y = monster.Peek();

                    if (x < y - 1e-8)
                    {
                        ans.Add(0);
                        me.Dequeue();
                    }
                    else if (y < x - 1e-8)
                    {
                        ans.Add(1);
                        monster.Dequeue();
                    }
                    else
                    {
                        if (priority)
                        {
                            ans.Add(1);
                            monster.Dequeue();
                        }
                        else
                        {
                            ans.Add(0);
                            me.Dequeue();
                        }
                    }
                }
                draw(ans, (PictureBox)control.Controls.Find("picture", true)[0]);
            }
            catch (Exception)
            {

            }

        }

        private void remove(object sender, EventArgs args)
        {
            if (panel_all.Controls.Count == 1) return;
            try
            {
                Control parent = ((Control)sender).Parent;
                int index = panel_all.Controls.IndexOf(parent);
                if (index == -1) return;
                remove(index);
            }
            catch (Exception) { }
        }

        private void remove(int index)
        {
            try
            {
                Control control = panel_all.Controls[index];
                panel_all.Controls.RemoveAt(index);
                control.Dispose();
                for (int i = index; i < panel_all.Controls.Count; i++)
                {
                    Point location = panel_all.Controls[i].Location;
                    location.Y -= 30;
                    panel_all.Controls[i].Location = location;
                }
            }
            catch (Exception)
            {
            }
        }

        private void draw(List<int> list, PictureBox picture)
        {
            Bitmap bitmap = new Bitmap(400, 20);
            Bitmap me = new Bitmap(20, 20);
            Graphics graphic = Graphics.FromImage(me);
            graphic.FillRectangle(Brushes.MediumSeaGreen, 0, 0, 20, 20);
            graphic.DrawImage(Properties.Resources.me, 0,0);
            Bitmap monster = new Bitmap(20, 20);
            graphic = Graphics.FromImage(monster);
            graphic.FillRectangle(Brushes.Tomato, 0, 0, 20, 20);
            graphic.DrawImage(Properties.Resources.monster, 0, 0);

            Graphics graphics = Graphics.FromImage(bitmap);
            for (int i = 0; i < 20 && i < list.Count; i++)
            {
                graphics.DrawImage(list[i]==0?me:monster, 20 * i, 0);
            }
            picture.Image = bitmap;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btn_add_Click(null, null);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            pen.DashStyle = DashStyle.Solid;
            Graphics graphics = CreateGraphics();
            graphics.DrawLine(pen, 10, 50, ((Control)sender).Width - 32, 50);
            pen.Dispose();
            graphics.Dispose();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            int cnt = panel_all.Controls.Count;

            Panel panel = new Panel();
            panel.Size = new System.Drawing.Size(700, 37);
            panel.Location = new System.Drawing.Point(10, 30 * cnt - 5);

            TextBox tb_name = new TextBox();
            tb_name.Location = new System.Drawing.Point(8, 8);
            tb_name.Size = new System.Drawing.Size(72, 20);
            tb_name.Text = "怪物" + (cnt + 1);
            tb_name.Name = "monster_name";
            tb_name.TabIndex = 5 * cnt + 4;
            panel.Controls.Add(tb_name);

            TextBox tb_speed = new TextBox();
            tb_speed.Location = new System.Drawing.Point(90, 8);
            tb_speed.Size = new System.Drawing.Size(53, 20);
            tb_speed.Text = "1.0";
            tb_speed.TextChanged += calculate;
            tb_speed.Name = "monster_speed";
            tb_speed.TabIndex = 5 * cnt + 5;
            panel.Controls.Add(tb_speed);

            CheckBox cb_priority = new CheckBox();
            cb_priority.Location = new System.Drawing.Point(169, 8);
            cb_priority.Size = new System.Drawing.Size(20, 20);
            cb_priority.Name = "monster_priority";
            cb_priority.CheckedChanged += calculate;
            cb_priority.TabIndex = 5 * cnt + 6;
            panel.Controls.Add(cb_priority);

            PictureBox picture = new PictureBox();
            picture.Location = new System.Drawing.Point(212, 8);
            picture.Size = new System.Drawing.Size(400, 20);
            picture.Name = "picture";
            picture.TabIndex = 5 * cnt + 7;
            picture.BackColor = System.Drawing.Color.LightGray;
            panel.Controls.Add(picture);


            Button btn = new Button();
            btn.Text = "删除";
            btn.Location = new System.Drawing.Point(631, 8);
            btn.Size = new System.Drawing.Size(47, 20);
            panel.Controls.Add(btn);
            btn.Click += remove;

            panel_all.Controls.Add(panel);
            calculate(panel);

            tb_name.Focus();
        }

        private void hero_speed_TextChanged(object sender, EventArgs e)
        {
            calculate();
        }

        private void mode1_CheckedChanged(object sender, EventArgs e)
        {
            calculate();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (string.IsNullOrEmpty(lastDirectory) || string.IsNullOrEmpty(lastFilename))
            {
                dialog.FileName = "save";
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                dialog.FileName = lastFilename;
                dialog.InitialDirectory = lastDirectory;
            }
            dialog.Filter = "怪物攻速数据(*.monspeed)|*.monspeed";
            dialog.AddExtension = true;
            dialog.DefaultExt = ".monspeed";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string text = hero_speed.Text + " " + (mode1.Checked?0:1) + "\n";
                int cnt = panel_all.Controls.Count;
                text += cnt + "\n";
                foreach (Control control in panel_all.Controls)
                {
                    text += control.Controls.Find("monster_name", true)[0].Text + " " +
                            control.Controls.Find("monster_speed", true)[0].Text + " " +
                            (((CheckBox)control.Controls.Find("monster_priority", true)[0]).Checked?1:0) + "\n";
                }
                File.WriteAllText(dialog.FileName, text);
                lastDirectory = Path.GetDirectoryName(dialog.FileName);
                lastFilename = Path.GetFileName(dialog.FileName);

                MessageBox.Show("文件保存至 " + dialog.FileName + "。", "保存成功！");
            }
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "怪物攻速数据(*.monspeed)|*.monspeed";
            // dialog.RestoreDirectory = true;
            if (string.IsNullOrEmpty(lastDirectory) || string.IsNullOrEmpty(lastFilename))
            {
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
            }
            else
            {
                dialog.InitialDirectory = lastDirectory;
                dialog.FileName = lastFilename;
            }
            dialog.AddExtension = true;
            dialog.DefaultExt = ".monspeed";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] text = File.ReadAllLines(dialog.FileName);
                string[] hero = text[0].Split(" ".ToCharArray());
                hero_speed.Text = hero[0];
                // textbox_hero_def.Text = atkdef[1];
                bool x = (Convert.ToInt32(hero[1]) == 0 ? (mode1.Checked = true) : (mode2.Checked = true));
                int cnt = Convert.ToInt32(text[1]);
                while (panel_all.Controls.Count < cnt) btn_add_Click(null,null);
                while (panel_all.Controls.Count > cnt) remove(panel_all.Controls.Count - 1);
                for (int i = 0; i < cnt; i++)
                {
                    string[] strings = text[2 + i].Split(" ".ToCharArray());
                    Control control = panel_all.Controls[i];
                    control.Controls.Find("monster_name", true)[0].Text = strings[0];
                    control.Controls.Find("monster_speed", true)[0].Text = strings[1];
                    ((CheckBox)control.Controls.Find("monster_priority", true)[0]).Checked = Convert.ToInt32(strings[2])!=0;
                }
                calculate();

                lastDirectory = Path.GetDirectoryName(dialog.FileName);
                lastFilename = Path.GetFileName(dialog.FileName);

            }
        }
    }
}
