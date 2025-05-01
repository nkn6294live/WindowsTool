using System.Drawing;
using System.Windows.Forms;

namespace WindowTool
{
    public partial class SelectionOverlayForm : Form
    {
        public SelectionOverlayForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Fuchsia;
            TransparencyKey = Color.Fuchsia;
            TopMost = true;
            ShowInTaskbar = false;
            Enabled = false;
            Visible = false;
        }

        public Rectangle CaptureRegion
        {
            get => _captureRegion;
            set
            {
                _captureRegion = value;
                if (!_captureRegion.IsEmpty)
                {
                    // Đặt vị trí và kích thước của overlay theo captureRegion
                    this.Bounds = _captureRegion;
                    this.Invalidate(); // Vẽ lại overlay
                    this.Visible = true;
                }
                else
                {
                    this.Visible = false;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_captureRegion.IsEmpty)
            {
                using (var pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, _captureRegion.Width - 1, _captureRegion.Height - 1);
                }
            }
        }

        private Rectangle _captureRegion = Rectangle.Empty;
    }
}
