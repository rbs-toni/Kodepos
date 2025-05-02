namespace Kodepos.Models
{
    public record LatLng
    {
        public LatLng()
        {

        }
        public LatLng(float lat, float lng)
        {
            Lat = lat;
            Lng = lng;
        }
        public float Lat { get; set; }
        public float Lng { get; set; }
    }
}
