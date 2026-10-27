using SemanticKernelLearning.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticKernelLearning.Repositories
{
    public interface IInventoryRepository
    {
        Task<Product?> GetByIdAsync(
            int productId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> SearchAsync(
            string searchTerm,
            CancellationToken cancellationToken = default);
    }
}
