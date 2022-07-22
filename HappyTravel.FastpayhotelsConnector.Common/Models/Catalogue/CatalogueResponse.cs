namespace HappyTravel.FastpayhotelsConnector.Common.Models.Catalogue;

public class CatalogueResponse
{
    public List<ServiceType> ServiceTypes { get; set; }
    public List<CatalogueType> ImageTypes { get; set; }
    public List<CatalogueType> HotelCategoryTypes { get; set; }
    public List<CatalogueType> HotelTypes { get; set; }
    public List<CatalogueType> ServiceCategoryTypes { get; set; }
    public List<CatalogueType> MealPlanTypes { get; set; }
    public List<CatalogueType> BedTypes { get; set; }
    public List<CatalogueType> HostSegment { get; set; }
    public List<CatalogueType> Countries { get; set; }
    public List<CatalogueType> Tags { get; set; }
    public List<CatalogueType> RoomTypes { get; set; }
    public List<TuristicAreaType> TuristicAreas { get; set; }
}
