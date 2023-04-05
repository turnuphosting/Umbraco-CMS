﻿using System.Net.Mime;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Cms.Api.Management.Controllers.Dictionary;

public class ExportDictionaryController : DictionaryControllerBase
{
    private readonly IDictionaryItemService _dictionaryItemService;
    private readonly IEntityXmlSerializer _entityXmlSerializer;

    public ExportDictionaryController(IDictionaryItemService dictionaryItemService, IEntityXmlSerializer entityXmlSerializer)
    {
        _dictionaryItemService = dictionaryItemService;
        _entityXmlSerializer = entityXmlSerializer;
    }

    [HttpGet("{id:guid}/export")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Export(Guid id, bool includeChildren = false)
    {
        IDictionaryItem? dictionaryItem = await _dictionaryItemService.GetAsync(id);
        if (dictionaryItem is null)
        {
            return await Task.FromResult(NotFound());
        }

        XElement xml = _entityXmlSerializer.Serialize(dictionaryItem, includeChildren);

        return await Task.FromResult(File(Encoding.UTF8.GetBytes(xml.ToDataString()), MediaTypeNames.Application.Octet, $"{dictionaryItem.ItemKey}.udt"));
    }
}