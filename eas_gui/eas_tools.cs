using Csv;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static eas_gui.eas_tools;

namespace eas_gui
{
    internal static class eas_tools

    {


        public  class Exam
        {
            public string Name { get; set; } = "";
            public List<ExamQuestion> Questions = new List<ExamQuestion>();       
        }


        public class ExamQuestion
        {
            public int ID { get; set; } = 0;
            public string Question { get; set; } = "";
            public string Answer { get; set; }
        }

        public class Student
        {
            public int ID { get; set; }
            public List<string> Answers { get; set; }= new List<string>();
        }


        public class scoring
        {
            public List<Student> Students { get; set; }=new List<Student>();
            public Exam exam { get; set; }

        }


        [DataContract]
        public class Result
        {
            public int id { get; set; }
            public List<Score> scores { get; set; }
        }


        [DataContract]
        public class Root_Result

        {
            public List<Result>?  results { get; set; }
        }

        [DataContract]
        public class Score
        {
            public int? total { get; set; }
            public double? sts { get; set; }
            public string? paraphrase { get; set; }
            public string? entail { get; set; }
            public double? cola { get; set; }
        }
        public class exam_result
        {
            public int? SID { get; set; }
            public double? QID { get; set; }
            public string? Answer { get; set; }
            public double? Total { get; set; }
            public double? STS { get; set; }
            public string? Paraphrase { get; set; }
            public string? Entailment { get; set; }
            public double? COLA { get; set; }
        }
        public static void save_exam(Exam exam,string FileName)
        {
            var csv = new StringBuilder();

            foreach (var question in exam.Questions)
            {
                var Q = question.ID;
                var quest = question.Question;
                var answer = question.Answer;

                //Suggestion made by KyleMit
                var newLine = string.Format("{0},{1},{2}", Q, quest, answer);
                csv.AppendLine(newLine);

            }
            File.WriteAllText(FileName, csv.ToString());
        }

        public static Exam loadexam(string FileName) {
            Exam exam = new Exam();
            CsvOptions a = new CsvOptions();
            a.HeaderMode = HeaderMode.HeaderAbsent;
            var csv = File.ReadAllText(FileName);
            foreach (var line in CsvReader.ReadFromText(csv, a))
            {
                // Header is handled, each line will contain the actual row data
                exam.Questions.Add(new ExamQuestion()
                {
                    ID = int.Parse(line[0]),
                    Question = line[1],
                    Answer = line[2]
                });
                
            }
        return exam;
        }
    }
    


}
