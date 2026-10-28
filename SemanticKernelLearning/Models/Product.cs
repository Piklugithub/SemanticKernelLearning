using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticKernelLearning.Models
{
    public sealed class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
