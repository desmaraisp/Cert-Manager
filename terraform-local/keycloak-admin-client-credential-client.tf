/*
This client serves a single purposes. Allow swagger client_credentials authentication
and CertRenwer auth. As such, it needs to have access to all groups and scopes in the realm.
It also allows direct access auth for swaggerUI authentication
*/

resource "keycloak_openid_client" "admin_credentials_client" {
  realm_id  = keycloak_realm.realm.id
  client_id = "admin-client-credentials"

  name        = "private admin client"
  enabled     = true
  web_origins = ["*"]

  access_type = "CONFIDENTIAL"
  valid_redirect_uris = [
    "http://localhost:3000/oidc-callback"
  ]

  client_secret                = "123"
  implicit_flow_enabled        = true
  service_accounts_enabled     = true
  direct_access_grants_enabled = true
}
resource "keycloak_openid_audience_protocol_mapper" "admin_credentials_audience_mapper" {
  realm_id = keycloak_realm.realm.id

  client_id = keycloak_openid_client.admin_credentials_client.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}
resource "keycloak_generic_protocol_mapper" "admin_hardcode_attribute_mapper" {
  realm_id        = keycloak_realm.realm.id
  client_id       = keycloak_openid_client.admin_credentials_client.id
  name            = "groups-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-hardcoded-claim-mapper"
  config = {
    "claim.name" = "groups"
    "claim.value" : "[\"foo\", \"bar\"]"
    "jsonType.label"             = "JSON"
    "id.token.claim"             = "true"
    "access.token.claim"         = "true"
    "userinfo.token.claim"       = "true"
    "access.tokenResponse.claim" = "false"
  }
}

resource "keycloak_openid_client_optional_scopes" "admin_optional_scopes" {
  realm_id  = keycloak_realm.realm.id
  client_id = keycloak_openid_client.admin_credentials_client.id

  optional_scopes = [
    keycloak_openid_client_scope.cert-write-scope.name,
    keycloak_openid_client_scope.cert-read-scope.name
  ]
}
