#!/bin/bash

set -e
pg_ctl -D /var/lib/postgresql/data -1 /temp/pg.log start


util pg_isready -U postgres; do
echo "PostgreSQL is not ready yet. Waiting..."
  sleep 1
done


psql -U postgres -f /Script/PostgresDb/Users/createUserWorkClient.sql
psql -U postgres -f /Script/PostgresDb/DataBase/createWorkClientDatabase.sql
psql -U postgres -f /Script/PostgresDb/Previleges/previlegesWorkClientUser.sql

pg_ctl -D /var/lib/postgresql/data stop

exec /docker-entrypoint.sh postgres