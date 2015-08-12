SELECT table_name as Table, '' as Schema 
FROM information_schema.tables
WHERE table_schema = 'public' AND table_type = 'VIEW'