using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eas_gui
{
    internal class eas_tools
    {
        public class Exam
        {
            public string Name { get; set; } = "";
            public List<Question> Questions = new List<Question>();

            public DateTime Created_on { get; set; }

            public string Details
            {
                get
                {
                    return String.Format("{1} questions, created on\n{2}.", this.Name, this.Questions.Count, this.Created_on.ToString("g"));
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        public class Question
        {
            public String Q { get; set; }
            public String A { get; set; }


        }

    }
}
