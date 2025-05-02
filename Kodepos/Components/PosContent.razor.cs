using Kodepos.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;

namespace Kodepos.Components;
public partial class PosContent
{
    const string JSFile = "/Components/PosContent.razor.js";
    int _currentPage = 1;
    Sort? _currentSort;
    int _pageSize = 50;
    PosLeaflet _postLeafLet;

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
            PagedPostCode = await JSModule.InvokeAsync<PagedPostalCode>("postcodeRepository.searchWithPagination", _searchString, _currentPage, _pageSize, _currentSort);
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
