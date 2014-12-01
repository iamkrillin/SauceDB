using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Attributes
{
    /// <summary>
    /// Allows you to specify the type mapping CLR->DB,
    /// Its worth noting, not all datastores support all types, in this case a different type will be chosen
    /// most types will list the fallbacks (depending on datastore)
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Will use the default type mapping
        /// </summary>
        Default,

        /// <summary>
        /// Use this if you want to specify a user string for the field type,
        /// this option is not cross db compatible and is passed straight to the db,
        /// this option also requires the use of the FieldTypeString
        /// </summary>
        UserString,

        /// <summary>
        /// varchar
        /// </summary>
        String,

        /// <summary>
        /// nvarchar
        /// </summary>
        UnicodeString,

        /// <summary>
        /// date -> string
        /// </summary>
        Date,

        /// <summary>
        /// boolean -> bit -> int(1)
        /// </summary>
        Bool,

        /// <summary>
        /// int
        /// </summary>
        Int,

        /// <summary>
        /// bigint
        /// </summary>
        Long,

        /// <summary>
        /// real
        /// </summary>
        Real,

        /// <summary>
        /// longblob, varbinary(MAX)
        /// </summary>
        Binary,

        /// <summary>
        /// generally varchar(1)
        /// </summary>
        Char,

        /// <summary>
        /// nvarchar(1)
        /// </summary>
        UnicodeChar,

        /// <summary>
        /// text
        /// </summary>
        Text,

        /// <summary>
        /// time -> string 
        /// </summary>
        Time,

        /// <summary>
        /// ntext
        /// </summary>
        UnicodeText,

        /// <summary>
        /// ntext
        /// </summary>
        Money
    }
}
