using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Newtonsoft.Json;

namespace GoodImouto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        IWebDriver driver;
        IWebElement textArea, playButton;
        QandALib qAndALib;
        const string libFilePath = "lib.json";
        Random random = new Random();
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(libFilePath))
            {
                qAndALib = new QandALib();
            }
            else
            {
                string text = File.ReadAllText(libFilePath);
                qAndALib = JsonConvert.DeserializeObject<QandALib>(text);
            }
            driver = new EdgeDriver();
            driver.Navigate().GoToUrl("https://tts.yating.tw/?utm_source=yating_asr&utm_medium=link&utm_campaign=yating_friends");
            textArea = driver.FindElement(By.CssSelector("#__next > main > section.jsx-1414670988.mx-auto.max-w-screen-lg.px-2.lg\\:px-0 > div.jsx-1414670988.mx-auto.grid.grid-cols-1.lg\\:grid-cols-4.max-w-screen-lg.gap-x-8.gap-y-8 > div.jsx-1414670988.textarea-wrapper.lg\\:col-span-4.mb-6 > textarea"));
            textArea.Clear();
            textArea.SendKeys("魚進入測試中");
            playButton = driver.FindElement(By.CssSelector("#__next > main > section.jsx-1414670988.mx-auto.max-w-screen-lg.px-2.lg\\:px-0 > div.jsx-1414670988.w-full.flex.flex-col.lg\\:flex-row.justify-between.items-center.lg\\:justify-end.mb-16 > button.jsx-3175807707.button.justify-center.flex.items-center.cursor-pointer.rounded-lg.lg\\:mr-4.mb-4.lg\\:mb-0 > div > span"));
            playButton.Click();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            driver.Quit();
        }

        private void richTextBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Return)
            {
                //richTextBox2.Text = "";
            }
        }

        private void richTextBox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
           if (e.KeyChar == '\r')
            {
                //preproccess
                richTextBox2.Text = richTextBox2.Text.Replace("\r", "");
                if (richTextBox2.Text.Length == 0) return;
                while (richTextBox2.Text.Last() == '\n')
                {
                    richTextBox2.Text = richTextBox2.Text.Remove(richTextBox2.Text.Length - 1);
                }

                richTextBox1.AppendText("我:" + richTextBox2.Text + "\n");
                string output = null;
                if (richTextBox2.Text.Contains("+"))
                {
                    string[] arg = richTextBox2.Text.Split("+", 2);
                    if (!qAndALib.data.ContainsKey(arg[0]))
                    {
                        qAndALib.data.Add(arg[0], new List<string>());
                    }
                    List<string> answer = qAndALib.data[arg[0]];
                    string[] addArray = arg[1].Split("|");
                    if (addArray.Length > 0)
                    {
                        output = $"妹妹已新增";
                        foreach (string a in addArray)
                        {
                            if (!answer.Contains(a))
                            {
                                answer.Add(a);
                                output += a + " ";
                            }
                        }
                        output += "的回應";
                        saveLib();
                    }
                }
                else if (richTextBox2.Text.Contains("-"))
                {
                    string[] arg = richTextBox2.Text.Split("-", 2);
                    if (!qAndALib.data.ContainsKey(arg[0]))
                    {
                        qAndALib.data.Add(arg[0], new List<string>());
                    }
                    List<string> answer = qAndALib.data[arg[0]];
                    string[] removeArray = arg[1].Split("|");
                    if (answer.Contains(arg[1]))
                    {
                        answer.Remove(arg[1]);
                        output = $"妹妹已刪除{arg[1]}的回應";
                        saveLib();
                    }
                    if (removeArray.Length > 0)
                    {
                        output = $"妹妹已刪除";
                        foreach (string r in removeArray)
                        {
                            if (answer.Contains(arg[1]))
                            {
                                answer.Remove(arg[1]);
                                output += r + " ";
                            }
                        }
                        output += "的回應";
                        saveLib();
                    }
                }
                else
                {
                    if (qAndALib.data.ContainsKey(richTextBox2.Text))
                    {
                        var answer = qAndALib.data[richTextBox2.Text];
                        if (answer.Count > 0)
                        {
                            int randomIndex = random.Next(0, answer.Count);
                            output = answer[randomIndex];
                        }
                    }
                }
                if (string.IsNullOrEmpty(output))
                {
                    output = "哥哥你在說什麼？";
                }
                richTextBox1.AppendText("妹妹:" + output + "\n");
                richTextBox1.ScrollToCaret();
                textArea.Clear();
                textArea.SendKeys(output);
                playButton.Click();

                richTextBox2.Text = "";
            }
        }

        void saveLib()
        {
            lock (qAndALib)
            {
                Thread thread = new Thread(() =>
                {
                    string text = JsonConvert.SerializeObject(qAndALib);
                    File.WriteAllText(libFilePath, text);
                });
                thread.Start();
            }
        }
    }
    public class QandALib
    {
        public Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
    }
}