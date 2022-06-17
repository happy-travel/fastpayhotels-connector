using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Models.Catalogue;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace HappyTravel.FastpayhotelsConnector.Updater.Workers;

public class CatalogueLoader : IUpdateWorker
{
    public CatalogueLoader(FastpayhotelsContentClient client, FastpayhotelsSerializer serializer, FastpayhotelsContext context)
    {
        _client = client;
        _serializer = serializer;
        _context = context;
    }
    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            var catalogues = await _client.GetCatalogue(new CatalogueRequest() { Languages = Constants.Languages.ToList() }, cancellationToken);            

            var existCatalogues = new List<Catalogue>(); // await _context.Catalogues.ToListAsync();

            var temp = catalogues.GetType().GetProperties();

            foreach (var catalog in catalogues.GetType().GetProperties())
            {
                var catalogValues = catalog.GetValue(catalogues);

                var catalogType = catalog.PropertyType.GenericTypeArguments.Single();
                var fieldName = catalog.Name;                

                MethodInfo method = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.Name == "AddUpdateType");
                method = method.MakeGenericMethod(catalogType);
                Task invoke = (Task)method.Invoke(this, new object[4] { catalogValues, existCatalogues, GetTypeEnum(fieldName), cancellationToken });      
                await invoke;
            }

            //await AddUpdateType(catalogues.ServiceTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.ImageTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.HotelCategoryTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.HotelTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.ServiceCategoryTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.MealPlanTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.BedTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.HostSegment, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.Countries, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.Tags, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.RoomTypes, existCatalogues, CatalogType.ServiceTypes, cancellationToken);

            //await AddUpdateType(catalogues.TuristicAreas, existCatalogues, CatalogType.ServiceTypes, cancellationToken);
        }
        catch (Exception ex)
        {

        }
    }


    private CatalogType GetTypeEnum(string name)
        => name switch
        {
            "ServiceTypes" => CatalogType.ServiceType,
            "ImageTypes" => CatalogType.ImageType,
            "HotelCategoryTypes" => CatalogType.HotelCategoryType,
            "HotelTypes" => CatalogType.HotelType,
            "ServiceCategoryTypes" => CatalogType.ServiceCategoryType,
            "MealPlanTypes" => CatalogType.MealPlanType,
            "BedTypes" => CatalogType.BedType,
            "HostSegment" => CatalogType.HostSegment,
            "Countries" => CatalogType.Country,
            "Tags" => CatalogType.Tag,
            "RoomTypes" => CatalogType.RoomType,
            "TuristicAreas" => CatalogType.TuristicArea
        };


    private async Task AddUpdateType<T>(List<T> types, List<Catalogue> catalogues, CatalogType catalogType, CancellationToken cancellationToken) where T : CatalogueType
    {
        try
        {
            var jh = _serializer.Serialize(types);
            var test = types.GroupBy(x => x.Code).Where(y => y.Count() > 1).ToList();

            if (!test.Any() && types.Any())
            {
                //_context.Catalogues.AddRange(types
                //.Where(x => !catalogues.Any(y => y.Code == x.Code && y.Type == catalogType))
                //.Select(r => ConvertToCatalogueEntity(r, catalogType)));

                //_context.Catalogues.UpdateRange(types
                //    .Where(x => catalogues.Any(y => y.Code == x.Code && y.Type == catalogType))
                //    .Select(r => ConvertToCatalogueEntity(r, catalogType)));

                await _context.SaveChangesAsync(cancellationToken);
                _context.ChangeTracker.Clear();
            }
            
        }
        catch (Exception ex)
        {

        }
        
    }


    private Catalogue ConvertToCatalogueEntity<T>(T type, CatalogType catalogType) where T : CatalogueType
        => new Catalogue
        {
            Code = type.Code,
            Data = _serializer.Serialize(type),
            Type = catalogType,
        };





    private readonly FastpayhotelsContentClient _client;
    private readonly FastpayhotelsSerializer _serializer;
    private readonly FastpayhotelsContext _context;
}
