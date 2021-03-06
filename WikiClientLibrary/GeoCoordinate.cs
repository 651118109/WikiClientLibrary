﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WikiClientLibrary
{
    /// <summary>
    /// Represents a coordinate on the globe.
    /// </summary>
    /// <remarks>This structure uses degrees as the unit of the coordinate.</remarks>
    public struct GeoCoordinate : IEquatable<GeoCoordinate>
    {

        /// <summary>
        /// The globe identifier of the Earth.
        /// </summary>
        /// <seealso cref="Globe"/>
        public const string Earth = "earth";

        /// <summary>
        /// Gets an <see cref="GeoCoordinate"/> value with its members uninitialized.
        /// </summary>
        public static readonly GeoCoordinate Empty = new GeoCoordinate();

        /// <summary>Initializes a new instance of <see cref="GeoCoordinate"/> representing a location on the Earth.</summary>
        /// <inheritdoc cref="GeoCoordinate(double,double,double,string)"/>
        /// <remarks>The <see cref="Dimension"/> is set to <c>0</c>.</remarks>
        public GeoCoordinate(double latitude, double longitude) : this(latitude, longitude, 0, Earth)
        {
        }

        /// <summary>Initializes a new instance of <see cref="GeoCoordinate"/> representing a location on the Earth.</summary>
        /// <inheritdoc cref="GeoCoordinate(double,double,double,string)"/>
        public GeoCoordinate(double latitude, double longitude, double dimension) : this(latitude, longitude, dimension, Earth)
        {
        }

        /// <param name="latitude">latitude of the location.</param>
        /// <param name="longitude">longitude of the location.</param>
        /// <param name="dimension">size of the object.</param>
        /// <param name="globe">globe identifier of the coordinate.</param>
        public GeoCoordinate(double latitude, double longitude, double dimension, string globe)
        {
            Longitude = longitude;
            Latitude = latitude;
            Dimension = dimension;
            Globe = globe;
        }

        /// <summary>
        /// Gets/sets the latitude of the location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets/sets the longitude of the location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets/sets the size of the object.
        /// </summary>
        public double Dimension { get; set; }

        /// <summary>
        /// Gets/set the globe identifier of the coordinate.
        /// </summary>
        /// <seealso cref="Earth"/>
        public string Globe { get; set; }

        /// <summary>
        /// Gets a value that indicates if all the members in the structure have their uninitialized values.
        /// </summary>
        public bool IsEmpty => Globe == null && Longitude.Equals(0) && Latitude.Equals(0) && Dimension.Equals(0);

        /// <summary>
        /// Offsets the coordinates by the specified values.
        /// </summary>
        /// <param name="longitudeOffset">Offset of the longitude.</param>
        /// <param name="latitudeOffset">Offset of the latitude.</param>
        public void Offset(double longitudeOffset, double latitudeOffset)
        {
            Longitude += longitudeOffset;
            Latitude += latitudeOffset;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var s = Latitude.ToString();
            if (Latitude >= 0) s += "°N ";
            else s += "°S ";
            s += Longitude;
            if (Longitude >= 0) s += "°E";
            else s += "°W";
            return s;
        }

        /// <inheritdoc />
        public bool Equals(GeoCoordinate other)
        {
            return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude) && Dimension.Equals(other.Dimension) && string.Equals(Globe, other.Globe);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GeoCoordinate coordinate && Equals(coordinate);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Longitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Dimension.GetHashCode();
                hashCode = (hashCode * 397) ^ (Globe != null ? Globe.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(GeoCoordinate left, GeoCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GeoCoordinate left, GeoCoordinate right)
        {
            return !left.Equals(right);
        }
    }
}
