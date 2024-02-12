resource "keycloak_openid_client" "client_credentials_client" {
  realm_id  = "master"
  client_id = "dev-client"

  name        = "development client"
  enabled     = true
  web_origins = ["*"]

  access_type = "CONFIDENTIAL"
  valid_redirect_uris = [
    "http://localhost:3222/openid-callback"
  ]

  client_secret                = "BAD_CLIENT_SECRET"
  standard_flow_enabled        = true
  service_accounts_enabled     = true
  direct_access_grants_enabled = true
}

resource "keycloak_openid_audience_protocol_mapper" "audience_mapper" {
  realm_id  = "master"
  client_id = keycloak_openid_client.client_credentials_client.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}

resource "keycloak_openid_client_scope" "cert-read-scope" {
  realm_id               = "master"
  name                   = "cert-manager/read"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}
resource "keycloak_openid_client_scope" "cert-write-scope" {
  realm_id               = "master"
  name                   = "cert-manager/write"
  description            = "Allows read-only access to certManager api"
  include_in_token_scope = true
}

resource "keycloak_openid_client_optional_scopes" "client_optional_scopes" {
  realm_id  = "master"
  client_id = keycloak_openid_client.client_credentials_client.id

  optional_scopes = [
    keycloak_openid_client_scope.cert-write-scope.name,
    keycloak_openid_client_scope.cert-read-scope.name
  ]
}

resource "keycloak_user" "user_with_initial_password" {
  realm_id = "master"
  username = "alice"
  enabled  = true

  email = "alice@domain.com"

  initial_password {
    value     = "VERY_BAD_PASSWORD"
    temporary = true
  }
}
