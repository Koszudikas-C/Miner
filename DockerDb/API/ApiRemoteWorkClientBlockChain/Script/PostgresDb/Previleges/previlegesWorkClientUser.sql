\connect work_client_database

GRANT CONNECT ON DATABASE work_client_database TO work_client;
GRANT USAGE, CREATE ON SCHEMA public TO work_client;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO work_client;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE ON TABLES TO work_client;
