terraform {
  required_providers {
    keycloak = {
      source  = "mrparkers/keycloak"
      version = "4.4.0"
    }
  }
  backend "local" {}
}

provider "keycloak" {
    client_id     = "admin-cli"
    username      = "admin"
    password      = "admin"
    url           = "http://keycloak-local:8080"
}
