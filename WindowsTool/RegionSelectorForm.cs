using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowTool
{
    public partial class RegionSelectorForm : Form
    {
        public Rectangle SelectedRegion => selectedRegion;

        public RegionSelectorForm(string _title = "")
        {
            InitializeComponent();
            DoubleBuffered = true; // Giảm nhấp nháy khi vẽ
            Cursor = Cursors.Cross;
            
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = fullScreenRect.Value.Location;
            Size = fullScreenRect.Value.Size;
            title = string.IsNullOrEmpty(_title) ? "Chọn vùng" : _title;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = new Point(e.X, e.Y);
                endPoint = startPoint;
                Invalidate(); // Yêu cầu vẽ lại form
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isSelecting) {
                endPoint = new Point(e.X, e.Y);
                Invalidate(); // Yêu cầu vẽ lại form để hiển thị vùng chọn hiện tại
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isSelecting && e.Button == MouseButtons.Left) {
                isSelecting = false;
                // Xác định hình chữ nhật vùng chọn đã vẽ
                int x = Math.Min(startPoint.X, endPoint.X);
                int y = Math.Min(startPoint.Y, endPoint.Y);
                int width = Math.Abs(startPoint.X - endPoint.X);
                int height = Math.Abs(startPoint.Y - endPoint.Y);
                width += width % 2;
                height += height % 2;
                selectedRegion = new Rectangle(x, y, width, height);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //using (Graphics g = e.Graphics) 
            {
                //using (var backgroundBrush = new SolidBrush(Color.FromArgb((int)(0.1 * 255), BackColor)))
                //{
                //    e.Graphics.FillRectangle(backgroundBrush, ClientRectangle);
                //}

                if (isSelecting)
                {
                    var x = Math.Min(startPoint.X, endPoint.X);
                    var y = Math.Min(startPoint.Y, endPoint.Y);
                    var width = Math.Abs(startPoint.X - endPoint.X);
                    var height = Math.Abs(startPoint.Y - endPoint.Y);
                    var drawRect = new Rectangle(x, y, width, height);
                    using (var pen = new Pen(Color.Red, 2))
                    {
                        //e.Graphics.DrawRectangle(pen, x, y, width, height);
                        e.Graphics.DrawRectangle(pen, drawRect);
                    }
                    //if (drawRect.Width > 0 && drawRect.Height > 0) {
                    //    using (var selectionBrush = new SolidBrush(Color.FromArgb(0, Color.Black)))
                    //    {
                    //        e.Graphics.FillRectangle(selectionBrush, drawRect);
                    //    }
                    //}
                }
                {
                    string cancelText = $"{title}. Ấn ESC để hủy".ToUpper();
                    using (Font font = new Font("Arial", 24, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    using (var sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        RectangleF textRect = new RectangleF(100, ClientRectangle.Height / 2 - 25, 500, 200);
                        e.Graphics.DrawString(cancelText, font, textBrush, textRect, sf);
                    }
                }
            }
            base.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel; // Đặt DialogResult thành Cancel
                this.Close(); // Đóng form
            }
            base.OnKeyDown(e);
        }

        private Point startPoint;
        private Point endPoint;
        private bool isSelecting = false;
        private Rectangle selectedRegion = Rectangle.Empty;
        private readonly string title = "";
        private readonly Lazy<Rectangle> fullScreenRect = new Lazy<Rectangle>(() =>
        {
            return GetAllScreenRect();
        });

        private static Rectangle GetAllScreenRect()
        {
            Rectangle virtualScreenBounds = Rectangle.Empty;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (virtualScreenBounds == Rectangle.Empty)
                {
                    virtualScreenBounds = screen.Bounds;
                }
                else
                {
                    virtualScreenBounds = Rectangle.Union(virtualScreenBounds, screen.Bounds);
                }
            }
            return virtualScreenBounds;
        }
    }
}
