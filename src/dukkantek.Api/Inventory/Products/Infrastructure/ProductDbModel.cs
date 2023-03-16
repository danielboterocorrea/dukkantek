using System;
using System.Collections.Generic;

namespace dukkantek.Api.Models;

public partial class ProductDbModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Barcode { get; set; }

    public string Description { get; set; }

    public string CategoryName { get; set; }

    public bool Weighted { get; set; }

    public string Status { get; set; }
}
