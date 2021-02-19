using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Shell;
using System.Reflection;

namespace ocr1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static int x, y;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            //Bắt sự kiện nhấn chuột xuống
            panel2.Size = new Size(0, 0);
            //tao su kien mouse move;
            panel1.MouseMove += Panel1_MouseMove;
            x = e.X; y = e.Y;
            #region kéo trái >phai
            panel2.Location = new Point(e.X, e.Y);
            #endregion
            panel2.Controls.Clear();

        }
        
        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            
            //if((e.X - panel2.Location.X)>0)
            if (e.X > x && e.Y > y)
            {
                //1
                panel2.Location = new Point(x, y);
                panel2.Size = new Size(Math.Abs(x - e.X), Math.Abs(y - e.Y));
                
            }
            if (e.X < x && e.Y < y)
            {
                panel2.Location = new Point(e.X, e.Y);
                panel2.Size = new Size(Math.Abs(x - e.X), Math.Abs(y - e.Y));
                
            }
            if (e.X > x && e.Y < y)
            {
                panel2.Location = new Point(x, e.Y);
                panel2.Size = new Size(Math.Abs(x - e.X), Math.Abs(y - e.Y));
                
            }
            if (e.X < x && e.Y > y)
            {
                panel2.Location = new Point(e.X, y);
                panel2.Size = new Size(Math.Abs(x - e.X), Math.Abs(y - e.Y));
                
            }
           

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            panel1.MouseMove -= Panel1_MouseMove;        


            PictureBox pictureBox1 = new PictureBox();
            panel2.Controls.Add(pictureBox1);
            pictureBox1.ContextMenuStrip = OCR;
            pictureBox1.Dock = DockStyle.Fill;
            #region chup
          
            //chup man hinh
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);
            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);
            
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            pictureBox1.Refresh();
            //cat anh
            if (pictureBox1.Width != 0 && pictureBox1.Height != 0)
            {
                pictureBox1.Image = CropImage(bmpScreenshot, new Rectangle(panel2.Location.X, panel2.Location.Y, pictureBox1.Width, pictureBox1.Height));
                
            }
            if(File.Exists(Application.StartupPath + @"\img\img.png"))
            {
                File.Delete(Application.StartupPath + @"\img\img.png");
            }
            if (pictureBox1.Image != null ){
                pictureBox1.Image.Save(Application.StartupPath + @"\img\img.png"); 
            }
            if (bmpScreenshot != null)
            {
                bmpScreenshot.Dispose();
                bmpScreenshot = null;
            }
            #endregion
            //panel1.BackColor=Color.FromArgb()

        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnOCR_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(Application.StartupPath, "a");
            string input= string.Format(@"{0}\{1}\{2}",Application.StartupPath,"img","img.png");
            string output = string.Format(@"{0}\{1}\{2}", Application.StartupPath, "img", "res");

            string tesseract = Application.StartupPath + @"\Tesseract\tesseract.exe";
            string res = " " + input + " " + output + " -l eng";

            Process.Start(tesseract,res);
            Thread.Sleep(3000);
            string i=File.ReadAllText(string.Format(@"{0}\{1}\{2}", Application.StartupPath, "img", "res.txt"));
            MessageBox.Show(i);




        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Hiển thi full màn hình. 
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height - 40; //thanh taskbar có độ cao 40px
            this.TopMost = true;
            panel1.Dock = DockStyle.Fill;

            //JumpTask task = new JumpTask();
            //task.Title = "hemm";
            //JumpList jumpList = new JumpList();
            //jumpList.JumpItems.Add(task);
            //jumpList.ShowFrequentCategory = false;
            //jumpList.ShowRecentCategory = false;
            //jumpList.Apply();
            //JumpList.SetJumpList(App.Current, jumpList);
        }

        private void engToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #region cat_anh
        /// <summary>
        /// Hàm cắt ảnh theo hình vuông.
        /// </summary>
        /// <param name="originalImage"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="destinationRectangle"></param>
        /// <returns></returns>
        static Bitmap CropImage(Image originalImage, Rectangle sourceRectangle, Rectangle? destinationRectangle = null)
        {
            if (destinationRectangle == null)
            {
                destinationRectangle = new Rectangle(Point.Empty, sourceRectangle.Size);
            }

            var croppedImage = new Bitmap(destinationRectangle.Value.Width,
                destinationRectangle.Value.Height);
            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.DrawImage(originalImage, destinationRectangle.Value,
                    sourceRectangle, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }
        #endregion
    }
}
