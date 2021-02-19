# Phần mềm chụp và ORC ảnh bằng c#



*Đây là project chụp màn hình trong một khoảng:
*có thể chụp rồi OCR sử dụng tessract 5.0
*chức năng đã có
	-chụp lại màn hình theo mọi chiều
	-ocr

## Các bước thực hiện

#### Chúng ta cần có 1 panel và 1 form đặt chúng hiển thị toàn màn hình:

```c#
  private void Form1_Load(object sender, EventArgs e)
  {
      this.Width = Screen.PrimaryScreen.Bounds.Width;
      this.Height = Screen.PrimaryScreen.Bounds.Height - 40;
      this.TopMost = true;
      panel1.Dock = DockStyle.Fill;
  }
```

* Thanh taskbar mặc định windows 10 có độ cao là 40px.

#### Chúng ta cần chụp lại màn hình:

1. Băt sự kiện nhấn chuột để lấy tọa độ đầu tiên:	

   ```c#
    private void panel1_MouseDown(object sender, MouseEventArgs e)
    {
        panel2.Size = new Size(0, 0);
        panel1.MouseMove += Panel1_MouseMove;
        x = e.X; y = e.Y;
        panel2.Location = new Point(e.X, e.Y); // Đặt tọa độ của panel2
        panel2.Controls.Clear(); // Xóa toàn bộ controls trên panel2
    }
   ```

2.  Thao tác sự kiện mouse move:

   ```c#
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
   ```

3. Thực hiện chụp

   ```c#
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
   ```

   

### OCR

```c#
 //MessageBox.Show(Application.StartupPath, "a");
            string input= string.Format(@"{0}\{1}\{2}",Application.StartupPath,"img","img.png");
            string output = string.Format(@"{0}\{1}\{2}", Application.StartupPath, "img", "res");

            string tesseract = Application.StartupPath + @"\Tesseract\tesseract.exe";
            string res = " " + input + " " + output + " -l eng";

            Process.Start(tesseract,res);
            Thread.Sleep(3000);
            string i=File.ReadAllText(string.Format(@"{0}\{1}\{2}", Application.StartupPath, "img", "res.txt"));
            MessageBox.Show(i);
```

