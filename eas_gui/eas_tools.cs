using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eas_gui
{
    internal class eas_tools
    {
        

        public class Exam
        {
            public string Name { get; set; } = "";
            public List<ExamQuestion> Questions = new List<ExamQuestion>();       
        }

        /////////////////////////////////////////////////////////////////////////////////

        public class ExamQuestion
        {
            public int ID { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }
        public class Root
        {
            public List<Student> Students { get; set; }
            public List<ModelAnswer> Model_Answers { get; set; }
        }
        public class Student
        {
            public int ID { get; set; }
            public List<string> Answers { get; set; }
        }
        public class ModelAnswer
        {
            public int ID;
            public string Answer;
        }

        public class scoring
        {
            public List<Student> Students { get; set; }
            public List<ModelAnswer> Model_Answers { get; set; }

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
    }




}
