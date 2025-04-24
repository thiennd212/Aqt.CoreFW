using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // Using System.Text.Json
using Aqt.CoreFW.OrganizationUnits; // Using for OrganizationUnitStatus

namespace Aqt.CoreFW.Application.Contracts.OrganizationUnits.Dtos;

public class OrganizationUnitTreeNodeDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("parent")]
    public string Parent { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("state")]
    public NodeStateDto State { get; set; } = new NodeStateDto();

    [JsonPropertyName("children")]
    public bool Children { get; set; }

    [JsonPropertyName("data")]
    public TreeNodeDataDto? Data { get; set; }
    public int Order { get; set; }
    public string Code { get; set; }

    public OrganizationUnitTreeNodeDto() { }

    public OrganizationUnitTreeNodeDto(Guid id, Guid? parentId, string text, bool hasChildren = false)
    {
        Id = id.ToString();
        Parent = parentId?.ToString() ?? "#";
        Text = text;
        Children = hasChildren;
    }
}

public class NodeStateDto
{
    [JsonPropertyName("opened")]
    public bool Opened { get; set; } = false;

    [JsonPropertyName("selected")]
    public bool Selected { get; set; } = false;

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; } = false;
}

public class TreeNodeDataDto
{
     public string? ManualCode { get; set; }
     public string? Code { get; set; }
     public OrganizationUnitStatus Status { get; set; }
     public int Order { get; set; }
} 