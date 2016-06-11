using FluentAssertions;
using LocationAlarm.Location;
using System;
using Windows.Devices.Geolocation;
using Xunit;

namespace LocationAlarmsTests.Location
{
    public class GeolocationModelTests
    {
        private GeolocationModel _geolocationModel;
        private Geopoint _geopoint;
        private BasicGeoposition _geoposition = new BasicGeoposition { Latitude = 48.86681, Longitude = 2.333444 };
        private string _locationName = "Paryż";

        public GeolocationModelTests()
        {
            _geolocationModel = new GeolocationModel();

            _geopoint = new Geopoint(_geoposition);
        }

        [Fact]
        public async void FindBestMatchedLocationNameAsync_GivenGeopointInParisAndDontKeepContext_ShouldReturnParis()
        {
            var result = await _geolocationModel.FindBestMatchedLocationNameAsync(_geopoint).ConfigureAwait(false);

            result.Should().NotBeNullOrEmpty().And.Contain(_locationName);
        }

        [Fact]
        public async void FindBestMatchedLocationNameAsync_GivenGeopointInParisAndKeepContext_ShouldReturnParis()
        {
            var result = await _geolocationModel.FindBestMatchedLocationNameAsync(_geopoint).ConfigureAwait(true);

            result.Should().NotBeNullOrEmpty().And.Contain(_locationName);
        }

        [Fact]
        public async void FindBestMatchedLocationNameAsync_NullInput_ShouldReturnEmptyString()
        {
            var result = await _geolocationModel.FindBestMatchedLocationNameAsync(_geopoint).ConfigureAwait(false);

            result.Should().BeEmpty();
        }
    }
}