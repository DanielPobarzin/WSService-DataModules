FROM postgres:16

   ENV POSTGRES_USER=postgres
   ENV POSTGRES_PASSWORD=19346jaidj
   ENV POSTGRES_DB=ClientConfigs

   # Копируем SQL-скрипты в директорию инициализации
   COPY Dumps/ClientConfigsData.sql /docker-entrypoint-initdb.d/ClientConfigsData.sql

   RUN mkdir -p /usr/local/bin/docker-entrypoint-initdb.d && \
    echo "initdb --encoding=UTF8 --locale=C.UTF-8" >> /usr/local/bin/docker-entrypoint-initdb.d/init.sh