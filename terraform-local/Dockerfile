FROM hashicorp/terraform:latest

WORKDIR /app
COPY ./terraform-local/* .

RUN /bin/terraform init

ENTRYPOINT ["/bin/terraform", "apply", "-auto-approve"]