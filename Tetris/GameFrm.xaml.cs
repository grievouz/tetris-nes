using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for GameFrm.xaml
    /// </summary>
    public partial class GameFrm : Window
    {
        private Game TetrisGame;

        public GameFrm()
        {
            InitializeComponent();
            TetrisGame = new Game(MainGame);
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            TetrisGame.KeyHandler(e);
        }
    }
}
