using System;

namespace FSALib
{
    public interface IArchiveEntry : IDisposable, ICloneable
    {
        /// <summary>
        /// Gets or sets the name of the archive entry.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the size of the archive entry in bytes.
        /// </summary>
        uint Length { get; }
    }
}