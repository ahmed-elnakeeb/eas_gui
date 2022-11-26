using Csv;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Encodings.Web;
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
using static System.Net.Mime.MediaTypeNames;

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
        int Q=0;
        int sts_size;
        Exam exam;
        Student takeexam_student;
        int takeexam_curent = 1;
        List<exam_result> results=new List<exam_result>();
        bool strted_creating_exam=false;
        bool is_server_working=false;
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8087/");
            entail_size = grid_entailment.Children.Count;
            cola_size= grid_cola.Children.Count;
            sts_size= grid_sts.Children.Count;
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

        private void button_load_asnwers_Click(object sender, RoutedEventArgs e)
        {
            var x =new OpenFileDialog();
            x.Title = "Select a csv file";
            x.Filter = "CSV Files (*.csv)|*.csv|all files (*)|*";
            if (x.ShowDialog() == true)

            {
                textBox_examtab_answers_location.Text = x.FileName;
            }
        }

        private void button_load_question_Click(object sender, RoutedEventArgs e)
        {
            var x = new OpenFileDialog();
            x.Title = "Select a csv file";
            x.Filter = "CSV Files (*.csv)|*.csv|all files (*)|*";
            if (x.ShowDialog() == true)

            {
                textBox_examtab_questions_location.Text = x.FileName;
            }
        }
        private void button_output_path_Click(object sender, RoutedEventArgs e)
        {
            
                var s = new SaveFileDialog();
                s.Filter = "CSV Files (*.csv)|*.csv|all files (*)|*";
            if (s.ShowDialog() == true)
                textBox_examtab_out_location.Text = s.FileName;
            
        }

        private void button_start_Click(object sender, RoutedEventArgs e)
        {
            exam = new Exam();
            List<Student> students_answers=new List<Student>();
            CsvOptions a = new CsvOptions();
            a.HeaderMode = HeaderMode.HeaderAbsent;

            // load students answers
            var csv = File.ReadAllText(textBox_examtab_answers_location.Text);
            foreach (var line in CsvReader.ReadFromText(csv,a))
            {
                // Header is handled, each line will contain the actual row data
                int ID = int.Parse(line[0]);
                int number_of_Q=int.Parse(line[1]);
                List<string> Answers = new List<string>();
                for (int i = 2; i < number_of_Q + 2; i++)
                    Answers.Add(line[i]);

                students_answers.Add(new Student() { ID=ID,Answers= Answers });
            }
            //load model answers
            csv = File.ReadAllText(textBox_examtab_questions_location.Text);
            foreach (var line in CsvReader.ReadFromText(csv, a))
            {
                // Header is handled, each line will contain the actual row data
                int id = int.Parse(line[0]);
                string question = line[1];
                string answer = line[2];

                exam.Questions.Add(new ExamQuestion() { ID = id, Question = question, Answer = answer });
                

            }
            var root =new scoring() { Students=students_answers,exam=exam};
            var xyzf =Newtonsoft.Json.JsonConvert.SerializeObject(root);

            if (textBox_examtab_out_location.Text != "")
            {
                var jsonstring = "api/scoring/?data=" + Uri.EscapeDataString(xyzf);
                //jsonstring+="&outputpath=" + Uri.EscapeDataString(textBox_examtab_out_location.Text);
                response = client.GetAsync(jsonstring).Result;
                using (StreamWriter writer = new StreamWriter(textBox_examtab_out_location.Text))
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    res=res.Remove(0,1);
                    res=res.Remove(res.Length-1,1);
                    res=res.Replace("\\n", "\r\n");

                    writer.Write(res);

                }

            }
            else
            { 
                var jsonstring = "api/scoring/?data=" + Uri.EscapeDataString(xyzf);
                response = client.GetAsync(jsonstring).Result;
            }      

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                //var obj = System.Text.Json.JsonSerializer.Deserialize<ServerModel>(result);
            }

            else
            {
                MessageBox.Show("bad  response");
            }
        }

        private void button_new_Click(object sender, RoutedEventArgs e)
        {

            dataGrid1.ItemsSource = null;
            Q = 0;
            exam=new Exam() { Name=""};

            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            dataGrid1.ItemsSource =exam.Questions;

        }

        private void button_add_Click(object sender, RoutedEventArgs e)
        {
            exam.Questions.Add(new ExamQuestion() { ID = ++Q, Question = "", Answer = "" });
            
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            if (exam.Questions.Count > 0)
            {
                var s = new SaveFileDialog();
                s.ShowDialog();
                save_exam(exam, s.FileName);
            }


        }

        private void button_takeexam_selectexam_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "CSV Files (*.csv)|*.csv|all files (*)|*";
            openfiledialog.Multiselect = false;

            if (openfiledialog.ShowDialog()==true)
            {
                textbox_takeexam_selectexam.Text = openfiledialog.FileName;
            }


        }

        private void button_takeexam_startexam_Click(object sender, RoutedEventArgs e)
        {
            takeexam_curent = 1;
            takeexam_student=new Student() { ID=int.Parse(textbox_takeexam_studentid.Text),Answers= new List<string>() };
            exam = loadexam(textbox_takeexam_selectexam.Text);
            foreach (var item in exam.Questions)
            {
                takeexam_student.Answers.Add("");
            }            
            refresh_takeexam();

        }

        private void button_takeexam_next_Click(object sender, RoutedEventArgs e)
        {
            if (takeexam_curent<exam.Questions.Count)
                ++takeexam_curent;
            refresh_takeexam();

        }

        private void button_takeexam_last_Click(object sender, RoutedEventArgs e)
        {
            if (takeexam_curent > 1)
                --takeexam_curent;
            refresh_takeexam();
        }

        private void textbox_takeexam_answer_TextChanged(object sender, TextChangedEventArgs e)
        {
            takeexam_student.Answers[takeexam_curent - 1] =textbox_takeexam_answer.Text;
        }
        public void refresh_takeexam()
        {
            textblock_takeexam_question_number.Text = string.Format("question {0}/{1}", takeexam_curent, exam.Questions.Count.ToString());

            textbox_takeexam_question.Text = exam.Questions[takeexam_curent - 1].Question;

            textbox_takeexam_answer.Text = takeexam_student.Answers[takeexam_curent - 1];

        }

        private void button_takeexam_submit_Click(object sender, RoutedEventArgs e)
        {
            if (exam != null && takeexam_student != null)
            {
                // load students answers
                var students_answers = new List<Student>();
                students_answers.Add(takeexam_student);

                //load model answers
               

                var root = new scoring() { Students = students_answers, exam = exam };
                var xyzf = Newtonsoft.Json.JsonConvert.SerializeObject(root);
                SaveFileDialog save = new SaveFileDialog();
                save.Filter =  "CSV Files (*.csv)|*.csv|all files (*)|*";
                if(save.ShowDialog()==true)
                {
                    var jsonstring = "api/scoring/?data=" + Uri.EscapeDataString(xyzf) + "&outputpath=" + Uri.EscapeDataString(save.FileName);
                    response = client.GetAsync(jsonstring).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        using (StreamWriter writer = new StreamWriter(textBox_examtab_out_location.Text))
                        {
                            string res = response.Content.ReadAsStringAsync().Result;
                            res = res.Remove(0, 1);
                            res = res.Remove(res.Length - 1, 1);
                            res = res.Replace("\\n", "\r\n");

                            writer.Write(res);

                        }
                        //var obj = System.Text.Json.JsonSerializer.Deserialize<ServerModel>(result);
                    }

                    else
                    {
                        MessageBox.Show("bad  response");
                    }
                }

                
                

            }
            else
            {
                MessageBox.Show("not enogh data");
            }
        }

        private void button_browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            textBox_browse.Text = openFileDialog.FileName;

            results = new List<exam_result>();
            CsvOptions a = new CsvOptions();
            a.HeaderMode = HeaderMode.HeaderPresent;

            // load students answers
            var csv = File.ReadAllText(openFileDialog.FileName);
            foreach (var line in CsvReader.ReadFromText(csv, a))
            {
                // Header is handled, each line will contain the actual row data
                int sid = int.Parse(line[0]);
                int qid = int.Parse(line[1]);
                string Answer=line[2];
                double total =float.Parse(line[3]);
                double sts=float.Parse(line[4]);
                string paraphrase=line[5];
                string entailment=line[6];
                double cola=float.Parse(line[7]);
                results.Add(new exam_result()
                {
                    SID = sid,
                    QID = qid,
                    Answer = Answer,
                    Total = total,
                    STS = sts,
                    Paraphrase = paraphrase,
                    Entailment = entailment,
                    COLA = cola
                });


            }
            dataGrid_results.ItemsSource = results;


        }

 
        private void change_style_textblock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var tmp = tap_create_exam_1.Content;
            tap_create_exam_1.Content = tap_createexam_2.Content;
            tap_createexam_2.Content = tmp;
                       
        }

        private void button_createexame_2_add_Click(object sender, RoutedEventArgs e)
        {
            if (strted_creating_exam==false)
            {
                exam = new Exam();
                strted_creating_exam = true;
                Q = 0;
            }
            if (textBox_createexam_2_question.Text!="" && textbox_createexam_2_answer.Text!="")
            {
                exam.Questions.Add(new ExamQuestion() { ID=Q++,Question=textBox_createexam_2_question.Text,
                    Answer= textbox_createexam_2_answer.Text });
                textBox_createexam_2_question.Text = "";
                textbox_createexam_2_answer.Text = "";

            }
        }

        private void button_createexame_2_clear_Click(object sender, RoutedEventArgs e)
        {
            strted_creating_exam=false;
            exam = new Exam();
            Q = 0;

        }

        private void button_createexame_2_save_Click(object sender, RoutedEventArgs e)
        {
            if (exam.Questions.Count>0)
            {
                var s = new SaveFileDialog();
                if (s.ShowDialog()==true)
                save_exam(exam,s.FileName);
            }
        }

        private void button_test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("working");
                    is_server_working=true;
                }
                else
                {
                    MessageBox.Show("not working");
                }
            }
            catch
            {
                MessageBox.Show("no such server");
            }
        }

        private void button_set_Click(object sender, RoutedEventArgs e)
        {
            if (is_server_working)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri(textBox_server.Text);
                MessageBox.Show("done");
            }
            else
                MessageBox.Show("somthing went wrong test the server first");
        }
    }
}