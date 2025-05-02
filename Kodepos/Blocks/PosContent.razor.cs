using Kodepos.Models;
using Kodepos.Services;
using Kodepos.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;

namespace Kodepos;
public partial class PosContent : IAsyncDisposable
{
    const string JSFile = "/Blocks/PosContent.razor.js";
    int _currentPage = 1;
    Sort? _currentSort;
    Debounce _debounce;
    string? _optionTitle;
    int _pageSize = 50;
    bool _pageSizeOptionOpen;
    PosLeaflet? _postLeafLet;
    ElementReference _searchInputRef;
    string? _searchString;
    [Inject]
    IDataConverter DataConverter { get; set; } = default!;

    readonly Dictionary<string, bool> _filterOptions = new Dictionary<string, bool>()
    {
        {"province", true },
        {"regency", true },
        {"district", true },
        {"village", true },
    };
    readonly Dictionary<string, string> _fieldNameMap = new Dictionary<string, string>
    {
        { "province", "Provinsi" },
        { "regency", "Kabupaten/Kota" },
        { "district", "Kecamatan" },
        { "village", "Kelurahan/Desa" }
    };

    async Task ConvertDataAsync(string type)
    {
        if (type == "csv")
        {
            if (PagedPostCode.Items?.Count > 0)
            {
                await DataConverter.GenerateCSVAsync(PagedPostCode.Items);
            }
        }
        if (type == "xls")
        {
            if (PagedPostCode.Items?.Count > 0)
            {
                await DataConverter.GenerateXLSAsync(PagedPostCode.Items);
            }
        }
        if (type == "pdf")
        {
            if (PagedPostCode.Items?.Count > 0)
            {
                await DataConverter.GeneratePDFAsync(PagedPostCode.Items);
            }
        }
    }

    List<string> ActiveFilters => _filterOptions
         .Where(x => x.Value)
         .Select(x => _fieldNameMap.TryGetValue(x.Key, out var label) ? label : x.Key)
         .ToList();

    public PosContent()
    {
        _debounce = new Debounce();
    }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    public bool IsLoading { get; set; }
    public PagedPostalCode PagedPostCode { get; set; } = new();
    public bool ResultVisible => PagedPostCode.Items?.Count > 0;
    public string? SearchString { get; set; }
    public SortCollection Sorting { get; set; } = new();
    public PostalCodeStats Stats { get; set; } = new();
    DotNetObjectReference<PosContent>? DotNetObject { get; set; }
    IJSObjectReference? JSModule { get; set; }
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    public async Task DebounceSearchPostCodeAsync()
    {
        await _debounce.RunAsync(100, SearchPostCodeAsync);
    }
    public async ValueTask DisposeAsync()
    {
        DotNetObject?.Dispose();
        if (JSModule != null)
        {
            await JSModule.DisposeAsync();
        }
    }
    [JSInvokable]
    public void OnDataUpdated(PostalCodeStats stats)
    {
        Stats.ProvinceCount += stats.ProvinceCount;
        Stats.RegencyCount += stats.RegencyCount;
        Stats.DistrictCount += stats.DistrictCount;
        Stats.VillageCount += stats.VillageCount;
        StateHasChanged();
    }
    public async Task SearchPostCodeAsync()
    {
        if (JSModule != null)
        {
            PagedPostCode = await JSModule.InvokeAsync<PagedPostalCode>("postcodeRepository.searchWithPagination", _searchString, _currentPage, _pageSize, _currentSort, _filterOptions);
        }
        else
        {
            PagedPostCode = new PagedPostalCode();
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", JSFile);
            if (JSModule != null)
            {
                DotNetObject ??= DotNetObjectReference.Create(this);
                await JSModule.InvokeVoidAsync("getStats", DotNetObject, nameof(OnDataUpdated));
                await SearchPostCodeAsync();
                InvokeAsync(StateHasChanged);
            }
        }
    }
    protected override void OnInitialized()
    {
        _optionTitle = _pageSize.ToString();
    }
    async Task ChangePageSizeAsync(int pageSize)
    {
        _pageSizeOptionOpen = false;
        if (_pageSize != pageSize)
        {
            _pageSize = pageSize;
            _optionTitle = _pageSize.ToString();
            _currentPage = 1;
            await SearchPostCodeAsync();
        }
    }
    async Task GoNextAsync()
    {
        if (PagedPostCode.HasNext)
        {
            _currentPage += 1;
            await SearchPostCodeAsync();
        }
    }
    async Task GoPrevAsync()
    {
        if (PagedPostCode.HasPrevious)
        {
            _currentPage -= 1;
            await SearchPostCodeAsync();
        }
    }
    async Task OnClearAsync()
    {
        _searchString = string.Empty;
        await SearchPostCodeAsync();
        await _searchInputRef.FocusAsync();
    }
    async Task OnMouseOver(PostalCode postCode)
    {
        await _postLeafLet.SetViewAsync(new LatLng(postCode.Latitude, postCode.Longitude));
    }
    async Task SortByAsync(string name)
    {
        if (_currentSort?.Name == name)
        {
            _currentSort.Dir = _currentSort.Dir == "asc" ? "dsc" : "asc";
        }
        else
        {
            _currentSort = new Sort
            {
                Name = name,
                Dir = "asc"
            };
        }
        await SearchPostCodeAsync();
    }
}
