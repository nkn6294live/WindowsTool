using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowTool
{
    public partial class ScreenRecordMainForm : Form
    {
        public ScreenRecordMainForm()
        {
            InitializeComponent();
            _selectionOverlay = new SelectionOverlayForm();
            ffmpegFolderPath = Path.GetFullPath(ffmpegFolderPath);
            Console.WriteLine($"ffmpegFolderPath: {ffmpegFolderPath}");
        }

        private bool isRecording = false;
        private readonly List<Bitmap> frames = new List<Bitmap>();
        private Timer captureTimer;
        private Rectangle captureRegion = Rectangle.Empty;
        private int fps = 24;
        private long captureStartTime = 0;
        private long captureCount = 0;
        private long saveCount = 0;

        private string currentTempFolder = "";
        private readonly string ffmpegFolderPath = "./ffmpeg";
        private readonly long maxLengthSecond = 1800;
        private readonly SelectionOverlayForm _selectionOverlay;
        private readonly bool openVideoBySystem = false;

        private void ScreenRecordMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRecording)
            {
                MessageBox.Show("Vui lòng dừng quay trước khi đóng ứng dụng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else
            {
                foreach (var frame in frames)
                {
                    frame?.Dispose();
                }
                _selectionOverlay?.Dispose();
            }
        }

        private void RecordButton_Click(object sender, EventArgs e)
        {
            if (isRecording)
            {
                StopRecording();
            }
            else
            {
                using (RegionSelectorForm selector = new RegionSelectorForm("Chọn vùng ghi hình"))
                {
                    recordButton.Text = "Chọn vùng";
                    if (selector.ShowDialog() == DialogResult.OK)
                    {
                        captureRegion = selector.SelectedRegion;
                        Rectangle overlayRegion = captureRegion;
                        overlayRegion.Inflate(2, 2);
                        _selectionOverlay.CaptureRegion = overlayRegion;
                    } else
                    {
                        recordButton.Text = "Quay màn hình";
                        _selectionOverlay.CaptureRegion = Rectangle.Empty;
                        return;
                    }
                }
                StartRecording();
            }
        }

        private void FPSNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            fps = (int)fpsNumericUpDown.Value;
        }

        private void CaptureScreen_Click(object sender, EventArgs e)
        {
            Rectangle rect = captureRegion;
            using (RegionSelectorForm selector = new RegionSelectorForm("Chọn vùng chụp màn hình"))
            {
                captureScreen.Text = "Chọn vùng";
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    rect = selector.SelectedRegion;
                } else
                {
                    captureScreen.Text = "Chụp màn hình";
                    return;
                }
            }
            captureScreen.Text = "Chụp màn hình";
            var bmp = CaptureFrameWithRect(rect);
            if (bmp == null)
            {
                MessageBox.Show("Lấy thông tin ảnh không thành công", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*",
                Title = "Chọn vị trí lưu ảnh",
                FileName = $"img_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.jpg"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var outputImgPath = saveFileDialog.FileName;
                if (File.Exists(outputImgPath))
                {
                    MessageBox.Show("File đã tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    bmp.Save(outputImgPath, ImageFormat.Jpeg);
                    if (openVideoBySystem)
                    {
                        OpenMediaFile(outputImgPath);
                    } else
                    {
                        OpenMediaFileInner(outputImgPath);
                    }
                }
                catch (Exception ex) { 
                    MessageBox.Show($"Lưu không thành công tại {outputImgPath}: {ex.Message}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void StartRecording()
        {
            if (isRecording)
            {
                return;
            }
            currentTempFolder = Path.Combine(Path.GetTempPath(), $"recording_{Guid.NewGuid()}");
            Directory.CreateDirectory( currentTempFolder );
            isRecording = true;
            UpdateUI(() =>
            {
                recordButton.Enabled = true;
                recordButton.Text = "00:00:00";
                fpsNumericUpDown.Enabled = false;
            });
            captureTimer = new Timer()
            {
                Interval = 1000 / fps
            };
            captureStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            captureCount = 0;
            saveCount = 0;
            captureTimer.Tick += CaptureFrame;
            captureTimer.Start();

            frames.Clear();
            Rectangle overlayRegion = captureRegion;
            overlayRegion.Inflate(2, 2);
            _selectionOverlay.CaptureRegion = overlayRegion;
        }
        private void StopRecording()
        {
            if (!isRecording)
            {
                return;
            }
            try
            {
                captureTimer?.Stop();
                _selectionOverlay.CaptureRegion = Rectangle.Empty;
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = $"recording_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.mp4",
                    Filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*",
                    Title = "Chọn vị trí lưu video"
                };
                var outputFilePath = string.Empty;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilePath = saveFileDialog.FileName;
                }
                if (string.IsNullOrEmpty(outputFilePath) || File.Exists(outputFilePath))
                {
                    UpdateUI(() => { MessageBox.Show($"File '{outputFilePath}' đã tồn tại hoặc không thỏa mãn. Kết thúc.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); });
                    foreach (var item in frames)
                    {
                        item.Dispose();
                    }
                    frames.Clear();
                    return;
                }
                Task.Run(() => { SaveVideo(frames, outputFilePath, true); });
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            } finally
            {
                isRecording = false;
                UpdateUI(() =>
                {
                    recordButton.Enabled = true;
                    recordButton.Text = "Quay màn hình";
                    fpsNumericUpDown.Enabled = true;
                });
            }
        }
        private void CaptureFrame(object sender, EventArgs e)
        {
            var bmp = CaptureFrameWithRect(captureRegion);
            if (bmp != null)
            {
                frames.Add(bmp);
            }
            ++captureCount;
            if (captureCount % fps == 0)
            {
                var s = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - captureStartTime) / 1000;
                UpdateUI(() => { recordButton.Text = $"{ToHumanTime(s + 0)}"; });
                if (s > maxLengthSecond)
                {
                    UpdateUI(() => { StopRecording(); });
                }
            } 
            if (captureCount % (10 * fps) == 0)
            {
                var _frames = new List<Bitmap>(frames);
                frames.Clear();
                Task.Run(() =>
                {
                    var frameDirectory = currentTempFolder;
                    for (int i = 0; i < _frames.Count; i++)
                    {
                        _frames[i].Save(Path.Combine(frameDirectory, $"frame_{(++saveCount):D8}.jpg"), ImageFormat.Jpeg);
                        _frames[i].Dispose();
                    }
                });
            }
        }

        private Bitmap CaptureFrameWithRect(Rectangle captureRegion)
        {
            try
            {
                var bmp = new Bitmap(captureRegion.Width, captureRegion.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                {
                    if (g == null) throw new Exception("graphic init null");
                    g.CopyFromScreen(captureRegion.X, captureRegion.Y, 0, 0, (captureRegion.Size));
                }
                return bmp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(captureRegion);
                return null;
            }
        }

        private async void SaveVideo(List<Bitmap> frames, string filePath = null, bool disposeAfterSave = true)
        {
            if (string.IsNullOrEmpty(filePath)) {
                if (disposeAfterSave)
                {
                    foreach (var item in frames)
                    {
                        item.Dispose();
                    }
                    frames.Clear();
                }
                return;
            }
            if (frames.Count > 0 || (captureCount > 0))
            {
                try
                {
                    UpdateUI(() =>
                    {
                        recordButton.Text = "Đang xử lý...";
                        recordButton.Enabled = false;
                    });

                    var directory = Path.GetDirectoryName(filePath) ?? throw new Exception("Lỗi lấy thông tin lưu file");
                    string frameDirectory = currentTempFolder;
                    Directory.CreateDirectory(frameDirectory);

                    for (int i = 0; i < frames.Count; i++)
                    {
                        frames[i].Save(Path.Combine(frameDirectory, $"frame_{(++saveCount):D8}.jpg"), ImageFormat.Jpeg);
                        if (disposeAfterSave)
                        {
                            frames[i].Dispose();
                        }
                    }
                    var errorOutput = await ConvertFramesToVideo(frameDirectory, filePath);
                    if (string.IsNullOrEmpty(errorOutput))
                    {
                        Directory.Delete(frameDirectory, true);
                        //Process.Start("explorer.exe", $"{directory}");
                        if (this.openVideoBySystem)
                        {
                            OpenMediaFile(filePath);
                        } else
                        {
                            OpenMediaFileInner(filePath);
                        }
                    } else
                    {
                        Console.WriteLine(errorOutput);
                    }
                }
                catch (Exception ex)
                {
                    UpdateUI(() => { MessageBox.Show($"Lỗi khi lưu video: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); });
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    if (disposeAfterSave)
                    {
                        frames.Clear();
                    }
                    {
                        UpdateUI(() =>
                        {
                            recordButton.Text = "Quay màn hình";
                            recordButton.Enabled = true;
                        });
                    }
                }
            }
        }

        private async Task<string> ConvertFramesToVideo(string frameDirectory, string outputFilePath)
        {
            var ffmpegPath = $"{ffmpegFolderPath}/bin/ffmpeg.exe";
            var ffArguments = $"-framerate {fps} -i {Path.Combine(frameDirectory, "frame_%08d.jpg")} -c:v libx264 -pix_fmt yuv420p {outputFilePath}";
            //Process.Start(ffmpegPath, ffArguments);
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo() { 
                    FileName = ffmpegPath,
                    Arguments = ffArguments,
                    UseShellExecute = false, // Không sử dụng shell để chạy
                    RedirectStandardError = true, // Để đọc lỗi (nếu có)
                    CreateNoWindow = true // Không hiển thị cửa sổ console của ffmpeg
                };
                process.EnableRaisingEvents = true; // Để bắt sự kiện Exited

                string errorOutput = null;
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorOutput += e.Data + Environment.NewLine;
                    }
                };

                process.Start();
                process.BeginErrorReadLine(); // Bắt đầu đọc lỗi bất đồng bộ

                await Task.Run(() => process.WaitForExit()); // Chờ tiến trình ffmpeg kết thúc

                if (process.ExitCode != 0)
                {
                    return errorOutput;
                }
                return "";
            }
        }

        private void OpenMediaFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                UpdateUI(() => { MessageBox.Show($"File '{filePath}' không tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); });
                return;
            }
            try
            {
                Process.Start(filePath);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Task.Run(() => OpenMediaFileInner(filePath));
            }
        }

        private void OpenMediaFileInner(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                UpdateUI(() => { MessageBox.Show($"File '{filePath}' không tồn tại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); });
                return;
            }
            try
            {
                var ffplayPath = $"{ffmpegFolderPath}/bin/ffplay.exe";
                var ffArguments = $"{filePath}";
                //Process.Start(ffplayPath, ffArguments);
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo() {
                        FileName = ffplayPath,
                        Arguments = ffArguments,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    process.Start();
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                UpdateUI(() => { MessageBox.Show($"Lỗi mở file '{filePath}' {ex.Message}", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); });
            }
        }



        private void UpdateUI(Action a)
        {
            if (InvokeRequired) { this.Invoke(a); } else { a(); }
        }

        private string ToHumanTime(long second)
        {
            long s = second, m = 0, h = 0;
            if (s >= 60)
            {
                m = s / 60;
                s -= m * 60;
            }
            if (m >= 60)
            {
                h = m / 60;
                m -= h * 60;
            }
            var ss = s < 10 ? $"0{s}" : $"{s}";
            var ms = m < 10 ? $"0{m}" : $"{m}";
            var hs = h < 10 ? $"0{h}" : $"{h}";
            return $"{hs}:{ms}:{ss}";
        }
    }
}
