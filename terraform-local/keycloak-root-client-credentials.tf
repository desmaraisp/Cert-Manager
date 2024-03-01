resource "keycloak_openid_client" "root_client_credentials_client" {
  realm_id  = "master"
  client_id = "root-client"

  name        = "root client"
  enabled     = true
  web_origins = ["*"]
  access_token_lifespan = 3600

  access_type = "CONFIDENTIAL"
  valid_redirect_uris = [
    "http://localhost:3222/openid-callback"
  ]

  client_secret                = "BAD_CLIENT_SECRET"
  standard_flow_enabled        = true
  service_accounts_enabled     = true
  direct_access_grants_enabled = true
}

resource "keycloak_openid_audience_protocol_mapper" "root_audience_mapper" {
  realm_id  = "master"
  client_id = keycloak_openid_client.root_client_credentials_client.id
  name      = "root-audience-mapper"

  included_custom_audience = "cert-manager"
}

resource "keycloak_openid_client_scope" "root-cert-read-scope" {
  realm_id               = "master"
  name                   = "cert-manager/read"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}
resource "keycloak_openid_client_scope" "root-cert-write-scope" {
  realm_id               = "master"
  name                   = "cert-manager/write"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}

resource "keycloak_openid_client_optional_scopes" "root_client_optional_scopes" {
  realm_id  = "master"
  client_id = keycloak_openid_client.root_client_credentials_client.id

  optional_scopes = [
    keycloak_openid_client_scope.root-cert-write-scope.name,
    keycloak_openid_client_scope.root-cert-read-scope.name
  ]
}
