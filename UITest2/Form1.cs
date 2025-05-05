using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UITest2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void TestButtonSync_Click(object sender, EventArgs e)
        {
            //Thread.Sleep(5000);
            //Console.WriteLine($"Click:{((Button)sender).Text}");
            resultTextBox.Text = "Get data...";
            //Thread.Sleep(1000);
            string data = CallLongRunningApi("https://jsonplaceholder.typicode.com/todos/1");
            resultTextBox.Text = data.Substring(0, Math.Min(500, data.Length));
        }

        private async void TestButtonAsync_Click(object sender, EventArgs e)
        {
            //await Task.Delay(5000);
            //Console.WriteLine($"Click:{((Button)sender).Text}");
            resultTextBox.Text = "Get data...";
            string data = await CallLongRunningApiAsync("https://jsonplaceholder.typicode.com/todos/1");
            resultTextBox.Text = data.Substring(0, Math.Min(500, data.Length));
        }

        private async Task<string> CallLongRunningApiAsync(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                await Task.Delay(5000);
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private string CallLongRunningApi(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                var task = Task.Run(() => client.GetAsync(apiUrl));
                Thread.Sleep(5000);
                task.Wait();
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                var rTask = Task.Run(() => response.Content.ReadAsStringAsync());
                rTask.Wait();
                return rTask.Result;
            }
        }
    }
}
