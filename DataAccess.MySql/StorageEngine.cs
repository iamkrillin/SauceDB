﻿namespace DataAccess.MySql
{
    /// <summary>
    /// Mysql storage Engines
    /// </summary>
    public enum StorageEngine
    {
        /// <summary>
        ///Innodb, this is the default 
        /// </summary>
        InnoDB,

        /// <summary>
        ///MyISAM
        /// </summary>
        MyISAM,

        /// <summary>
        /// In Memory
        /// </summary>
        Memory
    }
}
