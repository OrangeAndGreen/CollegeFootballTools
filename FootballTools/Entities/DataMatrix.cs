using System.Drawing;

namespace FootballTools.Entities
{
    public class DataMatrix
    {
        public string[] ColumnHeaders { get; set; }
        public string[] RowHeaders { get; set; }
        public string[,] Labels { get; set; }
        public Color[,] Colors { get; set; }

        public DataMatrix(string[,] labels, Color[,] colors)
        {
            Labels = labels;
            Colors = colors;
        }

        public DataMatrix(string[] rowHeaders, string[] columnHeaders, string[,] labels, Color[,] colors)
        {
            RowHeaders = rowHeaders;
            ColumnHeaders = columnHeaders;
            Labels = labels;
            Colors = colors;
        }
    }
}
