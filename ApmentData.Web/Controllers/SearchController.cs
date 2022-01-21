using ApmentData.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApmentData.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IElasticClient _elasticClient;

        public SearchController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public IActionResult Index()
        {
            //Here I request all markets from all indeses to add them as scopes.
            var response = _elasticClient.Search<Scope>(s => s
            .AllIndices().From(0).Size(10000)
            .MatchAll());
            var scopes = response.Hits.ToList();
            Scope scope = null;

            List<string> listScope = new List<string>();


            foreach (var item in scopes)
            {
                scope = new Scope
                {
                    market = item.Source.market
                };
                //If the market already exists then it will be ignored.
                if (!listScope.Contains(scope.market)){

                    listScope.Add(scope.market);
                }
            }
            ViewBag.Scopes = listScope.OrderBy(x => x);
            return View();
        }

        //Here we get all properties from our property index.
        public async Task<List<Property>> GetProperties(string market, string term)

        
        {
            Property property = null;
            var response = await _elasticClient.SearchAsync<Property>(s => s
            .Index("properties")
            .Size(10)
            .Query(q => q
            .Match(m => m.Field(f => f.name).Query(term))));
            var properties = response.Hits.ToList();
            List<Property> propertyList = new List<Property>();
            foreach (var item in properties)
            {
                //If the market is not selected or is DFW then all properties that
                //contain the string inputted will be displayed.
                if (string.IsNullOrEmpty(market) || market == "DFW")
                {
                    property = new Property
                    {
                        name = item.Source.name
                    };
                    propertyList.Add(property);
                }
                //If market is specified, then only properties with within the same
                //market will be displayed.
                else if(item.Source.market == market)
                {
                    property = new Property
                    {
                        name = item.Source.name
                    };
                    propertyList.Add(property);
                }                   
                
            }     
            
            return propertyList;
        }

        //Here we get all managements from our management index.
        public async Task<List<Management>> GetManagements(string market, string term)

        {
            Management mgmt = null;
            var response = await _elasticClient.SearchAsync<Management>(s => s
            .Index("managements")
            .Size(10)
            .Query(q => q
            .Match(m => m.Field(f => f.name).Query(term))));
            var management = response.Hits.ToList();
            List<Management> mgmtList = new List<Management>();
            foreach (var item in management)
            {
                //If the market is not selected or is DFW then all managements that
                //contain the string inputted will be displayed.
                if (string.IsNullOrEmpty(market) || market == "DFW")
                {
                    mgmt = new Management
                    {
                        name = item.Source.name
                    };
                    mgmtList.Add(mgmt);
                }
                //If market is specified, then only managements within the same
                //market will be displayed.
                else if (item.Source.market == market)
                {
                    mgmt = new Management
                    {
                        name = item.Source.name
                    };
                    mgmtList.Add(mgmt);
                }

            }

            return mgmtList;
        }

        //Here we Bind our property and management classes so all the information gotten from
        //our previous function can be displayed.
        public List<string> GetBinding(string market, string term)
        {
            BindingModel binding = new BindingModel();
            binding.Properties = GetProperties(market, term);
            binding.Managements = GetManagements(market, term);
            List<string> listMgmtProd = new List<string>();
            foreach (var property in binding.Properties.Result)
            {
                listMgmtProd.Add($"Property: {property.name}");
            }
            foreach (var property in binding.Managements.Result) 
            { 
                listMgmtProd.Add($"Management: {property.name}");
            } 

            return listMgmtProd;
        }
    }
}
