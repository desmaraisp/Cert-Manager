/*
This client serves a single purposes. Allow CertAgent authentication scoped to a specific user group (foo)
As such, it need only the read scope and should only have the foo group
*/

resource "keycloak_openid_client" "foo_credentials_client" {
  realm_id  = keycloak_realm.realm.id
  client_id = "foo-client-credentials"

  name        = "private foo client"
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
resource "keycloak_openid_audience_protocol_mapper" "foo_credentials_audience_mapper" {
  realm_id = keycloak_realm.realm.id

  client_id = keycloak_openid_client.foo_credentials_client.id
  name      = "audience-mapper"

  included_custom_audience = "cert-manager"
}
resource "keycloak_generic_protocol_mapper" "foo_hardcode_attribute_mapper" {
  realm_id        = keycloak_realm.realm.id
  client_id       = keycloak_openid_client.foo_credentials_client.id
  name            = "groups-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-hardcoded-claim-mapper"
  config = {
    "claim.name" = "groups"
    "claim.value" : "[\"foo\"]"
    "jsonType.label"             = "JSON"
    "id.token.claim"             = "true"
    "access.token.claim"         = "true"
    "userinfo.token.claim"       = "true"
    "access.tokenResponse.claim" = "false"
  }
}

resource "keycloak_openid_client_optional_scopes" "foo_optional_scopes" {
  realm_id  = keycloak_realm.realm.id
  client_id = keycloak_openid_client.foo_credentials_client.id

  optional_scopes = [
    keycloak_openid_client_scope.cert-read-scope.name
  ]
}
