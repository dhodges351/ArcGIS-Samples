using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Tasks.Geocoding;

namespace AddressSearch
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {
            _geocoder = new LocatorTask(new Uri("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer"));
            _geocoder.LoadAsync();
            _map.InitialViewpoint = new Viewpoint(34.05293, -118.24368, 600000);
        }

        public Esri.ArcGISRuntime.UI.Controls.MapView MapView;
        private LocatorTask _geocoder;

        private IReadOnlyList<SuggestResult> _suggestions;
        public IReadOnlyList<SuggestResult> AddressSuggestions
        {
            get { return _suggestions; }
            set
            {
                _suggestions = value;
                OnPropertyChanged();
            }
        }

        private Map _map = new Map(Basemap.CreateStreets());

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void GetAddressSuggestions(string searchText)
        {
            if (_geocoder.LocatorInfo.SupportsSuggestions)
            {
                Geometry currentExtent = MapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry).TargetGeometry;
                SuggestParameters suggestParams = new SuggestParameters { MaxResults = 10, SearchArea = currentExtent };
                IReadOnlyList<SuggestResult> suggestions = await _geocoder.SuggestAsync(searchText, suggestParams);
                AddressSuggestions = suggestions;
            }
        }

        public async void ShowAddressLocation(SuggestResult suggestion)
        {
            IReadOnlyList<GeocodeResult> matches = await _geocoder.GeocodeAsync(suggestion);
            GeocodeResult bestMatch = (from match in matches orderby match.Score select match).FirstOrDefault();
            if (bestMatch == null) { return; }

            GraphicsOverlay matchOverlay = MapView.GraphicsOverlays.FirstOrDefault();
            matchOverlay.Graphics.Clear();

            Graphic matchGraphic = new Graphic(bestMatch.DisplayLocation);
            matchGraphic.Symbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Cyan, 16);
            matchOverlay.Graphics.Add(matchGraphic);

            MapView.SetViewpointAsync(new Viewpoint(bestMatch.DisplayLocation, 24000));
        }
    }
}
