/*
This client serves a single purposes. Allow CertAgent authentication scoped to a specific org (foo)
As such, it need only the serverAgent roles and should only have the foo org
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
  name            = "roles-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-hardcoded-claim-mapper"
  config = {
    "claim.name" = "roles"
    "claim.value" : "[\"foo.ServerAgent\"]"
    "jsonType.label"             = "JSON"
    "id.token.claim"             = "true"
    "access.token.claim"         = "true"
    "userinfo.token.claim"       = "true"
    "access.tokenResponse.claim" = "false"
  }
}