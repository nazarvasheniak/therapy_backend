using System;
namespace Domain.Models
{
    public interface IDeletableObject
    {
        /// <summary>
        /// Архивирован
        /// </summary>
        bool Deleted { get; set; }
    }
}
