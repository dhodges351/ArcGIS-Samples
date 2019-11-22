using Esri.ArcGISRuntime.UI;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Tasks.Geocoding;

namespace AddressSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = this.FindResource("MapViewModel") as MapViewModel;
            _viewModel.MapView = MyMapView;
        }

        // Map initialization logic is contained in MapViewModel.cs
        private void AddressTextChanged(object sender, TextChangedEventArgs e)
        {
            string address = AddressTextBox.Text.Trim();

            if (string.IsNullOrEmpty(address) || address.Length < 3)
            {
                return;
            }

            SuggestResult suggestion = SuggestionList.SelectedItem as SuggestResult;
            if (suggestion != null && suggestion.Label == address) { return; }

            _viewModel.GetAddressSuggestions(address);
        }

        private void SuggestionChosen(object sender, SelectionChangedEventArgs e)
        {
            SuggestResult suggestion = SuggestionList.SelectedItem as SuggestResult;
            if (suggestion == null) { return; }

            AddressTextBox.Text = suggestion.Label;

            _viewModel.ShowAddressLocation(suggestion);
        }

    }
}
