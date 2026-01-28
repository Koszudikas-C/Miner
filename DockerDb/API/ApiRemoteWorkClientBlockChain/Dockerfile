FROM postgres:17.5

COPY ../../Script/PostgresDb/DataBase/* /Script/PostgresDb/DataBase/
COPY ../../Script/PostgresDb/Previleges/* /Script/PostgresDb/Previleges/
COPY ../../Script/PostgresDb/Users/* /Script/PostgresDb/Users/
COPY  /Script/PostgresDb/init.sh /Script/PostgresDb/

RUN chmod +x /Script/PostgresDb/init.sh

RUN cp /Script/PostgresDb/Users/alterPwdUserPostegres.sql /docker-entrypoint-initdb.d/00-alterPwdUserWorkClient.sql
RUN cp /Script/PostgresDb/Users/createUserWorkClient.sql /docker-entrypoint-initdb.d/01-createUserWorkClient.sql
RUN cp /Script/PostgresDb/Users/createUserFromDatabase.sql /docker-entrypoint-initdb.d/02-createUserFromDatabase.sql
RUN cp /Script/PostgresDb/DataBase/createWorkClientDatabase.sql /docker-entrypoint-initdb.d/03-createWorkClientDatabase.sql
RUN cp /Script/PostgresDb/Previleges/previlegesWorkClientUser.sql /docker-entrypoint-initdb.d/04-previlegesWorkClientUser.sql

#ENTRYPOINT ["/Script/PostgresDb/init.sh"]