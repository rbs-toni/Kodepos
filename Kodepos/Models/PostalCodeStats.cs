
namespace Kodepos.Models;
public record PostalCodeStats
{
    public int ProvinceCount { get; set; }
    public int DistrictCount { get; set; }
    public int RegencyCount { get; set; }
    public int VillageCount { get; set; }
}
