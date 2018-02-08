using System;
using System.Collections.Generic;
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
        private Grid[,] _blockControls;
        private bool Shiny = false;
        public new Brush Background;

        public oldBlock()
        {
            InitializeComponent();

            _rows = MainGrid.RowDefinitions.Count;
            _cols = MainGrid.ColumnDefinitions.Count;

            _blockControls = new Grid[_cols, _rows];

            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    _blockControls[i, j] = new Grid();
                    Grid.SetRow(_blockControls[i, j], j);
                    Grid.SetColumn(_blockControls[i, j], i);
                    MainGrid.Children.Add(_blockControls[i, j]);
                }
            }
        }

        public void SetColor(Brush backgroundBrush)
        {
            Background = backgroundBrush;
            MainGrid.Background = backgroundBrush;
            SetBlocky();
        }

        public void SetShiny()
        {
            Reset();

            if(Shiny)
                _blockControls[0, 0].Background = Brushes.White;

            _blockControls[1, 1].Background = Brushes.White;
            _blockControls[1, 2].Background = Brushes.White;
            _blockControls[2, 1].Background = Brushes.White;
        }

        public void SetBlocky()
        {
            Reset();

            if (Shiny)
                _blockControls[0, 0].Background = Brushes.White;

            _blockControls[1, 1].Background = Brushes.White;
            _blockControls[1, 2].Background = Brushes.White;
            _blockControls[1, 3].Background = Brushes.White;

            _blockControls[2, 1].Background = Brushes.White;
            _blockControls[2, 2].Background = Brushes.White;
            _blockControls[2, 3].Background = Brushes.White;

            _blockControls[3, 1].Background = Brushes.White;
            _blockControls[3, 2].Background = Brushes.White;
            _blockControls[3, 3].Background = Brushes.White;

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
