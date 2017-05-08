using System;

namespace PartyWifi.Server.Components
{
    /// <summary>
    /// State flags of images
    /// </summary>
    [Flags]
    public enum ImageState
    {
        /// <summary>
        /// Image was approved
        /// </summary>
        Approved = 0x1,

        /// <summary>
        /// Image was selected as hidden by organizor
        /// </summary>
        Hidden = 0x2,
    }
}