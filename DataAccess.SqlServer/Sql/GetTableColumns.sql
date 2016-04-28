--its WAY faster to pull the pices into temp tables then join at the end, go figure..

SELECT 
	ORDINAL_POSITION,
	OBJECT_ID(table_schema + '.' + TABLE_NAME) TableObject
INTO #PrimaryKey_Temp
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1

select 
	cols.ORDINAL_POSITION, 
	cols.TABLE_SCHEMA 'Schema', 
	cols.TABLE_NAME AS TableName, 
	cols.COLUMN_NAME AS ColumnName, 
	Data_Type as DataType,
	OBJECT_ID(cols.TABLE_SCHEMA + '.' + cols.TABLE_NAME) TableObject,
	cols.Table_Schema + '.' + cols.TABLE_NAME as FullName,
	COLUMN_DEFAULT as 'DefaultValue',
	CASE 
		WHEN cols.Data_Type = 'datetime' THEN NULL
		ELSE 
		CASE ISNULL(CHARACTER_MAXIMUM_LENGTH, DATETIME_PRECISION) 
			WHEN -1 THEN 'MAX' 
			ELSE convert(varchar, ISNULL(CHARACTER_MAXIMUM_LENGTH, DATETIME_PRECISION)) 
		END 
		END as 'ColumnLength'
INTO #Base_Temp
FROM  INFORMATION_SCHEMA.TABLES tbls
INNER JOIN INFORMATION_SCHEMA.COLUMNS cols on cols.TABLE_SCHEMA = tbls.TABLE_SCHEMA AND cols.TABLE_NAME = tbls.TABLE_NAME
WHERE tbls.TABLE_TYPE='BASE TABLE'

select 
	base.*,
	case when pkeys.ORDINAL_POSITION iS NULL THEN 0 ELSE 1 end 'PrimaryKey'
from #Base_Temp base
LEFT OUTER JOIN #PrimaryKey_Temp pkeys on pkeys.TableObject = base.TableObject AND pkeys.ORDINAL_POSITION = base.ORDINAL_POSITION


