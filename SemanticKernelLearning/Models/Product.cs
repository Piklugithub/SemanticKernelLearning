using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticKernelLearning.Models
{
    public sealed record Product(
    int Id,
    string Name,
    int Quantity);
}
