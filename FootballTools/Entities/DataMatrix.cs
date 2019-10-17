using System.Drawing;

namespace FootballTools.Entities
{
    public class DataMatrix
    {
        public string[,] Labels { get; set; }
        public Color[,] Colors { get; set; }

        public DataMatrix(string[,] labels, Color[,] colors)
        {
            Labels = labels;
            Colors = colors;
        }
    }
}
