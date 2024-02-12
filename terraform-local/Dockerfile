FROM hashicorp/terraform:latest

WORKDIR /app
COPY ./terraform-local/* .

RUN /bin/terraform init

ENTRYPOINT ["./entrypoint.sh"]