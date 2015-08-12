SELECT table_name as TableName, column_name as ColumnName, 
       case Data_type
	WHEN 'ARRAY' THEN replace(udt_name, '_', '') || '[]'
	ELSE Data_Type
       END as DataType, '' as Schema,
       CASE (
              SELECT const2.CONSTRAINT_TYPE	
              FROM  INFORMATION_SCHEMA.KEY_COLUMN_USAGE const	
              INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS const2 on const2.TABLE_NAME = col.TABLE_NAME AND const2.TABLE_SCHEMA = col.TABLE_SCHEMA AND 
                                                              const2.TABLE_CATALOG = col.TABLE_CATALOG AND const2.CONSTRAINT_NAME = const.CONSTRAINT_NAME AND 
                                                              const2.CONSTRAINT_TYPE = 'PRIMARY KEY'	WHERE const.COLUMN_NAME = col.COLUMN_NAME
            ) 
      WHEN 'PRIMARY KEY' THEN 1 ELSE 0 END as PrimaryKey 
FROM information_schema.columns col 
WHERE table_name IN (	
                      SELECT table_name as Table 
                      FROM information_schema.tables
                      WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
                    );