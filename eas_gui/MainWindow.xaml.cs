using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static eas_gui.eas_tools;

namespace eas_gui
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client;
        HttpResponseMessage response;
        int entail_size;
        int cola_size;

        
        int sts_size;
        int para_size;
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8087/");
            entail_size = grid_entailment.Children.Count;
            cola_size= grid_cola.Children.Count;
            sts_size= grid_sts.Children.Count;
            para_size= grid_para.Children.Count;
        }


        private void button_new_Click(object sender, RoutedEventArgs e)
        {
            Question question = new Question() {Q="what is the name of the sun",A="the sun!" };

            Question question2 = new Question() { Q = "what is the name of the sun", A = "the sun!" };


            var exam = new Exam() {Name="ahmed" ,Questions=new List<Question>() { question ,question2} , Created_on =DateTime.Now};          
            dataGrid.Items.Add(exam);
        }
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dataGrid.SelectedItems.Count ==1)
            {
                Exam exam = new Exam();
                foreach (var obj in dataGrid.SelectedItems)
                {
                    exam = obj as Exam;
                    var questions = exam.Questions;
                    dataGrid1.Items.Clear();
                    foreach (var item in questions)
                        dataGrid1.Items.Add(item);
                }
            }
            else
            {
            }
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Items.Remove(dataGrid.SelectedItem);
        }
        
        private void button_from_csv_Click(object sender, RoutedEventArgs e)
        {
            Exam exam=new Exam();
            List<Question> questions = new List<Question>();


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            exam.Created_on = DateTime.Now;
            exam.Name=openFileDialog.FileName;
            using (var reader = new StreamReader(openFileDialog.FileName))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    questions.Add(new Question() { Q = values[0], A = values[1] });




                }

            }
            exam.Questions = questions;
            dataGrid.Items.Add(exam);
        }

        private void calculate_score_Click(object sender, RoutedEventArgs e)
        {


            double total = 0;


            string s1 = textBox_essay1.Text;
            string s2 = textBox_essay2.Text;

            string sts = "api/sts/" + s1 + "/" + s2;
            string paraphrase = "api/paraphrase_detection/" + s1 + "/" + s2;
            string cola1 = "api/cola/" + s1;
            string cola2 = "api/cola/" + s2;
            string sentiment_analysis1 = "api/sentiment_analysis/" + s1;
            string sentiment_analysis2 = "api/sentiment_analysis/" + s2;
            string classification = "api/classification/" + s1 + "/" + s2;
            string sintax = "api/sintax/" + s2;




            response = client.GetAsync(sts).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                double score = double.Parse(result);
                //var obj = System.Text.Json.JsonSerializer.Deserialize<ServerModel>(result);
                textBlock_semantic_similarity.Text = "semantic similarity: " + score;
                total = score;

            }

            else
            {
                MessageBox.Show("bad sts response");
            }

            //response = client.GetAsync(paraphrase).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    string result = response.Content.ReadAsStringAsync().Result;
            //    float score = float.Parse(result);
            //    //var obj = System.Text.Json.JsonSerializer.Deserialize<ServerModel>(result);
            //    textBlock_Paraphrase.Text = "para: " + score;
            //}

            response = client.GetAsync(cola1).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                float score = float.Parse(result);
                textBlock_Linguistic_Acceptability.Text = "Linguistic_Acceptability: " + score;
            }
            else
            {
                MessageBox.Show("bad cola1 response");
            }


            response = client.GetAsync(cola2).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                float score = float.Parse(result);
                textBlock_Linguistic_Acceptability_2.Text = "Linguistic_Acceptability: " + score;
                if (score > .7)
                    total = total * 1.05;
                else if (score < .4)
                    total = total * 0.95;
            }
            else
            {
                MessageBox.Show("bad cola2 response");
            }

            response = client.GetAsync(sentiment_analysis1).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                textBlock_sentiment_analysis.Text = "sentiment_analysis1: " + result.Replace("\"", "");
            }
            else
            {
                MessageBox.Show("bad sentiment_analysis response");
            }

            response = client.GetAsync(sentiment_analysis2).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                textBlock_sentiment_analysis_2.Text = "sentiment_analysis: " + result.Replace("\"", "");
                if (result.Replace("\"", "") == "good")
                    total = total * 1.05;
                else if (result.Replace("\"", "") == "bad")
                    total = total * .95;

            }
            else
            {
                MessageBox.Show("bad sentiment_analysis2 response");
            }

            response = client.GetAsync(paraphrase).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                textBlock_Paraphrase.Text = "Paraphrase: " + result.Replace("\"", "");
                if (result.Replace("\"", "") == "good")
                    total = total * 1.05;
                else if (result.Replace("\"", "") == "bad")
                    total = total * .95;

            }
            else
            {
                MessageBox.Show("bad sentiment_analysis2 response");
            }

            if (total > 5)
                total = 5;
            total = Math.Round(total, 3);
            textBlock_total.Text = total.ToString();



        }

        private void button_cola_submit_Click(object sender, RoutedEventArgs e)
        {

            grid_cola.Children.RemoveRange(cola_size, 12);
                foreach (var tbox in new List<TextBox> {textBox_cola1, textBox_cola2, textBox_cola3, textBox_cola4, textBox_cola5, textBox_cola6, textBox_cola7,
                textBox_cola8, textBox_cola9,textBox_cola10,textBox_cola11,textBox_cola12 })
            {
                if (tbox.Text == "")
                    continue;
                String slag = "api/cola/" + tbox.Text;
                Label label = new Label();
                label.Name = "NewLabel";
                label.Width = 240;
                label.Height = 30;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                Thickness th = tbox.Margin;
                th.Left = 400;
                label.Margin = th;
                label.Foreground = new SolidColorBrush(Colors.Black);

                grid_cola.Children.Add(label);


                response = client.GetAsync(slag).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    label.Content = "Linguistic Acceptability: " + result.Replace("\"", "");
                }
            }


        }

        private void button_entailment1_Click(object sender, RoutedEventArgs e)
        {

            grid_entailment.Children.RemoveRange(entail_size, 12);
            foreach (var tbox in new List<TextBox> {textBox_entailment1, textBox_entailment2, textBox_entailment3, textBox_entailment4, textBox_entailment5, textBox_entailment6, textBox_entailment7,
                textBox_entailment8, textBox_entailment9,textBox_entailment10,textBox_entailment11,textBox_entailment12 })
            {
                if (tbox.Text == "")
                    continue;
                String slag = "api/sentiment_analysis/" + tbox.Text;
                Label label = new Label();
                label.Name = "NewLabel";
                label.Width = 240;
                label.Height = 30;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                Thickness th = tbox.Margin;
                th.Left = 400;
                label.Margin = th;
                label.Foreground = new SolidColorBrush(Colors.Black);

                grid_entailment.Children.Add(label);


                response = client.GetAsync(slag).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    label.Content = "entailment : " + result.Replace("\"", "");
                }
            }
        }

        private void button_tab_stsb_Click(object sender, RoutedEventArgs e)
        {
            grid_sts.Children.RemoveRange(sts_size, 12);
            var tboxs = new List<TextBox> {textBox_tab_stsb1, textBox_tab_stsb2, textBox_tab_stsb3, textBox_tab_stsb4, textBox_tab_stsb5, textBox_tab_stsb6, textBox_tab_stsb7,
                textBox_tab_stsb8, textBox_tab_stsb9,textBox_tab_stsb10,textBox_tab_stsb11,textBox_tab_stsb12 };
            for (int i = 0; i < 12; i+=2)
            {
                TextBox tbox = tboxs[i];
                TextBox tbox2 = tboxs[i+1];

                if (tbox.Text  == "" || tbox2.Text=="" )
                    continue;
                String slag = "api/sts/" + tbox.Text + "/" + tbox2.Text;
                Label label = new Label();
                label.Name = "NewLabel";
                label.Width = 240;
                label.Height = 30;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                Thickness th = tbox.Margin;
                th.Left = 400;
                label.Margin = th;
                label.Foreground = new SolidColorBrush(Colors.Black);

                grid_sts.Children.Add(label);


                response = client.GetAsync(slag).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    label.Content = "sts: " + result.Replace("\"", "");
                }
            }
        }

        private void button_tab_para_Click(object sender, RoutedEventArgs e)
        {
            grid_para.Children.RemoveRange(sts_size, 12);
            var tboxs = new List<TextBox> {textBox_tab_para1, textBox_tab_para2, textBox_tab_para3, textBox_tab_para4, textBox_tab_para5, textBox_tab_para6, textBox_tab_para7,
                textBox_tab_para8, textBox_tab_para9,textBox_tab_para10,textBox_tab_para11,textBox_tab_para12 };


            for (int i = 0; i < 12; i += 2)
            {
                TextBox tbox = tboxs[i];
                TextBox tbox2 = tboxs[i + 1];

                if (tbox.Text == "" || tbox2.Text == "")
                    continue;
                String slag = "api/paraphrase_detection/" + tbox.Text + "/" + tbox2.Text;
                Label label = new Label();
                label.Name = "NewLabel";
                label.Width = 240;
                label.Height = 30;
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                Thickness th = tbox.Margin;
                th.Left = 400;
                label.Margin = th;
                label.Foreground = new SolidColorBrush(Colors.Black);

                grid_para.Children.Add(label);


                response = client.GetAsync(slag).Result;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    label.Content = "para: " + result.Replace("\"", "");
                }
            }
        }
    }
    }



