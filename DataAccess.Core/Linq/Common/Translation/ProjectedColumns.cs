using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
#pragma warning disable 1591

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Result from calling ColumnProjector.ProjectColumns
    /// </summary>
    public sealed class ProjectedColumns
    {
        public Expression Projector { get; private set; }
        public ReadOnlyCollection<ColumnDeclaration> Columns { get; private set; }

        public ProjectedColumns(Expression projector, ReadOnlyCollection<ColumnDeclaration> columns)
        {
            this.Projector = projector;
            this.Columns = columns;
        }
    }
}
