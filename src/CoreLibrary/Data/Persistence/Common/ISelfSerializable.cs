﻿using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Common
{
    /// <summary>
    /// Can serialize itself 
    /// </summary>
    public interface ISelfSerializable
    {
        Task DeserializeAsync();

        Task SerializeAsync();
    }
}