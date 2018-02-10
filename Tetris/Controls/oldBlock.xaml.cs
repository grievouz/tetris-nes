using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
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

namespace Tetris.Controls
{
    /// <summary>
    /// Interaction logic for oldBlock.xaml
    /// </summary>
    public partial class oldBlock : UserControl
    {

        private int _rows;
        private int _cols;
        private Label[,] _blockControls;
        private bool Shiny = false;
        public new Brush Background;

        public oldBlock()
        {
            InitializeComponent();

            _rows = MainGrid.RowDefinitions.Count;
            _cols = MainGrid.ColumnDefinitions.Count;

            _blockControls = new Label[_cols, _rows];

            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    _blockControls[i, j] = new Label();
                    Grid.SetRow(_blockControls[i, j], j);
                    Grid.SetColumn(_blockControls[i, j], i);
                    MainGrid.Children.Add(_blockControls[i, j]);
                    _blockControls[i, j].BorderBrush = Brushes.Black;
                    _blockControls[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                }
            }
        }

        public void SetPreviewShape(Point[] shape, Brush color)
        {
            foreach (var point in shape)
            {
                _blockControls[(int) point.X + 1, (int) point.Y + 1].Background = color;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    _blockControls[i, j].Background = Brushes.Transparent;
                }
            }
        }
    }
}
