using System.Globalization;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace CodeGen;

public class DataTableModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var defaultCultureInfo = CultureInfo.InvariantCulture;
        var request = bindingContext.HttpContext.Request;

        // Retrieve request data
        var draw = Convert.ToInt32(request.Query["draw"], defaultCultureInfo);
        var start = Convert.ToInt32(request.Query["start"], defaultCultureInfo);
        var length = Convert.ToInt32(request.Query["length"], defaultCultureInfo);

        // Search
        var search = new Search
        {
            Value = request.Query["search[value]"].ToString(),
            Regex = Convert.ToBoolean(request.Query["search[regex]"], defaultCultureInfo)
        };

        // Order
        var o = 0;
        var order = new List<ColumnOrder>();
        while (!StringValues.IsNullOrEmpty(request.Query[$"order[{o}][column]"]))
        {
            order.Add(new ColumnOrder
            {
                Column = Convert.ToInt32(request.Query[$"order[{o}][column]"], defaultCultureInfo),
                Dir = request.Query[$"order[{o}][dir]"] == nameof(ColumnOrderDir.asc) ? ColumnOrderDir.asc : ColumnOrderDir.dec
            });
            o++;
        }

        // Columns
        var c = 0;
        var columns = new List<Column>();
        while (!StringValues.IsNullOrEmpty(request.Query[$"columns[{c}][data]"]))
        {
            columns.Add(new Column
            {
                Data = request.Query[$"columns[{c}][data]"].ToString(),
                Name = request.Query[$"columns[{c}][name]"].ToString(),
                Orderable = Convert.ToBoolean(request.Query[$"columns[{c}][orderable]"], defaultCultureInfo),
                Searchable = Convert.ToBoolean(request.Query[$"columns[{c}][searchable]"], defaultCultureInfo),
                Search = new Search
                {
                    Value = request.Query[$"columns[{c}][search][value]"].ToString(),
                    Regex = Convert.ToBoolean(request.Query[$"columns[{c}][search][regex]"], defaultCultureInfo)
                }
            });
            c++;
        }

        // SearchBuilder
        var sb = 0;
        var searchBuilder = new SearchBuilder();
        searchBuilder.Logic = SearchBuilder.ToSearchLogic(request.Query[$"searchBuilder[logic]"].ToString());
        while (!StringValues.IsNullOrEmpty(request.Query[$"searchBuilder[criteria][{sb}][data]"]))
        {
            var searchCondition = new SearchCondition
            {
                Type = SearchCondition.ToSearchColumnType(request.Query[$"searchBuilder[criteria][{sb}][type]"].ToString()),
                Data = request.Query[$"searchBuilder[criteria][{sb}][data]"].ToString().Trim(),
                OriginalData = request.Query[$"searchBuilder[criteria][{sb}][origData]"].ToString(),
                Value1 = request.Query[$"searchBuilder[criteria][{sb}][value1]"].ToString(),
                Value2 = request.Query[$"searchBuilder[criteria][{sb}][value2]"].ToString()
            };

            if (searchCondition.Type == SearchColumnType.Num)
            {
                searchCondition.NumCondition = SearchCondition.ToSearchNumCondition(request.Query[$"searchBuilder[criteria][{sb}][condition]"].ToString());
            }
            else
            {
                searchCondition.StrCondition = SearchCondition.ToSearchStrCondition(request.Query[$"searchBuilder[criteria][{sb}][condition]"].ToString());
            }
            searchBuilder.SearchConditions.Add(searchCondition);
            sb++;
        }

        var result = new DataTableParameters
        {
            Draw = draw,
            Start = start,
            Length = length,
            Search = search,
            Order = order,
            Columns = columns,
            SearchBuilder = searchBuilder
        };

        bindingContext.Result = ModelBindingResult.Success(result);
        return Task.CompletedTask;
    }
}
