
namespace Schedulr.Models
{
    /// <summary>
    /// Determines the type of map to use for geographic locations.
    /// </summary>
    public enum GeoLocationMapProvider
    {
        /// <summary>
        /// The Bing map (road view).
        /// </summary>
        BingRoad,

        /// <summary>
        /// The Bing map (satellite view).
        /// </summary>
        BingSatellite,

        /// <summary>
        /// The Bing map (hybrid view).
        /// </summary>
        BingHybrid,

        /// <summary>
        /// The Yahoo map (road view).
        /// </summary>
        YahooRoad,

        /// <summary>
        /// The Yahoo map (satellite view).
        /// </summary>
        YahooSatellite,

        /// <summary>
        /// The Yahoo map (hybrid view).
        /// </summary>
        YahooHybrid,
        /// <summary>
        /// 
        /// </summary>
        GoogleRoad,
        /// <summary>
        /// 
        /// </summary>
        GoogleSatellite,
        /// <summary>
        /// 
        /// </summary>
        GoogleTerrain,
        /// <summary>
        /// 
        /// </summary>
        GoogleHybrid,
        /// <summary>
        /// 
        /// </summary>
        OviRoad,
        /// <summary>
        /// 
        /// </summary>
        OviSatellite,
        /// <summary>
        /// 
        /// </summary>
        OviTerrain,
        /// <summary>
        /// 
        /// </summary>
        OviHybrid,
        /// <summary>
        /// Yandex Map
        /// </summary>
        YandexRoad,
        /// <summary>
        /// 
        /// </summary>
        YandexSatellite,
        /// <summary>
        /// 
        /// </summary>
        YandexHybrid,

        /// <summary>
        /// The OpenStreetMap map (road view).
        /// </summary>
        OpenStreetMapRoad,
        /// <summary>
        /// ClodMade Map
        /// </summary>
        CloudMadeRoad
    }
}