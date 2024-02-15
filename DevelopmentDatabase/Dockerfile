FROM mcr.microsoft.com/mssql/server:2019-latest

USER root
ENV SA_PASSWORD "Change_this_password10"
ENV ACCEPT_EULA "Y"

EXPOSE 1433

WORKDIR /usr/config
COPY . /usr/config

RUN chmod +x /usr/config/wait-for-db.sh


HEALTHCHECK --interval=12s --timeout=10s --start-period=5s --retries=25 CMD /usr/config/wait-for-db.sh || exit 1
